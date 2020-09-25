using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tamagotchi
{
    class Program
    {
        public static int health = 100;

        public void draw()
        {

        }
        public static void loop()
        {
            for (;;)
            {
                //Console.WriteLine(Console.WindowWidth);
                Thread.Sleep(1000);
                health--;
            }
        }
        static void Main(string[] args)
        {
            
            Thread back = new Thread(new ThreadStart(loop));
            back.Start();
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
