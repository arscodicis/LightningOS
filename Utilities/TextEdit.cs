using System;

namespace LightningOS
{
    class TextEdit
    {
        public static void Run(string textToDisplay, string path)
        {
            Console.WriteLine("Press ESC to exit.");
            Console.WriteLine(textToDisplay);

            do
            {
                while (!Console.KeyAvailable)
                {
                    ConsoleKeyInfo pressed_key = Console.ReadKey();

                    if(pressed_key.Key == ConsoleKey.LeftArrow)
                    {
                        try
                        {
                            Console.SetCursorPosition(Console.CursorLeft - 1, 0);
                        } catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        

                    } else if(pressed_key.Key == ConsoleKey.RightArrow)
                    {
                        Console.CursorTop = Console.CursorLeft + 1;
                    }
                    else if (pressed_key.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
