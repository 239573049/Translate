namespace Translate.Models;

public class TranslateDto
{
    /// <summary>
    /// 内容
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// 内容语种
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// 翻译结果语种
    /// </summary>
    public string TargetLanguage { get; set; }

    /// <summary>
    /// 翻译结果
    /// </summary>
    public string Result { get; set; }
}