namespace Token.Translate.Services.Dto;

public class KeyboardHookDto
{
    /// <summary>
    /// 按键
    /// </summary>
    public VirtualKeyCodes KeyCodes { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}