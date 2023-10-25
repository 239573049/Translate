using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Translate.Helper;
using Translate.Models;
using Translate.Options;
using Translate.Services.Dto;
using Translate.ViewModels;

namespace Translate.Views;

public partial class SettingWindow : Window
{
    private WindowNotificationManager? _manager;

    public SettingWindow()
    {
        InitializeComponent();

        DragControlHelper.StartDrag(TabControl);

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

            ViewModel.MicrosoftLocation = options.MicrosoftLocation;
            ViewModel.AutomaticDetection = options.AutomaticDetection;
            ViewModel.MicrosoftKey = options.MicrosoftKey;
            ViewModel.MicrosoftEndpoint = options.MicrosoftEndpoint;
            ViewModel.SelectSetting = ViewModel.SelectSettingTranslate.First(x =>
                x.Value == options.LanguageService);
        };
    }

    protected override void OnClosed(EventArgs e)
    {
        DragControlHelper.StopDrag(TabControl);
        base.OnClosed(e);
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

            using var stream = File.CreateText("./" + Constant.SettingDb);
            stream.WriteLine(JsonSerializer.Serialize(options));
            _manager!.Show(new Notification("成功", "保存配置成功", NotificationType.Success));
        }
        catch (Exception exception)
        {
            _manager!.Show(new Notification("失败", exception.Message, NotificationType.Error));
        }
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is SelectSettingTranslateDto selectedItem)
        {
            var stack = this.FindControl<StackPanel>("StackPanel" + selectedItem.Value);

            if (stack != null)
            {
                stack.IsVisible = true;
            }
        }
    }
}