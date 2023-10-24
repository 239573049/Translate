using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Translate.Models;
using Translate.Options;
using Translate.Services;
using Translate.ViewModels;
using Translate.Views;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Translate;

public partial class App : Application
{
    public override void Initialize()
    {
        var context = TranslateContext.CreateContext();

        context.AddKeyedSingleton<ITranslateService, MicrosoftTranslateService>(Constant.MicrosoftLanguage);

        context.AddHttpClient();

        context.AddSingleton<SystemOptions>((_) =>
        {
            if (!File.Exists("./" + Constant.SettingDB)) return new SystemOptions();

            try
            {
                return JsonSerializer.Deserialize<SystemOptions>(File.ReadAllText("./" + Constant.SettingDB),
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
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Translate.language.json");

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
            // desktop.MainWindow = new MainWindow
            // {
            //     DataContext = new MainWindowViewModel(),
            // };
            //
            desktop.MainWindow=new SettingWindow()
            {
                DataContext = new SettingWindowViewModel(),
            };
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
}