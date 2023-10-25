using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Translate.Models;
using Translate.Options;
using Translate.Services;
using Translate.ViewModels;

namespace Translate;

public partial class HomeWindow : Window
{
    public HomeWindow()
    {
        InitializeComponent();
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

        ViewModel.IsLoading = true;
        
        var options = TranslateContext.GetService<SystemOptions>();

        var languages = TranslateContext.GetRequiredService<List<LanguageDto>>();

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
    }
}