using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Token.Translate.Models;
using Token.Translate.Options;
using Token.Translate.Services;
using Token.Translate.Services.Dto;
using Token.Translate.ViewModels;
using Translate;
using Translate.Services.Dto;

namespace Token.Translate.Views;

public partial class SettingWindow : Window
{
    private WindowNotificationManager? _manager;

    public SettingWindow()
    {
        InitializeComponent();

        DataContextChanged += (sender, args) =>
        {
            ViewModel.SelectSettingTranslate.Clear();
            ViewModel.SelectSettingTranslate.Add(new SelectSettingTranslateDto()
            {
                Title = "微软翻译",
                Value = Constant.MicrosoftLanguage
            });
            ViewModel.SelectSettingTranslate.Add(new SelectSettingTranslateDto()
            {
                Title = "有道翻译",
                Value = Constant.YouDaoLanguage
            });

            ViewModel.SelectSettingTranslate.Add(new SelectSettingTranslateDto()
            {
                Title = "AI翻译",
                Value = Constant.AILanguage
            });

            var options = TranslateContext.GetService<SystemOptions>();
            ViewModel.Languages = TranslateContext.GetRequiredService<List<LanguageDto>>();

            ViewModel.Language = ViewModel.Languages.FirstOrDefault(x => x.value == options.Language);
            ViewModel.TargetLanguage = ViewModel.Languages.FirstOrDefault(x => x.value == options.TargetLanguage);

            ViewModel.AiModel = options.AiModel;
            ViewModel.AiEndpoint = options.AiEndpoint;
            ViewModel.AiKey = options.AiKey;
            ViewModel.MicrosoftLocation = options.MicrosoftLocation;
            ViewModel.AutomaticDetection = options.AutomaticDetection;
            ViewModel.MicrosoftKey = options.MicrosoftKey;
            ViewModel.MicrosoftEndpoint = options.MicrosoftEndpoint;
            ViewModel.YoudaoKey = options.YoudaoKey;
            ViewModel.UseProxy = options.UseProxy;
            ViewModel.ProxyServer = options.ProxyServer;
            ViewModel.ProxyUsername = options.ProxyUsername;
            ViewModel.TranslationChineseAndEnglish = options.TranslationChineseAndEnglish;
            ViewModel.ProxyPassword = options.ProxyPassword;
            ViewModel.YoudaoAppSecret = options.YoudaoAppSecret;
            ViewModel.SelectSetting = ViewModel.SelectSettingTranslate.First(x =>
                x.Value == options.LanguageService);

            foreach (var key in (VirtualKeyCodes[])Enum.GetValues(typeof(VirtualKeyCodes)))
            {
                ViewModel.KeyList.Add(new KeyboardHookDto()
                {
                    KeyCodes = key
                });
            }

            ViewModel.HomeKey = ViewModel.KeyList.First(x => x.KeyCodes == options.HomeKey);
        };
    }

    private SettingWindowViewModel ViewModel => (DataContext as SettingWindowViewModel)!;

    public override void Show()
    {
        base.Show();

        _manager = new WindowNotificationManager(this) { MaxItems = 3 };
    }

    private void Save_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var options = TranslateContext.GetService<SystemOptions>();

            options.LanguageService = ViewModel.SelectSetting.Value;
            options.MicrosoftLocation = ViewModel.MicrosoftLocation;
            options.MicrosoftKey = ViewModel.MicrosoftKey;
            options.MicrosoftEndpoint = ViewModel.MicrosoftEndpoint;
            options.AutomaticDetection = ViewModel.AutomaticDetection;
            options.TargetLanguage = ViewModel.TargetLanguage?.value;
            options.Language = ViewModel.Language?.value;
            options.AiModel = ViewModel.AiModel;
            options.AiEndpoint = ViewModel.AiEndpoint;
            options.AiKey = ViewModel.AiKey;
            options.YoudaoKey = ViewModel.YoudaoKey;
            options.YoudaoAppSecret = ViewModel.YoudaoAppSecret;
            options.HomeKey = ViewModel.HomeKey.KeyCodes;
            options.UseProxy = ViewModel.UseProxy;
            options.ProxyServer = ViewModel.ProxyServer;
            options.ProxyUsername = ViewModel.ProxyUsername;
            options.ProxyPassword = ViewModel.ProxyPassword;
            options.TranslationChineseAndEnglish = ViewModel.TranslationChineseAndEnglish;

            using var stream = File.CreateText("./" + Constant.SettingDb);
            stream.WriteLine(JsonSerializer.Serialize(options));
            _manager!.Show(new Notification("成功", "保存配置成功", NotificationType.Success));
        }
        catch (Exception exception)
        {
            _manager!.Show(new Notification("失败", exception.Message, NotificationType.Error));
        }
    }

    private StackPanel _stackPanel;

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { SelectedItem: SelectSettingTranslateDto selectedItem })
        {
            if (_stackPanel != null)
            {
                _stackPanel.IsVisible = false;
            }

            // 规范Stack组件名称
            _stackPanel = this.FindControl<StackPanel>("StackPanel" + selectedItem.Value);

            if (_stackPanel != null)
            {
                _stackPanel.IsVisible = true;
            }
        }
    }
}