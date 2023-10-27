using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Token.Translate.Helper;
using Token.Translate.Models;
using Token.Translate.Options;
using Token.Translate.ViewModels;
using Translate.Models;
using Translate.Services;

namespace Translate.Views;

public partial class MainWindow : Window, IDisposable
{
    private readonly CancellationTokenSource _cancellationToken = new();

    private WindowNotificationManager? _manager;

    private readonly IKeyboardHookService? _keyboardHookService;

    public MainWindow()
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

        ShowInTaskbar = false;

        Task.Run(async () =>
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000);
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        if (IsVisible)
                            Topmost = true;
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }, _cancellationToken.Token);

        WindowState = WindowState.Maximized;

        DragControlHelper.StartDrag(TranslateLogo);

        this.GetObservable(WindowStateProperty).Subscribe(WindowStatePropertyChanged);

        if (MainBorder.ContextFlyout != null)
        {
            MainBorder.ContextFlyout.Closed += (o, args) =>
            {
                ViewModel.FlyoutVisible = false;

                ViewModel.EditIsVisible = false;

                MainBorder.ContextFlyout?.Hide();
                ViewModel.Width = 50;
            };
        }
    }


    private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    private void WindowStatePropertyChanged(WindowState state)
    {
        if (state == WindowState.Maximized)
        {
            BottomRight();
        }
    }

    public override void Show()
    {
        base.Show();
        BottomRight();
        _manager = new WindowNotificationManager(this) { MaxItems = 3 };
    }

    private void BottomRight()
    {
        var screenBounds = Screens.All[0].Bounds;
        Position = new PixelPoint((int)(screenBounds.Width - Width - 30),
            (int)(screenBounds.Height - 200 - Height));
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        BottomRight();
    }

    protected override void OnClosed(EventArgs e)
    {
        DragControlHelper.StopDrag(TranslateLogo);
        _cancellationToken.Cancel();
    }

    private void MainBorder_OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (ViewModel.FlyoutVisible)
        {
            return;
        }

        ViewModel.EditIsVisible = false;

        MainBorder.ContextFlyout?.Hide();
        ViewModel.Width = 50;
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        ViewModel.Width = 100;
        ViewModel.EditIsVisible = true;
        TextBox.Focus();
    }

    private async void TextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e is { Key: Key.Enter, KeyModifiers: KeyModifiers.None })
        {
            MainBorder.ContextFlyout?.ShowAt(MainBorder);
            ViewModel.FlyoutVisible = true;


            if (!string.IsNullOrWhiteSpace(TextBox.Text))
            {
                try
                {
                    var options = TranslateContext.GetService<SystemOptions>();

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
                            _manager.Show(new Notification("错误", "Microsoft Key并没有配置，请打开设置配置！",
                                NotificationType.Error));
                            return;
                        }

                        if (string.IsNullOrWhiteSpace(options.MicrosoftLocation))
                        {
                            _manager.Show(new Notification("错误", "Microsoft Location并没有配置，请打开设置配置！",
                                NotificationType.Error));
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

                    var languages = TranslateContext.GetRequiredService<List<LanguageDto>>();

                    var translateService =
                        TranslateContext.GetKeyedService<ITranslateService>(options.LanguageService);
                    var result = await translateService.ExecuteAsync(TextBox.Text);
                    result.Language = languages
                        .FirstOrDefault(x => x.value == result.Language)?.label ?? string.Empty;
                    result.TargetLanguage = languages
                        .FirstOrDefault(x => x.value == result.TargetLanguage)?.label ?? string.Empty;

                    ViewModel.TranslateDto = new TranslateDto()
                    {
                        TargetLanguage = result.TargetLanguage,
                        Language = result.Language,
                        Value = result.Value,
                        Result = result.Result
                    };

                    ResultTextBox.Focus();

                    ResultTextBox.SelectAll();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }

    public void Dispose()
    {
        _keyboardHookService?.Dispose();
    }
}