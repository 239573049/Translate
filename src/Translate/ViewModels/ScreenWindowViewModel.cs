using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Token.Translate.ViewModels;

public class ScreenWindowViewModel : ViewModelBase
{
    public byte[] Bytes;

    private Bitmap _source;

    public Bitmap Source
    {
        get => _source;
        set => this.RaiseAndSetIfChanged(ref _source, value);
    }

    private int _x;

    public int X
    {
        get => _x;
        set => this.RaiseAndSetIfChanged(ref _x, value);
    }

    public int _y;

    public int Y
    {
        get => _y;
        set => this.RaiseAndSetIfChanged(ref _y, value);
    }

}