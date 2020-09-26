using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tamagotchi.Properties;
using static tamagotchi.ConsoleAPI;
using static tamagotchi.Avatars;

namespace tamagotchi
{
    class Program
    {
        public static Avatar avatar;
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
        static ConsoleText foodBar;
        
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
            //Console.Title = $"{Console.WindowWidth} x {Console.WindowHeight}";
            ConsoleDraw.ConsoleWriteImage(avatar.states[avatarState-1], 1, Console.WindowWidth - 31);

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
            SetQuickEdit(false);

            avatar = AvatarSelection();

            Thread back = new Thread(new ThreadStart(loop)); 

            healthBar = new ConsoleText(1, 0, Console.WindowWidth - 40, 0, "Balance: 100$", center: false);
            healthBar = new ConsoleText(Console.WindowWidth - 30, 19, Console.WindowWidth - 1, 19, "HP: 100 / 100", center: true);
            foodBar = new ConsoleText(Console.WindowWidth - 30, 20, Console.WindowWidth - 1, 20, "Food: 100 / 100", center: true);

            //new ConsoleText(10, 10, 14, 13, "  v  > 2 <  ^  ", center: true);

            back.Start();

            ConsoleEventHandler += new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(ConsoleEventHandler, true);
            Console.ReadLine();
        }

        static Avatar AvatarSelection()
        {
            int select = 0;
            ConsoleKeyInfo info;
            int count = avatars.Length;

            void drawAvatar()
            {
                ConsoleText inList = new ConsoleText(0, 2, Console.WindowWidth, 2, $"{select + 1}/{count}", center: true);
                ConsoleText avatarName = new ConsoleText(0, 4, Console.WindowWidth, 4, avatars[select].name, center: true);
                ConsoleDraw.ConsoleWriteImage(avatars[select].states[0], 6, Console.WindowWidth / 2 - 32 / 2);
            }
            void draw()
            {
                Console.Clear();
                ConsoleText title = new ConsoleText(0, 1, Console.WindowWidth, 1, "Выберите персонажа:", center: true);
                drawAvatar();
                ConsoleText hint1 = new ConsoleText(0, 6+16+2, Console.WindowWidth, 6+16+2, "<-- | -->", center: true);
                ConsoleText hint2 = new ConsoleText(0, 6+16+3, Console.WindowWidth, 6+16+3, "Enter", center: true);
            }
            draw();

            Thread resizeWatcher = new Thread(new ThreadStart(() =>
            {
                for (;;)
                {
                    if (Console.WindowWidth != Wwidth || Console.WindowHeight != Console.WindowHeight)
                    { 
                        draw();
                        Wheight = Console.WindowHeight;
                        Wwidth = Console.WindowWidth;
                    }
                    Thread.Sleep(1000);
                }
            }));
            resizeWatcher.Start();

            while (true)
            {
                info = Console.ReadKey(true);

                if (info.Key == ConsoleKey.RightArrow)
                {
                    if (select + 1 < count) select++;
                    drawAvatar();
                }
                else if (info.Key == ConsoleKey.LeftArrow)
                {
                    if (select > 0) select--;
                    drawAvatar();
                }
                else if (info.Key == ConsoleKey.Enter) { resizeWatcher.Abort(); Console.Clear(); return avatars[select]; }
            }
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