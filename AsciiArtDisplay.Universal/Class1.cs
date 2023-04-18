using System;
using AsciiArtDisplay.Resources;
using SkiaSharp;

namespace AsciiArtDisplay.Universal
{
    public class UniversalSetup
    {
        public static void Init()
        {
            Resource.ConsoleSize += Resource_ConsoleSize;
            Resource.ImageFromStream += Resource_ImageFromStream;
            Resource.WriteByteAsConsoleBackground += Resource_WriteByteAsConsoleBackground;
        }

        private static void Resource_WriteByteAsConsoleBackground(byte value)
        {
            ConsoleColor[] shadesArray =
            {
                ConsoleColor.Black,
                ConsoleColor.DarkGray,
                ConsoleColor.Gray,                
                ConsoleColor.White
            };

            byte pxl = value;
            byte pxlrb = (byte)(255 / shadesArray.Length);
            int sp = pxl / pxlrb;
            sp = sp < shadesArray.Length ? sp : shadesArray.Length - 1;
            Console.BackgroundColor = shadesArray[sp];
            Console.Write(" ");
        }

        private static IImage Resource_ImageFromStream(System.IO.Stream IconAsStream)
        {
            return new UniversalImage(SKBitmap.Decode(IconAsStream));
        }

        private static IConsoleSize Resource_ConsoleSize()
        {
            return new UniversalConsoleSize();
        }
        
    }
    public class UniversalConsoleSize : IConsoleSize
    {
        public int Height => System.Console.WindowHeight;

        public int Width => System.Console.WindowWidth;
    }
    public class UniversalPixel : IPixel
    {
        public SKColor Color { get; }
        public UniversalPixel(SKColor color)
        {
            Color = color;
        }
        public byte Red => Color.Red;

        public byte Green => Color.Green;

        public byte Blue => Color.Blue;
    }
    public class UniversalImage : IImage
    {
        private SkiaSharp.SKBitmap Bitmap { get; }
        public UniversalImage(SkiaSharp.SKBitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public int Height => Bitmap.Height;

        public int Width => Bitmap.Width;

        public IPixel GetPixel(int x, int y)
        {
            return new UniversalPixel(Bitmap.GetPixel(x,y));
        }
    }

}
