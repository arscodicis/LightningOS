using System;
using System.Collections.Generic;
using System.Text;

namespace LightningOS
{
    public class LiquidEditor
    {
        public static string version = "0.1";
        public static char[] line = new char[80]; public static int pointer = 0;
        public static List<String> lines = new List<string>();
        public static string[] final;

        public static void Start(String currentdirectory)
        {
            Console.Clear();
            Utils.WriteTextCentered("Edit");
            Utils.WriteTextCentered("Version " + version);
            Console.Write("Enter File Path: ");
            string filename = Console.ReadLine();
            Start(filename, currentdirectory);
        }

        public static void Start(String filename, String currentdirectory)
        {
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    Console.Clear();
                    drawTopBar();
                    Console.SetCursorPosition(0, 1);
                    ConsoleKeyInfo c; cleanArray(line);

                    List<String> text = new List<String>();
                    text.Add(System.IO.File.ReadAllText(filename));

                    String file = "";

                    foreach (String value in text)
                    {
                        file = file + value;
                    }

                    Console.Write(file);

                    while ((c = Console.ReadKey(true)) != null)
                    {
                        drawTopBar();
                        Char ch = c.KeyChar;
                        if (c.Key == ConsoleKey.Escape)
                            break;

                        else if (c.Key == ConsoleKey.F1)
                        {
                            try
                            {
                                Console.Clear();
                                Console.BackgroundColor = ConsoleColor.Gray;
                                Console.ForegroundColor = ConsoleColor.Black;
                                Kernel.printLightning();
                                Utils.WriteTextCentered("OS Edit");
                                Utils.WriteTextCentered("Version " + version);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Black;

                                lines.Add(new String(line).TrimEnd());

                                final = lines.ToArray();
                                String foo = Utils.ConcatString(final);
                                System.IO.File.Create(filename);
                                System.IO.File.WriteAllText(filename, file + foo);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("'" + filename + "' has been saved in '" + currentdirectory + "'! Press enter...");
                                Console.ForegroundColor = ConsoleColor.White;

                                Console.ReadKey();
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                        }

                        else if (c.Key == ConsoleKey.F2)
                        {
                            Start(currentdirectory);
                            break;
                        }

                        switch (c.Key)
                        {
                            case ConsoleKey.Home: break;
                            case ConsoleKey.PageUp: break;
                            case ConsoleKey.PageDown: break;
                            case ConsoleKey.End: break;
                            case ConsoleKey.UpArrow:
                                if (Console.CursorTop > 1)
                                {
                                    Console.CursorTop = Console.CursorTop - 1;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                if (Console.CursorTop < 24)
                                {
                                    Console.CursorTop = Console.CursorTop + 1;
                                }
                                break;
                            case ConsoleKey.LeftArrow: if (pointer > 0) { pointer--; Console.CursorLeft--; } break;
                            case ConsoleKey.RightArrow: if (pointer < 80) { pointer++; Console.CursorLeft++; if (line[pointer] == 0) line[pointer] = ' '; } break;
                            case ConsoleKey.Backspace:
                                try
                                {
                                    deleteChar();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Issue deleting character. Please navigate with the arrow keys.");
                                }

                                break;
                            case ConsoleKey.Delete: deleteChar(); break;
                            case ConsoleKey.Enter:
                                lines.Add(new String(line).TrimEnd()); cleanArray(line); Console.CursorLeft = 0; Console.CursorTop++; pointer = 0;
                                break;
                            default: line[pointer] = ch; pointer++; Console.Write(ch); break;
                        }
                    }
                    Console.Clear();

                }
                else
                {
                    try
                    {
                        Console.Clear();
                        drawTopBar();
                        Console.SetCursorPosition(0, 1);
                        ConsoleKeyInfo c; cleanArray(line);
                        while ((c = Console.ReadKey(true)) != null)
                        {
                            drawTopBar();
                            Char ch = c.KeyChar;
                            if (c.Key == ConsoleKey.Escape)
                                break;
                            else if (c.Key == ConsoleKey.F1)
                            {
                                Console.Clear();
                                Console.BackgroundColor = ConsoleColor.Gray;
                                Console.ForegroundColor = ConsoleColor.Black;
                                Kernel.printLightning();
                                Utils.WriteTextCentered("OS Edit");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Black;

                                lines.Add(new String(line).TrimEnd());

                                final = lines.ToArray();
                                String foo = Utils.ConcatString(final);
                                System.IO.File.Create(filename);
                                System.IO.File.WriteAllText(filename, foo);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("'" + filename + "' has been saved in '" + currentdirectory + "' !");
                                Console.ForegroundColor = ConsoleColor.White;

                                Console.ReadKey();
                                break;
                            }
                            else if (c.Key == ConsoleKey.F2)
                            {
                                Start(currentdirectory);
                                break;
                            }
                            switch (c.Key)
                            {
                                case ConsoleKey.Home: break;
                                case ConsoleKey.PageUp: break;
                                case ConsoleKey.PageDown: break;
                                case ConsoleKey.End: break;
                                case ConsoleKey.UpArrow:
                                    if (Console.CursorTop > 1)
                                    {
                                        Console.CursorTop = Console.CursorTop - 1;
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    if (Console.CursorTop < 23)
                                    {
                                        Console.CursorTop = Console.CursorTop + 1;
                                    }
                                    break;
                                case ConsoleKey.LeftArrow: if (pointer > 0) { pointer--; Console.CursorLeft--; } break;
                                case ConsoleKey.RightArrow: if (pointer < 80) { pointer++; Console.CursorLeft++; if (line[pointer] == 0) line[pointer] = ' '; } break;
                                case ConsoleKey.Backspace:
                                    try
                                    {
                                        deleteChar();
                                    }
                                    catch (Exception ex)
                                    {
                                        //Console.WriteLine("Issue deleting character. Please navigate with the arrow keys.");
                                    }
                                    break;
                                case ConsoleKey.Delete: deleteChar(); break;
                                case ConsoleKey.Enter:
                                    lines.Add(new String(line).TrimEnd()); cleanArray(line); Console.CursorLeft = 0; Console.CursorTop++; pointer = 0;
                                    break;
                                default: line[pointer] = ch; pointer++; Console.Write(ch); break;
                            }
                        }
                        Console.Clear();
                    }
                    catch
                    {

                    }

                }
            }
            catch
            {

            }
        }

        public static void cleanArray(Char[] c)
        {
            for (int i = 0; i < c.Length; i++)
                c[i] = ' ';
        }

        public static void drawTopBar()
        {
            int x = Console.CursorLeft, y = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("LightningOS Edit" + version + "                                 ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("[F1]Save  [F2]New  [ESC]Exit\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(x, y);
        }

        public static void deleteChar()
        {
            try
            {
                if ((Console.CursorLeft >= 1) && (pointer >= 1))
                {
                    pointer--;
                    Console.CursorLeft--;
                    Console.Write(" ");
                    Console.CursorLeft--;
                    line[pointer] = ' ';
                }
                else if ((Console.CursorTop >= 2))
                {
                    Console.CursorTop--;
                    Console.Write(new String(' ', lines[lines.Count - 1].Length - 1));
                    Console.CursorTop--;
                    lines.RemoveAt(lines.Count - 1);
                    line = lines[lines.Count - 1].ToCharArray();
                }
            } catch
            {

            }
            
        }

        public static void listCheck()
        {
            foreach (var s in lines)
                Console.WriteLine(" List: " + s + "\n");
        }

        private String[] arrayCheck(String[] s)
        {
            foreach (var ss in s)
            {
                Console.WriteLine(" Line: " + ss + "\n");
            }
            return s;
        }
    }
}