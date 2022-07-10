using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightningOS.LSSystem.Apps
{
    class CMD
    {
        public static void CMDRun()
        {
            Console.Clear();

            //Print top bar.

            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Command Interpreter" + "                            " + "\n");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
            string input = Console.ReadLine();
            while(input.ToLower() != "exit")
            {
                string inputLower = Console.ReadLine();
            }
        }
    }
}
