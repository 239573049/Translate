using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Token.Translate.Models;
using Token.Translate.Options;
using Token.Translate.Services;
using Token.Translate.ViewModels;
using Translate.Models;
using Translate.Services;

namespace Translate;

public partial class HomeWindow : Window, IDisposable
{
    private WindowNotificationManager? _manager;

    private readonly IKeyboardHookService? _keyboardHookService;

    public HomeWindow()
    {
        InitializeComponent();

        _keyboardHookService = TranslateContext.GetService<IKeyboardHookService>();
        _keyboardHookService?.Start();

        if (_keyboardHookService != null)
            _keyboardHookService.KeyDownEvent += (key) =>
            {
                if (KeyboardUtilities.IsCtrlAltPressed())
                {
                    var option = TranslateContext.GetService<SystemOptions>();
                    if (option.HomeKey == key)
                    {
                        var home = TranslateContext.GetService<HomeWindow>();
                        if (!home.IsVisible)
                        {
                            home.Show();
                        }
                        else
                        {
                            home.Hide();
                        }
                    }
                }
            };

        this.Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public override void Show()
    {
        base.Show();

        Topmost = true;

        TextBoxMessage.Focus();

        _manager = new WindowNotificationManager(this) { MaxItems = 3 };
    }

    private HomeWindowViewModel ViewModel => DataContext as HomeWindowViewModel;

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        TextBoxMessage.Focus();
    }

    private void Clear_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Message = string.Empty;
        ViewModel.IsVisibleQueryResult = false;
        ViewModel.Translate = null;
    }

    private async void Query_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.Message))
        {
            return;
        }

        await ExecuteAsync();
    }

    private async Task ExecuteAsync()
    {
        ViewModel.Translate = null;
        ViewModel.IsLoading = true;

        var options = TranslateContext.GetService<SystemOptions>();

        var languages = TranslateContext.GetRequiredService<List<LanguageDto>>();

        if (options.LanguageService == Constant.AILanguage)
        {
            if (string.IsNullOrWhiteSpace(options.AiEndpoint))
            {
                _manager.Show(new Notification("错误", "AI端点并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }

            if (string.IsNullOrWhiteSpace(options.AiKey))
            {
                _manager.Show(new Notification("错误", "AIKey并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }

            if (string.IsNullOrWhiteSpace(options.AiModel))
            {
                _manager.Show(new Notification("错误", "AI模型并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }
        }
        else if (options.LanguageService == Constant.MicrosoftLanguage)
        {
            if (string.IsNullOrWhiteSpace(options.MicrosoftEndpoint))
            {
                _manager.Show(new Notification("错误", "Microsoft端点并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }

            if (string.IsNullOrWhiteSpace(options.MicrosoftKey))
            {
                _manager.Show(new Notification("错误", "Microsoft Key并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }

            if (string.IsNullOrWhiteSpace(options.MicrosoftLocation))
            {
                _manager.Show(new Notification("错误", "Microsoft Location并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }
        }
        else if (options.LanguageService == Constant.YouDaoLanguage)
        {
            if (string.IsNullOrWhiteSpace(options.YoudaoKey))
            {
                _manager.Show(new Notification("错误", "有道Key并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }

            if (string.IsNullOrWhiteSpace(options.YoudaoAppSecret))
            {
                _manager.Show(new Notification("错误", "有道AppSecret并没有配置，请打开设置配置！", NotificationType.Error));
                return;
            }
        }


        var translateService =
            TranslateContext.GetKeyedService<ITranslateService>(options.LanguageService);
        var result = await translateService.ExecuteAsync(ViewModel.Message);
        result.Language = languages
            .FirstOrDefault(x => x.value == result.Language)?.label ?? string.Empty;
        result.TargetLanguage = languages
            .FirstOrDefault(x => x.value == result.TargetLanguage)?.label ?? string.Empty;

        ViewModel.IsVisibleQueryResult = true;
        ViewModel.Translate = new TranslateDto()
        {
            TargetLanguage = result.TargetLanguage,
            Language = result.Language,
            Value = result.Value,
            Result = result.Result
        };

        ViewModel.IsLoading = false;

        // 方便直接复制翻译的结果
        ResultTextBox.Focus();
        ResultTextBox.SelectAll();

        // 自动Copy?
        // ResultTextBox.Copy();
    }


    private async void TextBoxMessage_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.None })
        {
            await ExecuteAsync();
        }
        else if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    public void Dispose()
    {
        _keyboardHookService?.Dispose();
    }
}