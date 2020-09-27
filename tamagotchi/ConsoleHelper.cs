using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    public static class ConsoleHelper
    {
        public static void ClearArea(int x1, int y1, int x2, int y2)
        {
            int width = x2 - x1 + 1;
            for (; y1 <= y2; y1++)
            {
                Console.SetCursorPosition(x1, y1);
                Console.Write(new string(' ', width));
            }
        }
    }
}
