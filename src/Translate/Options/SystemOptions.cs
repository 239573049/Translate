using Token.Translate.Services;
using Translate;
using Translate.Services;

namespace Token.Translate.Options;

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

    /// <summary>
    /// Ai Key
    /// </summary>
    public string AiKey { get; set; }

    /// <summary>
    /// Ai的端点
    /// </summary>
    public string AiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

    /// <summary>
    /// Ai模型
    /// </summary>
    public string AiModel { get; set; } = "gpt-3.5-turbo";

    /// <summary>
    /// 有道key
    /// </summary>
    public string YoudaoKey { get; set; }

    /// <summary>
    /// 有道Id
    /// </summary>
    public string YoudaoAppSecret { get; set; }

    /// <summary>
    /// 首页快捷键
    /// </summary>
    public VirtualKeyCodes HomeKey { get; set; } = VirtualKeyCodes.VK_E;

    /// <summary>
    /// 是否使用代理
    /// </summary>
    public bool UseProxy { get; set; }

    /// <summary>
    /// 代理服务
    /// </summary>
    public string ProxyServer { get; set; }

    public string ProxyUsername { get; set; }

    public string ProxyPassword { get; set; }

    /// <summary>
    /// 中英互相翻译
    /// </summary>
    public bool TranslationChineseAndEnglish { get; set; }
}
