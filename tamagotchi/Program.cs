using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tamagotchi.Properties;
using static tamagotchi.CloseEvent;
using static tamagotchi.QuickEditMode;

namespace tamagotchi
{
    class Program
    {
        private static int health = 100;
        public static int Health
        {
            get => health;
            set
            {
                health = value > 100 ? 100 : value;
            }
        }
        public static int avatarState = 0;

        static int Wheight = 120;
        static int Wwidth = 30;
        static int saveConsoleMode;

        static ConsoleText healthBar;
        
        public static int getAvatarState(int health_ = -1)
        {
            if (health_ == -1) health_ = Health;
            switch (health_)
            {
                case int h when (h <= 100 && h > 75): return 1;
                case int h when (h <= 75 && h > 50): return 2;
                case int h when (h <= 50 && h > 25): return 3;
                case int h when (h <= 25): return 4;
                default: return 0;
            }
        }
        public static void drawAvatar()
        {
            Console.Title = $"{Console.WindowWidth} x {Console.WindowHeight}";
            switch (avatarState)
            {
                case 1: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman1), 1, Console.WindowWidth - 31); break;
                case 2: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman2), 1, Console.WindowWidth - 31); break;
                case 3: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman3), 1, Console.WindowWidth - 31); break;
                case 4: ConsoleDraw.ConsoleWriteImage(new Bitmap(Resources.stickman4), 1, Console.WindowWidth - 31); break;
            }
            
        }
        public static void loop()
        {
            for (;;)
            {
                bool WResized = Console.WindowWidth != Wwidth || Console.WindowHeight != Console.WindowHeight;
                if (WResized)
                {
                    Console.Clear();
                    healthBar.SetArea(Console.WindowWidth - 30, 19, Console.WindowWidth - 1, 19);
                }
                //Console.WriteLine(Console.WindowWidth);
                Health--;
                healthBar.Text = $"HP: {Health} / 100";


                int state = getAvatarState();
                if (avatarState != state || WResized)
                {
                    avatarState = state;
                    drawAvatar();
                }

                Wheight = Console.WindowHeight;
                Wwidth = Console.WindowWidth;
                //Console.Title = $"{Wwidth} x {Wheight}";

                Thread.Sleep(200);
            }
        }
        static void Main(string[] args)
        {
            //GetConsoleMode(GetStdHandle((int)StdHandle.STD_INPUT_HANDLE), out saveConsoleMode);
            //helper.mb(saveConsoleMode);
            DisableQuickEdit();
            //Console.WindowWidth = 160;
            //Console.WindowHeight = 50;

            Thread back = new Thread(new ThreadStart(loop));

            healthBar = new ConsoleText(Console.WindowWidth - 30, 19, Console.WindowWidth - 1, 19, "HP: 100 / 100", center: true);

            back.Start();

            ConsoleEventHandler += new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(ConsoleEventHandler, true);
            Console.ReadLine();
        }

        public static bool ConsoleEventCallback(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    {
                        //SetConsoleMode(GetStdHandle((int)StdHandle.STD_INPUT_HANDLE), saveConsoleMode);
                        //helper.mb("closing...");
                    } return false;
                default:
                    return false;
            }
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