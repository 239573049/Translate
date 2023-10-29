using ReactiveUI;
using Translate.Models;

namespace Token.Translate.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
    private string _message = string.Empty;

    public string Message
    {
        get => _message;
        set
        {
            IsQuery = !string.IsNullOrWhiteSpace(value);
            this.RaiseAndSetIfChanged(ref _message, value);
        }
    }

    private bool _isQuery;

    public bool IsQuery
    {
        get => _isQuery;
        set => this.RaiseAndSetIfChanged(ref _isQuery, value);
    }

    public bool IsVisibleQueryResult
    {
        get => _isVisibleQueryResult;
        set => this.RaiseAndSetIfChanged(ref _isVisibleQueryResult, value);
    }

    private bool _isVisibleQueryResult;

    private TranslateDto _translate;

    public TranslateDto Translate
    {
        get => _translate;
        set => this.RaiseAndSetIfChanged(ref _translate, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private bool _isLoading;

}