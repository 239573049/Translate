using ReactiveUI;
using Translate.Models;

namespace Token.Translate.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool editIsVisible = false;

    public bool EditIsVisible
    {
        get => editIsVisible;
        set => this.RaiseAndSetIfChanged(ref editIsVisible, value);
    }

    private int width = 50;

    public int Width
    {
        get => width;
        set => this.RaiseAndSetIfChanged(ref width, value);
    }

    private bool _flyoutVisible = false;

    public bool FlyoutVisible
    {
        get => _flyoutVisible;
        set => this.RaiseAndSetIfChanged(ref _flyoutVisible, value);
    }

    private TranslateDto _translateDto;

    public TranslateDto TranslateDto
    {
        get => _translateDto;
        set => this.RaiseAndSetIfChanged(ref _translateDto, value);
    }
}