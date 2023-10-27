using System;
using Token.Translate.Services;

namespace Translate.Services;

public interface IKeyboardHookService : IDisposable
{
    /// <summary>
    /// 按键按下事件
    /// </summary>
    Action<VirtualKeyCodes> KeyDownEvent { get; set; }

    /// <summary>
    /// 启动挂载
    /// </summary>
    void Start();
}