namespace Translate.Services.Dto;

public class MicrosoftTranslateDto
{
    public DetectedLanguage detectedLanguage { get; set; }
    
    public Translations[] translations { get; set; }
}

public class DetectedLanguage
{
    /// <summary>
    /// 语种
    /// </summary>
    public string language { get; set; }
    public double score { get; set; }
}

public class Translations
{
    /// <summary>
    /// 翻译内容
    /// </summary>
    public string text { get; set; }
    
    /// <summary>
    /// 翻译目标语种
    /// </summary>
    public string to { get; set; }
}