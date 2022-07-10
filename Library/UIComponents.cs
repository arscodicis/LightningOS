using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUI;

namespace LightningOS.Library
{

    class UIComponents
    {

        public static void drawHeader(string titleLeft, string titleRight)
        {
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(titleLeft + "                                 ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(titleRight + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
        }

        public static void drawButton(string text, int x, int y)
        {
            VGADriver driver;
            driver = new VGADriver();
            Button btn = new Button(text, x, y);
            btn.OnEnter += Btn_OnEnter;
            Screen s = new Screen();
            s.Controls.Add(btn);
            driver.RenderScreen(s);
           
        }

        private static void Btn_OnEnter(object sender, EventArgs e)
        {
            Console.WriteLine("Button clicked.");
        }
    }
}
