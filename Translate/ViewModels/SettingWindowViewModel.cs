﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using Translate.Models;
using Translate.Services.Dto;

namespace Translate.ViewModels;

public class SettingWindowViewModel : ViewModelBase
{
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
}