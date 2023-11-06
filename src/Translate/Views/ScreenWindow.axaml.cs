using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Token.Translate.Options;
using Token.Translate.Services;
using Token.Translate.ViewModels;
using Translate;
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

        TextStackPanel.Children.Clear();
    }

    private async void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {

        TextStackPanel.Children.Add(new ProgressBar()
        {
            Classes = { new string("circular") },
            IsIndeterminate = true,
            IsVisible = true,
        });

        start = false;

        var startX = Border.Margin.Left;
        var startY = Border.Margin.Top;
        var endX = Border.Width;
        var endY = Border.Height;

        using var bit = new Bitmap(new MemoryStream(ViewModel.Bytes));

        var options = TranslateContext.GetService<SystemOptions>();
        var translateService =
            TranslateContext.GetKeyedService<ITranslateService>(options.LanguageService);

        await Task.Run(async () =>
        {

            try
            {

                // 创建一个新的Bitmap对象来存储裁剪后的图像
                using Bitmap croppedImage = bit.Clone(new Rectangle((int)startX, (int)startY, (int)endX, (int)endY),
                    bit.PixelFormat);

                using var stream = new MemoryStream();
                croppedImage.Save(stream, ImageFormat.Png);

                using PaddleOcrAll all = new(LocalFullModels.ChineseV4, PaddleDevice.Mkldnn())
                {
                    AllowRotateDetection = true, /* 允许识别有角度的文字 */
                    Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
                };

                using Mat src = Cv2.ImDecode(stream.ToArray(), ImreadModes.Color);
                PaddleOcrResult result = all.Run(src);
                var data = new List<string>();
                foreach (var region in result.Regions)
                {
                    var value = await translateService.ExecuteAsync(region.Text);
                    data.Add(value.Result);
                }

                Dispatcher.UIThread.Invoke(() =>
                {
                    TextStackPanel.Children.Clear();
                    foreach (var region in data)
                    {
                        TextStackPanel.Children.Add(new TextBlock()
                        {
                            Text = region,
                        });
                    }
                });

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        });
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