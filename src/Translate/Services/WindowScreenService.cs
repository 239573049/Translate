using Avalonia.Platform;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Token.Translate.Services;

public class WindowScreenService
{
    private static Screen? _screen;

    public static void Init(Screen screen)
    {
        _screen = screen;
    }

    public static byte[] GetScreen()
    {
        // 创建一个与屏幕大小相同的Bitmap对象
        Bitmap screenshot = new(_screen.Bounds.Width, _screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        // 创建一个用于从屏幕读取图像的Graphics对象
        using (Graphics graphics = Graphics.FromImage(screenshot))
        {
            // 将屏幕图像复制到Bitmap对象中
            graphics.CopyFromScreen(_screen.Bounds.X, _screen.Bounds.Y, 0, 0, new Size(_screen.Bounds.Width, _screen.Bounds.Height), CopyPixelOperation.SourceCopy);
        }

        // 将Bitmap对象转换为字节数组
        using (MemoryStream stream = new MemoryStream())
        {
            screenshot.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}