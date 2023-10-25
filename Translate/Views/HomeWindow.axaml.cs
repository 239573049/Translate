using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
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

        await ExecuteAsync();
    }

    private async Task ExecuteAsync()
    {
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
}