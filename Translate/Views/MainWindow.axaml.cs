using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Translate.Helper;
using Translate.Models;
using Translate.Options;
using Translate.Services;
using Translate.ViewModels;
using HotKeyManager = Translate.Helper.HotKeyManager;

namespace Translate.Views;

public partial class MainWindow : Window, IDisposable
{
    private readonly CancellationTokenSource _cancellationToken = new();
    private HotKeyManager hotKeyManager;

    public MainWindow()
    {
        InitializeComponent();
        // hotKeyManager = new HotKeyManager();
        //
        // // 注册快捷键 CTRL+ALT+F
        // hotKeyManager.RegisterHotKey(1, 0x11 | 0x12, 0x46);
        //
        // // 添加快捷键按下的处理器
        // hotKeyManager.HotKeyPressed += (s, e) =>
        // {
        //     // 在这里添加你的代码
        //     Console.WriteLine("Hotkey pressed!");
        // };

        // 在你的应用程序结束

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

        // ShowInTaskbar = false;

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
        // 在你的应用程序结束时，记得注销快捷键
        // hotKeyManager.UnregisterHotKey(1);
    }
}