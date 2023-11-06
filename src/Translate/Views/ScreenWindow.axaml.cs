using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Token.Translate.ViewModels;
using Point = Avalonia.Point;

namespace Token.Translate.Views;

public partial class ScreenWindow : Window
{
    private Point _startPoint;

    private bool start;
    public ScreenWindow()
    {
        InitializeComponent();

        this.WindowState = WindowState.FullScreen;

        KeyDown += MainWindow_KeyDown;
    }

    private ScreenWindowViewModel ViewModel => DataContext as ScreenWindowViewModel;

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Image_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _startPoint = e.GetPosition(sender as Visual);
        Border.IsVisible = true;
        Border.Width = 0;
        Border.Height = 0;
        Border.Margin = new Thickness(_startPoint.X, _startPoint.Y, 0, 0);
        start = true;
        Debug.WriteLine(_startPoint.X + " " + _startPoint.Y);
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        start = false;

        var startX = Border.Margin.Left;
        var startY = Border.Margin.Top;
        var endX = Border.Width;
        var endY = Border.Height;

        var bit = new Bitmap(new MemoryStream(ViewModel.Bytes));
        // 创建一个新的Bitmap对象来存储裁剪后的图像
        //Bitmap croppedImage = bit.Clone(new Rectangle((int)startX, (int)startY, (int)endX, (int)endY), bit.PixelFormat);

        //using var stream = new MemoryStream();
        //croppedImage.Save(stream,ImageFormat.Png);
        //ViewModel.Source = new Avalonia.Media.Imaging.Bitmap(new MemoryStream(stream.ToArray()));
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (start)
        {
            Point position = e.GetPosition(sender as Visual);
            var width = position.X - _startPoint.X;
            var height = position.Y - _startPoint.Y;

            Border.Width = width;
            Border.Height = height;
        }
    }
}