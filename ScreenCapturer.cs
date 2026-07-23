using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ScreenshotTool
{
    internal sealed class ScreenCapturer
    {
        private static readonly string OutputDir =
            Path.Combine(AppContext.BaseDirectory, "Screenshots");

        public string? CaptureActiveScreen()
        {
            try
            {
                Screen screen = Screen.FromPoint(Cursor.Position);
                Rectangle bounds = screen.Bounds;

                using var bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                }

                Directory.CreateDirectory(OutputDir);
                string file = Path.Combine(OutputDir,
                    $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                bmp.Save(file, ImageFormat.Png);
                Clipboard.SetImage(bmp);
                return file;
            }
            catch
            {
                return null;
            }
        }
    }
}