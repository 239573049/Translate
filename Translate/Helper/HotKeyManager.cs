
using System;
using System.Runtime.InteropServices;

namespace Translate.Helper;

public class HotKeyManager
{
    public delegate void HotKeyPressedHandler(object sender, HotKeyEventArgs e);
    public event HotKeyPressedHandler HotKeyPressed;

    private const int WM_HOTKEY = 0x0312;

    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
    
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
    
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    //使用WINDOWS API函数代替获取当前实例的函数,防止钩子失效
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string name);

    
    public void RegisterHotKey(int id, uint modifiers, uint key)
    {
        if (!RegisterHotKey(GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), id, modifiers, key))
        {
            throw new InvalidOperationException("Couldn't register the hotkey.");
        }
    }

    public void UnregisterHotKey(int id)
    {
        if (!UnregisterHotKey(IntPtr.Zero, id))
        {
            throw new InvalidOperationException("Couldn't unregister the hotkey.");
        }
    }

    public void ProcessHotKeyMessage(IntPtr wParam, IntPtr lParam)
    {
        if (HotKeyPressed != null)
        {
            HotKeyPressed(this, new HotKeyEventArgs(wParam, lParam));
        }
    }
}

public class HotKeyEventArgs : EventArgs
{
    public IntPtr WParam { get; private set; }
    public IntPtr LParam { get; private set; }

    public HotKeyEventArgs(IntPtr wParam, IntPtr lParam)
    {
        WParam = wParam;
        LParam = lParam;
    }
}