namespace Translate.Options;

public class SystemOptions
{
    /// <summary>
    /// Microsoft翻译的端点
    /// </summary>
    public string MicrosoftEndpoint { get; set; }

    /// <summary>
    /// Microsoft的Key
    /// </summary>
    public string MicrosoftKey { get; set; }

    /// <summary>
    /// Microsoft的location
    /// </summary>
    public string MicrosoftLocation { get; set; }

    /// <summary>
    /// 目标语种
    /// </summary>
    public string TargetLanguage { get; set; }

    /// <summary>
    /// 内容语种
    /// </summary>
    public string Language { get; set; }
    
    /// <summary>
    /// 自动识别
    /// </summary>
    public bool AutomaticDetection { get; set; }

    /// <summary>
    /// 当前语种服务
    /// </summary>
    public string LanguageService { get; set; } = Constant.MicrosoftLanguage;
}