using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    public static class ConsoleDraw
    {
        static ConsoleColor ClosestConsoleColor(Color cl)
        {
            ConsoleColor ret = 0;
            double rr = cl.R, gg = cl.G, bb = cl.B, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }

        public static void ConsoleWriteImage(Bitmap src, int start_y = -1, int start_x = 0)
        {
            if (start_y >= 0) Console.CursorTop = start_y;
            for (int i = 0; i < src.Height; i++)
            {
                Console.CursorLeft = start_x;
                for (int j = 0; j < src.Width; j++)
                {
                    ConsoleColor c = ClosestConsoleColor(src.GetPixel(j, i));
                    Console.ForegroundColor = c;
                    Console.Write("██");
                }
                if (!(Console.CursorLeft < start_x + src.Width)) Console.CursorTop++;
                
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
