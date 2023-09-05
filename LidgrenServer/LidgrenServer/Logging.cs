using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LidgrenServer
{
    class Logging
    {
        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[0] [INFO] {1}", Date(), message);
            Console.ResetColor();
        }
        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[0] [WARN] {1}", Date(), message);
            Console.ResetColor();
        }
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[0] [ERROR] {1}", Date(), message);
            Console.ResetColor();
        }
        public static void Debug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[0] [DEBUG] {1}", Date(), message);
            Console.ResetColor();
        }
        public static string Date()
        {
            return DateTime.Now.ToString();
        }
    }
}
