using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Update_BDS_AND_LL
{
    internal class Logger
    {
        static string getForMatTime(DateTime t)
        {
            return string.Format("{0}:{1}:{2}:{3}",
                t.Hour.ToString().PadLeft(2, '0'),
                t.Minute.ToString().PadLeft(2, '0'),
                t.Second.ToString().PadLeft(2, '0'),
                t.Millisecond.ToString().PadLeft(3, '0'));
        }

        public static void Info(String argn,params Object?[] p)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0} ", getForMatTime(DateTime.Now));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("INFO");
            Console.ResetColor();
            Console.Write("] ");
            Console.ForegroundColor = (currentColor == ConsoleColor.Gray) ? ConsoleColor.White : currentColor;
            Console.WriteLine(argn,p);
        }

        public static void Warn(String argn, params Object?[] p)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0} ", getForMatTime(DateTime.Now));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("WARN");
            Console.ResetColor();
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(argn, p);
            Console.ForegroundColor = currentColor;
        }
        public static void Error(String argn, params Object?[] p)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("{0} ", getForMatTime(DateTime.Now));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("EROR");
            Console.ResetColor();
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(argn, p);
            Console.ForegroundColor = currentColor;
        }
    }
}
