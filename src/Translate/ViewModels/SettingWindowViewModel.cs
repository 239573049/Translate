using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using Token.Translate.Models;
using Token.Translate.Services.Dto;
using Translate.Models;
using Translate.Services.Dto;

namespace Token.Translate.ViewModels;

public class SettingWindowViewModel : ViewModelBase
{
    public ObservableCollection<KeyboardHookDto> KeyList
    {
        get => _keyList;
        set => this.RaiseAndSetIfChanged(ref _keyList,value);
    }

    public string YoudaoKey
    {
        get => _youdaoKey;
        set => this.RaiseAndSetIfChanged(ref _youdaoKey, value);
    }

    public string YoudaoAppSecret
    {
        get => _youdaoAppSecret;
        set => this.RaiseAndSetIfChanged(ref _youdaoAppSecret, value);
    }

    private ObservableCollection<SelectSettingTranslateDto> _selectSettingTranslate = new();

    public ObservableCollection<SelectSettingTranslateDto> SelectSettingTranslate
    {
        get => _selectSettingTranslate;
        set => this.RaiseAndSetIfChanged(ref _selectSettingTranslate, value);
    }

    private SelectSettingTranslateDto _selectSetting;

    public SelectSettingTranslateDto SelectSetting
    {
        get => _selectSetting;
        set => this.RaiseAndSetIfChanged(ref _selectSetting, value);
    }


    /// <summary>
    /// Microsoft翻译的端点
    /// </summary>
    private string _microsoftEndpoint;

    /// <summary>
    /// Microsoft的Key
    /// </summary>
    private string _microsoftKey;

    /// <summary>
    /// Microsoft的location
    /// </summary>
    private string _microsoftLocation;

    /// <summary>
    /// 目标语种
    /// </summary>
    public LanguageDto? _targetLanguage;

    /// <summary>
    /// 内容语种
    /// </summary>
    public LanguageDto? _language;

    private bool _automaticDetection;


    /// <summary>
    /// 有道key
    /// </summary>
    private string _youdaoKey;

    /// <summary>
    /// 有道Id
    /// </summary>
    private string _youdaoAppSecret;

    public bool AutomaticDetection
    {
        get => _automaticDetection;
        set => this.RaiseAndSetIfChanged(ref _automaticDetection, value);
    }

    /// <summary>
    /// 目标语种
    /// </summary>
    public LanguageDto? TargetLanguage
    {
        get => _targetLanguage;
        set => this.RaiseAndSetIfChanged(ref _targetLanguage, value);
    }

    /// <summary>
    /// 内容语种
    /// </summary>
    public LanguageDto? Language
    {
        get => _language;
        set => this.RaiseAndSetIfChanged(ref _language, value);
    }

    /// <summary>
    /// Microsoft翻译的端点
    /// </summary>
    public string MicrosoftEndpoint
    {
        get => _microsoftEndpoint;
        set => this.RaiseAndSetIfChanged(ref _microsoftEndpoint, value);
    }

    /// <summary>
    /// Microsoft的Key
    /// </summary>
    public string MicrosoftKey
    {
        get => _microsoftKey;
        set => this.RaiseAndSetIfChanged(ref _microsoftKey, value);
    }

    /// <summary>
    /// Microsoft的location
    /// </summary>
    public string MicrosoftLocation
    {
        get => _microsoftLocation;
        set => this.RaiseAndSetIfChanged(ref _microsoftLocation, value);
    }

    private List<LanguageDto> _languages = new();

    public List<LanguageDto> Languages
    {
        get => _languages;
        set => this.RaiseAndSetIfChanged(ref _languages, value);
    }

    public string AiKey
    {
        get => _aiKey;
        set => this.RaiseAndSetIfChanged(ref _aiKey, value);
    }

    public string AiEndpoint
    {
        get => _aiEndpoint;
        set => this.RaiseAndSetIfChanged(ref _aiEndpoint, value);
    }

    /// <summary>
    /// Ai模型
    /// </summary>
    public string AiModel
    {
        get => _aiModel;
        set => this.RaiseAndSetIfChanged(ref _aiModel, value);
    }

    /// <summary>
    /// 首页呼出快捷键
    /// </summary>
    public KeyboardHookDto HomeKey
    {
        get => _homeKey;
        set => this.RaiseAndSetIfChanged(ref _homeKey,value);
    }

    /// <summary>
    /// Ai Key
    /// </summary>
    private string _aiKey;

    /// <summary>
    /// Ai的端点
    /// </summary>
    private string _aiEndpoint;

    /// <summary>
    /// Ai模型
    /// </summary>
    private string _aiModel;

    /// <summary>
    /// 首页呼出快捷键
    /// </summary>
    private KeyboardHookDto _homeKey;

    private ObservableCollection<KeyboardHookDto> _keyList = new();
    
    
}