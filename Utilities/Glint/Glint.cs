using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

namespace MicroBolt
{
    public class Runtime
    {

        public static string[] args;
        public static int number = 0;
        public static List<string> vars = new List<string>();

        public static void Interpreter(string code)
        {
            args = code.Split(" ");
            if (args[0] == "forever")
            {
                while (true)
                {
                    check();
                    if (Sys.KeyboardManager.AltPressed)
                    {
                        break;
                    }
                }
            }

            if (args[0] == "for")
            {
                for (int program = 0; program < Int32.Parse(args[1]) - 1; program++)
                {
                    check();
                    if (Sys.KeyboardManager.AltPressed)
                    {
                        break;
                    }
                }
            }
            check();
        }

        public static void check()
        {
            //Console.WriteLine("In check mode....");
            for (int count = 0; count < args.Length; count++)
            {
                //Console.WriteLine("In check mode....");
                if (args[count] == "print[")
                {
                    try
                    {
                        for (int generaluse = 1; generaluse < Int32.MaxValue; generaluse++)
                        {
                            if (args[generaluse] == "]")
                            {
                                break;
                            }
                            Console.Write(args[generaluse] + " ");
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("\nMicroBolt - Print Statement Not Properly Ended\nTraceback:\n    Expected ']' At End Of Print Statement.");
                        break;
                    }
                }

                if (args[count] == "println[")
                {
                    try
                    {
                        for (int generaluse = 1; generaluse < Int32.MaxValue; generaluse++)
                        {
                            if (args[generaluse] == "]")
                            {
                                Console.Write("\n");
                                break;
                            }
                            Console.Write(args[generaluse] + " ");
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("\nMicroBolt - Println Statement Not Properly Ended\nTraceback:\n    Expected ']' At End Of Println Statement.");
                        break;
                    }
                }

                if (args[count] == "printvar[")
                {
                    try
                    {
                        Console.Write(vars[int.Parse(args[count + 1])]);
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    
                }

                if (args[count] == "printMath")
                {
                    string[] numbers = (args[count + 1]).Split("+");
                    int answer = (Int32.Parse(numbers[0]) + Int32.Parse(numbers[1]));
                    Console.Write(answer);
                }

                if (args[count] == "increment")
                {
                    number += Int32.Parse(args[count + 1]);
                    Console.Write(number);
                }

                if (args[count] == "clear")
                {
                    Console.Clear();
                }

                if (args[count] == "waitforever")
                {
                    while (true)
                    {
                        if (Sys.KeyboardManager.AltPressed)
                        {
                            break;
                        }
                    }
                }

                if (args[count] == "beep")
                {
                    Sys.PCSpeaker.Beep(uint.Parse(args[count + 1]));
                }

                if (args[count] == "cursor")
                {
                    Console.SetCursorPosition(Int16.Parse(args[count + 1]), Int16.Parse(args[count + 2]));
                }

                if (args[count] == "cursoroff")
                {
                    Console.CursorVisible = false;
                }

                if (args[count] == "cursoron")
                {
                    Console.CursorVisible = true;
                }

                if (args[count] == "newvar[")
                {
                    vars.Add(args[count + 1]);
                }

                if (args[count] == "changevar[")
                {
                    try
                    {
                        vars.Insert(int.Parse(args[count + 1]), args[count + 2]);
                    } catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    
                }

                if (args[count] == "rmvar[")
                {
                    vars.RemoveAt(int.Parse(args[count + 1]));
                }

                if (args[count] == "((")
                {
                    try
                    {
                        for (int comment = 0; comment < Int32.MaxValue; comment++)
                        {
                            if (args[comment] == "))")
                            {
                                break;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("\nMicroBolt - Comment Not Properly Ended\nTraceback:\n    Expected '))' At End Of Comment.");
                        break;
                    }
                }
            }
        }

        public static void CMDInterpreter()
        {
            while (true)
            {
                Console.Write("\n>");
                string arg = Console.ReadLine();
                if(arg != "quit")
                {
                    Runtime.Interpreter(arg);   
                } else
                {
                    Console.Clear();
                    break;
                }
            }
        }

        public static void Run(string scriptpath)
        {
            var scriptfile = scriptpath;
            try
            {
                string text = File.ReadAllText(scriptfile);
                //Console.WriteLine(text);
                Runtime.Interpreter(text);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            
        }
    }
}