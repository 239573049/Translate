namespace Translate.Models;

/// <summary>
/// ChatGPT响应模型
/// </summary>
public class ChatResponseDto
{
    public Choices[] choices { get; set; }

    public int created { get; set; }

    public string id { get; set; }

    public string model { get; set; }

    public Usage usage { get; set; }
}

public class Choices
{
    public string finish_reason { get; set; }
    public int index { get; set; }
    public Message message { get; set; }
}

public class Message
{
    public string content { get; set; }
    public string role { get; set; }
}

public class Usage
{
    /// <summary>
    /// 响应使用的token
    /// </summary>
    public int completion_tokens { get; set; }

    /// <summary>
    /// 请求使用的token
    /// </summary>
    public int prompt_tokens { get; set; }
    
    /// <summary>
    /// 使用的总token
    /// </summary>
    public int total_tokens { get; set; }
}