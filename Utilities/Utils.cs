using System;
using System.Collections.Generic;
using System.Text;

namespace LightningOS
{
    public static class Utils
    {
        public static void WriteTextCentered(String content)
        {
            Console.Write(new string(' ', (Console.WindowWidth - content.Length - 2) / 2));
            Console.WriteLine(content);
        }
        public static String ConcatString(String[] s)
        {
            String t = "";
            try
            {
                
                if (s.Length >= 1)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (!String.IsNullOrWhiteSpace(s[i]))
                            t = String.Concat(t, s[i].TrimEnd(), Environment.NewLine);
                    }
                }
                else
                    t = s[0];
                t = String.Concat(t, '\0');
                
            } catch
            {

            }

            return t;

        }
    }
}