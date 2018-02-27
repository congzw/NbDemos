using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageDemo
{
    public class ScreenHelper
    {
        public string AutoCreateFileName()
        {
            return "Screen_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_ff") + ".png";
        }

        public void CaptureScreenToFile(string fileName)
        {
            using (var bitmap = CaptureScreen())
            {
                bitmap.Save(fileName);
            }
        }

        public Bitmap CaptureScreen()
        {
            var image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            var gfx = Graphics.FromImage(image);
            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return image;
        }
    }
}