using System;
using System.IO;
using System.Linq;

namespace AsciiArtDisplay.Resources
{
    public class Resource
    {
        private static byte[] jpg_icon_data;
        public static Stream IconAsStream
        {
            get
            {
                return jpg_icon_data != null ? new MemoryStream(jpg_icon_data) : new MemoryStream(BuildIconData());
            }
        }
        public static void DrawToConsoleBackground()
        {
            try
            {
                DrawToConsoleBackgroundUnsafe();
            }
            catch (Exception)
            {
                Console.WriteLine("Hmm... This does not seems to be a fully featured terminal. (Are you headless or using VSCode?) That's fine. Going on...");
            }
        }
        private static void DrawToConsoleBackgroundUnsafe()
        {
            var ci = GetConsoleImage();
            ci.WriteToConsole();
        }
        public static string IconAsConsoleString()
        {
            return GetConsoleImage().ToString();
        }
        private static byte[] BuildIconData()
        {
            MemoryStream ms = new MemoryStream();
            Stream rs = typeof(Resource).Assembly.GetManifestResourceStream("rsrc_icon");
            rs.CopyTo(ms);
            rs.Close();
            jpg_icon_data = ms.ToArray();
            return jpg_icon_data;
        }
        private static InterfaceType MaskDelegateCall<InterfaceType, DelegateType>(DelegateType @delegate, params object[] args) where InterfaceType : class
            where DelegateType : Delegate
        {
            if (@delegate != null)
            {
                return (InterfaceType)((DelegateType)@delegate.GetInvocationList().FirstOrDefault())?.DynamicInvoke(args);
            }
            else
            {
                return null;
            }
        }
        private static ConsoleImage GetConsoleImage()
        {
            return new ConsoleImage(GetConsoleSize(), GetGrayImage());
        }
        private class ConsoleImage
        {
            private IConsoleSize ConsoleSize { get; }
            private GrayImage GrayImage { get; }
            public ConsoleImage(IConsoleSize size, GrayImage grayImage)
            {
                ConsoleSize = size;
                GrayImage = grayImage;
            }
            public int Width
            {
                get
                {
                    return ConsoleSize.Width;
                }
            }
            public int Height
            {
                get
                {
                    return ConsoleSize.Height;
                }
            }
            public byte GetPixel(int x, int y)
            {
                (double dx, double dy) = (x, y);
                (double sx, double sy) = (
                     GrayImage.Width / (double)Width,
                     GrayImage.Height / (double)Height
                     );
                (double ox, double oy) = (sx * x, sy * y);
                (int oix, int oiy) = ((int)ox, (int)oy);
                (int six, int siy) = ((int)sx, (int)sy);
                int size = six * siy;
                double dsize = size;
                return (byte)(Enumerable.Range(oiy, siy).Aggregate(0.0, (prev, next) =>
                {
                    var yy = next;
                    return prev + Enumerable.Range(oix, six).Aggregate(0.0, (prevx, nextx) =>
                    {
                        return prevx + GrayImage.GetPixelAt(nextx, yy) / 255.0;
                    });
                }) / dsize * 255);
            }
            private char GetShade(int x, int y)
            {
                string shades = " ░▒▓█";
                
                char[] shadesArray = shades.ToCharArray();
                byte pxl = GetPixel(x, y);
                byte pxlrb = (byte)(255 / shadesArray.Length);
                int sp = pxl / pxlrb;
                sp = sp < shadesArray.Length ? sp : shadesArray.Length - 1;
                return shadesArray[sp];
            }
            private char GetShade(int xy)
            {
                int y = xy / Width;
                int x = xy % Width;
                return GetShade(x, y);
            }
            private int GetStringLength()
            {
                return Width * Height;
            }
            public  void WriteToConsole(int x,int y)
            {
                byte pxl = GetPixel(x, y);
                DoWriteByteAsConsoleBackground(pxl);
            }
            public void WriteToConsole()
            {
                Enumerable.Range(0, GetStringLength()).Aggregate(null, (object prev, int next) =>
                {
                    WriteToConsole(next);
                    return null;
                });
            }
            public void WriteToConsole(int xy)
            {
                int y = xy / Width;
                int x = xy % Width;
                WriteToConsole(x, y);   
            }
            public override string ToString()
            {
                return new string((from idx in Enumerable.Range(0, GetStringLength())
                                   select GetShade(idx)).ToArray());
            }
        }
        private static GrayImage GetGrayImage()
        {
            return new GrayImage(GetImageFromStream());
        }
        private class GrayImage
        {
            private IImage original { get; }
            public GrayImage(IImage image)
            {
                original = image;
            }
            public int Width
            {
                get
                {
                    return original.Width;
                }
            }
            public int Height
            {
                get
                {
                    return original.Height;
                }
            }
            public byte GetPixelAt(int x, int y)
            {
                IPixel pixel = original.GetPixel(x, y);
                (double redd, double greend, double blued) = (
                    pixel.Red / 255.0,
                    pixel.Green / 255.0,
                    pixel.Blue / 255.0
                    );
                double gray = redd * 0.2126 + greend * 0.7152 + blued * 0.0722;
                byte grayb = (byte)(gray * 255);
                return grayb;
            }
        }
        private static IImage GetImageFromStream()
        {
            return MaskDelegateCall<IImage, ImageFromStreamEvent>(ImageFromStream, IconAsStream);
        }
        public delegate IImage ImageFromStreamEvent(Stream IconAsStream);
        public static event ImageFromStreamEvent ImageFromStream;
        private static IConsoleSize GetConsoleSize()
        {
            return MaskDelegateCall<IConsoleSize, ConsoleSizeEvent>(ConsoleSize);
        }
        public delegate IConsoleSize ConsoleSizeEvent();
        public static event ConsoleSizeEvent ConsoleSize;

        public static void DoWriteByteAsConsoleBackground(byte value)
        {
            _ = MaskDelegateCall<object, WriteByteAsConsoleBackgroundEvent>(WriteByteAsConsoleBackground, value);
        }
        public delegate void WriteByteAsConsoleBackgroundEvent(byte value);
        public static event WriteByteAsConsoleBackgroundEvent WriteByteAsConsoleBackground;
    }
    public interface IConsoleSize
    {
         int Height { get; }
         int Width { get; }
    }
    public interface IImage
    {
         int Height { get; }
         int Width { get; }
         IPixel GetPixel(int x, int y);
    }
    public interface IPixel
    {
         byte Red { get; }
         byte Green { get; }
         byte Blue { get; }
    }
}
