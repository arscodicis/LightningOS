using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningOS.LSSystem
{
    class SysBoot
    {
         

        public static void SysBootPrintMenu()
        {
            string state = "normal";
            do
            {
                while (!Console.KeyAvailable)
                {
                    if(Console.ReadKey(true).Key == ConsoleKey.UpArrow)
                    {
                        Console.Clear();
                        Console.Beep();
                        Console.WriteLine("* Start LightningOS Normally");
                        Console.WriteLine("  Start LightningOS in GUI Mode");
                        Console.WriteLine("Press ESC to abort boot.");
                        state = "normal";
                    }
                    else if(Console.ReadKey(true).Key == ConsoleKey.DownArrow)
                    {
                        Console.Clear();
                        Console.Beep();
                        Console.WriteLine("  Start LightningOS Normally");
                        Console.WriteLine("* Start LightningOS in GUI Mode");
                        Console.WriteLine("Press ESC to abort boot.");
                        state = "gui";
                    }
                    else if(Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        public static void SystemBoot()
        {
            //Console.WriteLine("* Start LightningOS Normally");
            //Console.WriteLine("  Start LightningOS in GUI Mode");
            //Console.WriteLine("Press ESC to abort boot.");
            //SysBootPrintMenu();
        }
    }
}
