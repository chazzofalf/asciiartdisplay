using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiArtDisplay.Windows.Application
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AsciiArtDisplay.Windows.WindowsSetup.Init();
            Console.WriteLine(AsciiArtDisplay.Resources.Resource.IconAsConsoleString());
        }
    }
}
