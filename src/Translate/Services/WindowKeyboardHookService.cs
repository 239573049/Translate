using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Translate.Services;

/// <summary>
/// 键盘钩子
/// [以下代码来自某网友，并非本人原创]
/// </summary>
public class WindowKeyboardHookService : IKeyboardHookService
{
    public Action<VirtualKeyCodes> KeyDownEvent { get; set; }

    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

    static int hKeyboardHook = 0; //声明键盘钩子处理的初始值

    //值在Microsoft SDK的Winuser.h里查询
    // http://www.bianceng.cn/Programming/csharp/201410/45484.htm
    public const int WH_KEYBOARD_LL = 13; //线程键盘钩子监听鼠标消息设为2，全局键盘监听鼠标消息设为13

    HookProc KeyboardHookProcedure; //声明KeyboardHookProcedure作为HookProc类型

    //键盘结构
    [StructLayout(LayoutKind.Sequential)]
    public class KeyboardHookStruct
    {
        public int vkCode; //定一个虚拟键码。该代码必须有一个价值的范围1至254
        public int scanCode; // 指定的硬件扫描码的关键
        public int flags; // 键标志
        public int time; // 指定的时间戳记的这个讯息
        public int dwExtraInfo; // 指定额外信息相关的信息
    }

    //使用此功能，安装了一个钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


    //调用此函数卸载钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern bool UnhookWindowsHookEx(int idHook);


    //使用此功能，通过信息钩子继续下一个钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

    // 取得当前线程编号（线程钩子需要用到）
    [DllImport("kernel32.dll")]
    static extern int GetCurrentThreadId();

    //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string name);


    public void Start()
    {
        // 安装键盘钩子
        if (hKeyboardHook == 0)
        {
            KeyboardHookProcedure = new HookProc(KeyboardHookProc);
            hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure,
                GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            //hKeyboardHook = SetWindowsHookEx(WH_KEY11BOARD_LL, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            //************************************
            //键盘线程钩子
            //SetWindowsHookEx( 2,KeyboardHookProcedure, IntPtr.Zero, GetCurrentThreadId());//指定要监听的线程idGetCurrentThreadId(),
            //键盘全局钩子,需要引用空间(using System.Reflection;)
            //SetWindowsHookEx( 13,MouseHookProcedure,Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),0);
            //
            //关于SetWindowsHookEx (int idHook, HookProc lpfn, IntPtr hInstance, int threadId)函数将钩子加入到钩子链表中，说明一下四个参数：
            //idHook 钩子类型，即确定钩子监听何种消息，上面的代码中设为2，即监听键盘消息并且是线程钩子，如果是全局钩子监听键盘消息应设为13，
            //线程钩子监听鼠标消息设为7，全局钩子监听鼠标消息设为14。lpfn 钩子子程的地址指针。如果dwThreadId参数为0 或是一个由别的进程创建的
            //线程的标识，lpfn必须指向DLL中的钩子子程。 除此以外，lpfn可以指向当前进程的一段钩子子程代码。钩子函数的入口地址，当钩子钩到任何
            //消息后便调用这个函数。hInstance应用程序实例的句柄。标识包含lpfn所指的子程的DLL。如果threadId 标识当前进程创建的一个线程，而且子
            //程代码位于当前进程，hInstance必须为NULL。可以很简单的设定其为本应用程序的实例句柄。threaded 与安装的钩子子程相关联的线程的标识符
            //如果为0，钩子子程与所有的线程关联，即为全局钩子
            //************************************
            //如果SetWindowsHookEx失败
            if (hKeyboardHook == 0)
            {
                Stop();
                throw new Exception("安装键盘钩子失败");
            }
        }
    }

    public void Stop()
    {
        bool retKeyboard = true;


        if (hKeyboardHook != 0)
        {
            retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
            hKeyboardHook = 0;
        }

        if (!(retKeyboard)) throw new Exception("卸载钩子失败！");
    }

    //ToAscii职能的转换指定的虚拟键码和键盘状态的相应字符或字符
    [DllImport("user32")]
    public static extern int To1Ascii(int uVirtKey, //[in] 指定虚拟关键代码进行翻译。
        int uScanCode, // [in] 指定的硬件扫描码的关键须翻译成英文。高阶位的这个值设定的关键，如果是（不压）
        byte[] lpbKeyState, // [in] 指针，以256字节数组，包含当前键盘的状态。每个元素（字节）的数组包含状态的一个关键。如果高阶位的字节是一套，关键是下跌（按下）。在低比特，如果设置表明，关键是对切换。在此功能，只有肘位的CAPS LOCK键是相关的。在切换状态的NUM个锁和滚动锁定键被忽略。
        byte[] lpwTransKey, // [out] 指针的缓冲区收到翻译字符或字符。
        int fuState); // [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.

    //获取按键的状态
    [DllImport("user32")]
    public static extern int GetKeyboardState(byte[] pbKeyState);


    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern short GetKeyState(int vKey);

    private const int WM_KEYDOWN = 0x100; //KEYDOWN
    private const int WM_KEYUP = 0x101; //KEYUP
    private const int WM_SYSKEYDOWN = 0x104; //SYSKEYDOWN
    private const int WM_SYSKEYUP = 0x105; //SYSKEYUP

    private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
    {
        // 侦听键盘事件
        if (nCode >= 0)
        {
            KeyboardHookStruct MyKeyboardHookStruct =
                (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            // raise KeyDown
            if (KeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
            {
                KeyDownEvent.Invoke((VirtualKeyCodes)MyKeyboardHookStruct.vkCode);
            }
        }

        return 0;
    }

    public void Dispose()
    {
        Stop();
    }
}

public static class KeyboardUtilities
{
    public static bool IsCtrlAltPressed()
    {
        return (GetKeyState(VirtualKeyCodes.VK_CONTROL) < 0) &&
               (GetKeyState(VirtualKeyCodes.VK_MENU) < 0);
    }

    [DllImport("user32.dll")]
    private static extern short GetKeyState(VirtualKeyCodes nVirtKey);
}

public enum VirtualKeyCodes
{
    // 0x0A-0x0B: 预留
    VK_CLEAR = 0x0C, // CLEAR 键
    VK_RETURN = 0x0D, // Enter 键

    // 0x0E-0x0F: 未分配
    VK_SHIFT = 0x10, // SHIFT 键
    VK_CONTROL = 0x11, // CTRL 键
    VK_MENU = 0x12, // Alt 键
    VK_PAUSE = 0x13, // PAUSE 键
    VK_CAPITAL = 0x14, // CAPS LOCK 键
    VK_KANA = 0x15, // IME Kana 模式
    VK_HANGUL = 0x15, // IME Hanguel 模式
    VK_IME_ON = 0x16, // IME 打开
    VK_JUNJA = 0x17, // IME Junja 模式
    VK_FINAL = 0x18, // IME 最终模式
    VK_HANJA = 0x19, // IME Hanja 模式
    VK_KANJI = 0x19, // IME Kanji 模式
    VK_IME_OFF = 0x1A, // IME 关闭
    VK_ESCAPE = 0x1B, // ESC 键
    VK_CONVERT = 0x1C, // IME 转换
    VK_NONCONVERT = 0x1D, // IME 不转换
    VK_ACCEPT = 0x1E, // IME 接受
    VK_MODECHANGE = 0x1F, // IME 模式更改请求
    VK_SPACE = 0x20, // 空格键
    VK_PRIOR = 0x21, // PAGE UP 键
    VK_NEXT = 0x22, // PAGE DOWN 键
    VK_END = 0x23, // END 键
    VK_HOME = 0x24, // HOME 键
    VK_LEFT = 0x25, // LEFT ARROW 键
    VK_UP = 0x26, // UP ARROW 键
    VK_RIGHT = 0x27, // RIGHT ARROW 键
    VK_DOWN = 0x28, // DOWN ARROW 键
    VK_SELECT = 0x29, // SELECT 键
    VK_PRINT = 0x2A, // PRINT 键
    VK_EXECUTE = 0x2B, // EXECUTE 键
    VK_SNAPSHOT = 0x2C, // PRINT SCREEN 键
    VK_INSERT = 0x2D, // INS 键
    VK_DELETE = 0x2E, // DEL 键
    VK_HELP = 0x2F, // HELP 键

    VK_A = 0x41, // A 键
    VK_B = 0x42, // B 键
    VK_C = 0x43, // C 键
    VK_D = 0x44, // D 键
    VK_E = 0x45, // E 键
    VK_F = 0x46, // F 键
    VK_G = 0x47, // G 键
    VK_H = 0x48, // H 键
    VK_I = 0x49, // I 键
    VK_J = 0x4A, // J 键
    VK_K = 0x4B, // K 键
    VK_L = 0x4C, // L 键
    VK_M = 0x4D, // M 键
    VK_N = 0x4E, // N 键
    VK_O = 0x4F, // O 键
    VK_P = 0x50, // P 键
    VK_Q = 0x51, // Q 键
    VK_R = 0x52, // R 键
    VK_S = 0x53, // S 键
    VK_T = 0x54, // T 键
    VK_U = 0x55, // U 键
    VK_V = 0x56, // V 键
    VK_W = 0x57, // W 键
    VK_X = 0x58, // X 键
    VK_Y = 0x59, // Y 键
    VK_Z = 0x5A, // Z 键

    // 0x92-0x96: OEM 特有
    // 0x97-0x9F: 未分配
    VK_LSHIFT = 0xA0, // 左 SHIFT 键
    VK_RSHIFT = 0xA1, // 右 SHIFT 键
    VK_LCONTROL = 0xA2, // 左 Ctrl 键
    VK_RCONTROL = 0xA3, // 右 Ctrl 键
}