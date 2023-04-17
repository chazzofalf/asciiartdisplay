using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using AsciiArtDisplay.Resources;
using System.IO;

namespace AsciiArtDisplay.Windows
{
    public class WindowsSetup
    {
        public static void Init()
        {
            AsciiArtDisplay.Resources.Resource.ConsoleSize += Resource_ConsoleSize;
            AsciiArtDisplay.Resources.Resource.ImageFromStream += Resource_ImageFromStream;
        }

        private static IImage Resource_ImageFromStream(Stream IconAsStream)
        {
            return new WindowsImage(IconAsStream);
        }

        private static IConsoleSize Resource_ConsoleSize()
        {
            return new WindowsConsoleSize();
        }
    }
    internal class WindowsConsoleSize : IConsoleSize
    {
        public int Height => System.Console.WindowHeight;

        public int Width => System.Console.WindowWidth;
    }
    internal class WindowsPixel : IPixel
    {
        private System.Drawing.Color Color { get; }

        public byte Red => Color.R;

        public byte Green => Color.G;

        public byte Blue => Color.B;

        public WindowsPixel(System.Drawing.Color color)
        {
            Color = color;
        }
    }
    internal class WindowsImage : IImage
    {
        private System.Drawing.Bitmap Bitmap { get; }
        public WindowsImage(Stream s)
        {
            Bitmap = new System.Drawing.Bitmap(System.Drawing.Bitmap.FromStream(s));
            
        }
        public int Height => Bitmap.Height;

        public int Width => Bitmap.Width;

        public IPixel GetPixel(int x, int y)
        {
            return new WindowsPixel(Bitmap.GetPixel(x, y));
        }
    }
}
