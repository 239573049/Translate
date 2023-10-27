namespace Translate.Services.Dto;

public class YoudaoTranslateDto
{
    public string tSpeakUrl { get; set; }
    public Web[] web { get; set; }
    public string requestId { get; set; }
    public string query { get; set; }
    public string[] translation { get; set; }
    public MTerminalDict mTerminalDict { get; set; }
    public string errorCode { get; set; }
    public Dict dict { get; set; }
    public Webdict webdict { get; set; }
    public string l { get; set; }
    public bool isWord { get; set; }
    public string speakUrl { get; set; }
}

public class Web
{
    public string[] value { get; set; }
    public string key { get; set; }
}

public class MTerminalDict
{
    public string url { get; set; }
}

public class Dict
{
    public string url { get; set; }
}

public class Webdict
{
    public string url { get; set; }
}