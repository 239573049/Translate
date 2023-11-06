using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using Token.Translate.ViewModels;
using Point = Avalonia.Point;
using Window = Avalonia.Controls.Window;

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

    private async void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        start = false;

        var startX = Border.Margin.Left;
        var startY = Border.Margin.Top;
        var endX = Border.Width;
        var endY = Border.Height;
        try
        {

            var bit = new Bitmap(new MemoryStream(ViewModel.Bytes));

            // 创建一个新的Bitmap对象来存储裁剪后的图像
            Bitmap croppedImage = bit.Clone(new Rectangle((int)startX, (int)startY, (int)endX, (int)endY), bit.PixelFormat);

            using var stream = new MemoryStream();
            croppedImage.Save(stream, ImageFormat.Png);

            FullOcrModel model = await OnlineFullModels.EnglishV3.DownloadAsync();


            using PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
            {
                AllowRotateDetection = false, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            };

            using Mat src = Cv2.ImDecode(stream.ToArray(), ImreadModes.Color);
            PaddleOcrResult result = all.Run(src);
            Console.WriteLine("Detected all texts: \n" + result.Text);
            foreach (PaddleOcrResultRegion region in result.Regions)
            {
                Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

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