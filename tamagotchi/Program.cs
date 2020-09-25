using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tamagotchi.Properties;

namespace tamagotchi
{
    class Program
    {
        public static int health = 100;

        static int Wheight = 120;
        static int Wwidth = 30;

        public static void drawAvatar()
        {
            Console.Title = health.ToString();
            switch (health)
            {
                case 99: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman1), 0, Console.WindowWidth - 31); break;
                case 75: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman2), 0, Console.WindowWidth - 31); break;
                case 50: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman3), 0, Console.WindowWidth - 31); break;
                case 25: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman4), 0, Console.WindowWidth - 31); break;
            }
        }
        public static void loop()
        {
            for (;;)
            {
                //Console.WriteLine(Console.WindowWidth);
                Thread.Sleep(100);
                health--;
                
                if (Console.WindowWidth != Wwidth || Console.WindowHeight != Console.WindowHeight)
                {
                    Console.Clear();
                }

                drawAvatar();

                Wheight = Console.WindowHeight;
                Wwidth = Console.WindowWidth;
                //Console.Title = $"{Wwidth} x {Wheight}";
            }
        }
        static void Main(string[] args)
        {
            
            Thread back = new Thread(new ThreadStart(loop));
            back.Start();
            /*for (int i = 0; i < 120; i++) Console.Write("█");
            Console.CursorTop++;
            Console.Write("█");
            Console.Read();*/
        }


    }
}

/*Справа вверху - аватар
 * руки опускаются со здоровьем
 * под аватаром - 95/100
 * меню - деньги, действия
 * работа - миниигра, нажимать на клавиши, высвечивающиеся на экране
 * покормить / вылечить
 * болезнь
 * 
 * 
 */
