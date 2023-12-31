using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Token.Translate.Models;
using Token.Translate.Options;
using Token.Translate.Services;
using Token.Translate.ViewModels;
using Translate;
using Translate.Services;
using HomeWindow = Token.Translate.Views.HomeWindow;
using JsonSerializer = System.Text.Json.JsonSerializer;
using SettingWindow = Token.Translate.Views.SettingWindow;

namespace Token.Translate;

public partial class App : Application
{
    public override void Initialize()
    {
        var context = TranslateContext.CreateContext();

        context.AddKeyedSingleton<ITranslateService, MicrosoftTranslateService>(Constant.MicrosoftLanguage);
        context.AddKeyedSingleton<ITranslateService, AiTranslateService>(Constant.AILanguage);
        context.AddKeyedSingleton<ITranslateService, YoudaoTranslateService>(Constant.YouDaoLanguage);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            context.AddSingleton<IKeyboardHookService, WindowKeyboardHookService>();
        }

        context.AddHttpClient();

        context.AddSingleton<HomeWindow>((_) => new HomeWindow()
        {
            DataContext = new HomeWindowViewModel()
        });

        context.AddSingleton<SystemOptions>((_) =>
        {
            if (!File.Exists("./" + Constant.SettingDb)) return new SystemOptions();

            try
            {
                return JsonSerializer.Deserialize<SystemOptions>(File.ReadAllText("./" + Constant.SettingDb),
                    new JsonSerializerOptions()
                    {
                        Converters =
                        {
                            new JsonStringEnumConverter(),
                        },
                        ReadCommentHandling = JsonCommentHandling.Skip
                    }) ?? new SystemOptions();
            }
            catch
            {
                return new SystemOptions();
            }
        });

        context.AddSingleton<List<LanguageDto>>(services =>
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Token.Translate.language.json");

            using var read = new StreamReader(stream);
            return JsonSerializer.Deserialize<List<LanguageDto>>(read.ReadToEnd());
        });

        context.Builder();

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var home = TranslateContext.GetService<HomeWindow>();
            desktop.MainWindow = home;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void Exit_OnClick(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void OpenSetting_OnClick(object? sender, EventArgs e)
    {
        var setting = new SettingWindow()
        {
            DataContext = new SettingWindowViewModel(),
        };
        setting.Show();
    }

    private void OpenHome_OnClick(object? sender, EventArgs e)
    {
        var home = TranslateContext.GetService<HomeWindow>();
        home.Show();
    }

    private void TrayIcon_OnClicked(object? sender, EventArgs e)
    {
        var home = TranslateContext.GetService<HomeWindow>();
        home.Show();
    }

    private void NativeMenuItem_OnClick(object? sender, EventArgs e)
    {
        var bytes = WindowScreenService.GetScreen();

        var screen = new Views.ScreenWindow()
        {
            DataContext = new ScreenWindowViewModel()
            {
                Bytes = bytes,
                Source = new Bitmap(new MemoryStream(bytes)),
            }
        };

        screen.Show();
    }
}