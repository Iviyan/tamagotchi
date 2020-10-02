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
using System.ComponentModel;
using System.Diagnostics;

namespace tamagotchi
{
    class Program
    {
        public static Avatar avatar;
        private static int balance = 100;
        private static int health = 100;
        private static int satiety = 80;
        private static int joy = 50;
        private static int fatigue = 0;
        public static int disease = 0;
        public static int Health
        {
            get => health;
            set => health = Limit(value);
        }
        public static int Satiety
        {
            get => satiety;
            set => satiety = Limit(value);
        }
        public static int FoodCost { get => 10 + (100 - Satiety) / 2; }
        public static int Joy
        {
            get => joy;
            set => joy = Limit(value);
        }
        public static int Fatigue
        {
            get => fatigue;
            set => fatigue = Limit(value);
        }

        public static void FallIll() => disease += rnd.Next(10, 50);
        public static void Recover() => disease = 0;
        public static int Limit(int num, int min = 0, int max = 100)
        {
            if (num < min) return min;
            if (num > max) return max;
            return num;
        }
        public static int Balance { get => balance; set { balance = value; balanceBar.Text = $"Баланс: ${balance}"; } }

        static void CheckDisease()
        {
            if (disease > 0)
                diseaseBar.Text = $"{avatar.name} болеет ({disease} сек.)";
            else if (disease  == 0)
                diseaseBar.Text = $"{avatar.name} не болеет";
        }
        public static void DisableAllActions()
        {
            Enum.GetValues(typeof(Actions))
                .Cast<Actions>()
                .ForEach(v =>
                {
                    actions.Disable((int)v);
                });
        }
        public static void EnableAllActions()
        {
            if (!feedActionDisable) actions.Enable((int)Actions.Feed);
            if (!workActionDisable) actions.Enable((int)Actions.Work);
            if (!playActionDisable) actions.Enable((int)Actions.Play);
            if (!relaxActionDisable) actions.Enable((int)Actions.Relax);
            if (!cureActionDisable) actions.Enable((int)Actions.Cure);
            /*Enum.GetValues(typeof(Actions))
                .Cast<Actions>()
                .ForEach(v =>
                {
                    actions.Enable((int)v);
                });*/
        }

        public static bool Playing = false;
        
        static int cooldown = 0;
        static Actions cooldownAction;

        static bool feedActionDisable = false;
        static bool workActionDisable = false;
        static bool playActionDisable = false;
        static bool relaxActionDisable = false;
        static bool cureActionDisable = false;

        public const int work_balance = 50;
        public const int work_fatigue = 30;
        public const int work_satiety = -25;
        public const int work_joy = -10;

        public const int play_fatigue = 25;
        public const int play_satiety = -20;
        public const int play_joy = 75;

        public const int relax_fatigue = -75;
        public const int relax_satiety = -25;
        public const int relax_joy = 15;

        static bool PossibleFeed { get => FoodCost <= balance; }
        static bool PossibleWork { get => Fatigue <= 100 - work_fatigue && satiety > work_fatigue; }
        static bool PossiblePlay { get => Fatigue <= 100 - play_fatigue && satiety > play_satiety; }
        static bool PossibleRelax { get => Satiety > 10; }
        public static void CheckPossibleFeed()
        {
            int foodCost = FoodCost;

            if (cooldown > 0 && cooldownAction == Actions.Feed)
            {
                actions.Edit((int)Actions.Feed, $"{Actions.Feed.Description()} ${foodCost} ({cooldown} сек.)");
            } else
            {
                actions.Edit((int)Actions.Feed, $"{Actions.Feed.Description()} ${foodCost}");
                if (!feedActionDisable && !PossibleFeed)
                {
                    feedActionDisable = true;
                    actions.Disable((int)Actions.Feed);
                } else if (feedActionDisable && PossibleFeed && cooldown == 0)
                {
                    feedActionDisable = false;
                    actions.Enable((int)Actions.Feed);
                }
            }
        }
        public static void CheckPossibleWork()
        {
            if (cooldown > 0 && cooldownAction == Actions.Work)
            {
                actions.Edit((int)Actions.Work, $"{Actions.Work.Description()} ({cooldown} сек.)");
            } else
            {
                actions.Edit((int)Actions.Work, $"{Actions.Work.Description()}");
                if (!workActionDisable && !PossibleWork)
                {
                    workActionDisable = true;
                    actions.Disable((int)Actions.Work);
                } else if (workActionDisable && PossibleWork && cooldown == 0)
                {
                    workActionDisable = false;
                    actions.Enable((int)Actions.Work);
                }
            }
        }
        public static void CheckPossiblePlay()
        {
            if (cooldown > 0 && cooldownAction == Actions.Play)
            {
                actions.Edit((int)Actions.Play, $"{Actions.Play.Description()} ({cooldown} сек.)");
            } else
            {
                actions.Edit((int)Actions.Play, $"{Actions.Play.Description()}");
                if (!feedActionDisable && !PossiblePlay)
                {
                    playActionDisable = true;
                    actions.Disable((int)Actions.Play);
                } else if (playActionDisable && PossiblePlay && cooldown == 0)
                {
                    playActionDisable = false;
                    actions.Enable((int)Actions.Play);
                }
            }
        }
        public static void CheckPossibleRelax()
        {
            if (cooldown > 0 && cooldownAction == Actions.Relax)
            {
                actions.Edit((int)Actions.Relax, $"{Actions.Relax.Description()} ({cooldown} сек.)");
            } else
            {
                actions.Edit((int)Actions.Relax, $"{Actions.Relax.Description()}");
                if (!relaxActionDisable && !PossibleRelax)
                {
                    relaxActionDisable = true;
                    actions.Disable((int)Actions.Relax);
                } else if (relaxActionDisable && PossibleRelax && cooldown == 0)
                {
                    relaxActionDisable = false;
                    actions.Enable((int)Actions.Relax);
                }
            }
        }

        private static Queue<int> balanceQ;
        private static Queue<int> healthQ;
        private static Queue<int> satietyQ;
        private static Queue<int> joyQ;
        private static Queue<int> fatigueQ;

        static Queue<int> CreateActionQueue(int num, int time)
        {
            int numBase = num / time;
            int[] res = new int[time];
            for (int i = 0; i < time; i++) res[i] = numBase;
            int numMod = num % time;
            if (numMod == 0) return helper.ArrayToQueue(res);
            int[] numMods = helper.RndItems(time, numMod);
            for (int i = 0; i < time; i++) res[i] += numMods[i];
            return helper.ArrayToQueue(res);
        }

        public static void Feed()
        {
            Balance -= FoodCost;
            satietyQ = CreateActionQueue(100, 10);//Satiety = 100;
            cooldown += 10;
            cooldownAction = Actions.Feed;
            DisableAllActions();
            CheckPossibleFeed();
        }
        public static void Work()
        {
            balanceQ = CreateActionQueue(work_balance, 10); //helper.mb(String.Join(",", balanceQ.ToArray()));
            fatigueQ = CreateActionQueue(work_fatigue, 10);//Fatigue += work_fatigue;
            satietyQ = CreateActionQueue(work_satiety, 10); //Satiety += work_satiety;
            joyQ = CreateActionQueue(work_joy, 10); //Satiety += work_satiety;
            cooldown += 10;
            cooldownAction = Actions.Work;
            DisableAllActions();
            CheckPossibleWork();
        }
        public static void Play()
        {
            GamePressKey gps = new GamePressKey();
            Playing = true;
            gps.start();
            Playing = false;
            fatigueQ = CreateActionQueue(play_fatigue, 10);//Fatigue += work_fatigue;
            satietyQ = CreateActionQueue(play_satiety, 10);
            joyQ = CreateActionQueue(play_joy, 10);
            cooldown += 10;
            cooldownAction = Actions.Play;
            DisableAllActions();
            CheckPossiblePlay();
        }
        public static void Relax()
        {
            fatigueQ = CreateActionQueue(relax_fatigue, 10);//Fatigue += work_fatigue;
            satietyQ = CreateActionQueue(relax_satiety, 10);
            joyQ = CreateActionQueue(relax_joy, 10);
            cooldown += 10;
            cooldownAction = Actions.Relax;
            DisableAllActions();
            CheckPossibleRelax();
        }
        public static int avatarState = 0;

        static int Wheight = 120;
        static int Wwidth = 30;

        static ConsoleText balanceBar;
        static ConsoleText healthBar;
        static ConsoleText foodBar;
        static ConsoleText joyBar;
        static ConsoleText fatigueBar;
        static ConsoleText diseaseBar;

        static ConsoleRadioGroup actions;

        public enum Actions
        {
            [Description("Покормить")]
            Feed = 0,
            [Description("Поработать")]
            Work,
            [Description("Поиграть")]
            Play,
            [Description("Отдохнуть")]
            Relax,
            [Description("Вылечить")]
            Cure
        }

        public static Random rnd = new Random();

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
            ConsoleDraw.ConsoleWriteImage(avatar.states[avatarState - 1], 1, Console.WindowWidth - 31);

        }

        static void logic(ref int c)
        {
            if (cooldown > 0)
            {
                if (cooldown - 1 == 0) EnableAllActions();
                
                switch (cooldownAction)
                {
                    case Actions.Feed:
                        Satiety += satietyQ.Dequeue();
                        break;
                    case Actions.Work:
                        Balance += balanceQ.Dequeue(); 
                        Fatigue += fatigueQ.Dequeue();
                        Satiety += satietyQ.Dequeue();
                        Joy += joyQ.Dequeue();
                        break;
                    case Actions.Play:
                        Fatigue += fatigueQ.Dequeue();
                        Satiety += satietyQ.Dequeue();
                        Joy += joyQ.Dequeue();
                        break;
                    case Actions.Relax:
                        Fatigue += fatigueQ.Dequeue();
                        Satiety += satietyQ.Dequeue();
                        Joy += joyQ.Dequeue();
                        break;
                    
                }
                
                cooldown--;
            }
            else
            {
                if (disease == 0 && rnd.Next(0, 25) == 15) FallIll();
                //helper.mb(c, " ", c % 2);
                int satietySub = 0;
                if (c % 2 == 0) satietySub += 1;
                if (health < 50 && disease > 0 || health < 50 && c % 2 == 0 || disease > 0 && c % 2 == 0) satietySub += 1;
                Satiety -= satietySub;

                int joySub = 0;
                if (joy > 50) joySub += 1;
                else if (c % 2 == 0) joySub += 1;
                if (health < 50 || disease > 0) joySub += 1;
                if (satiety < 50) joySub += 1;
                if (fatigue > 50) joySub += 1;
                Joy -= joySub;

                int fatigueSub = 0;
                if (c % 3 == 0) fatigueSub += 1;
                if (health < 50 || disease > 0) fatigueSub += 1;
                Fatigue += fatigueSub;

                int healthSub = 0;
                if (disease > 0) healthSub += 1;
                if (satiety < 10) healthSub += 1; if (satiety > 70) healthSub -= 1;
                if (joy == 0) healthSub += 1;
                if (fatigue > 90) healthSub += 1; if (fatigue < 20 && c % 2 == 0) healthSub -= 1;
                Health -= healthSub;

                if (c == 6) c = 1; else c++;
            }

            if (disease > 0) disease--;
        }

        static void loop()
        {
            int c = 1;
            for (; ; )
            {
                if (Playing) { Thread.Sleep(500); continue; }
                bool WResized = Console.WindowWidth != Wwidth || Wheight != Console.WindowHeight;
                if (WResized)
                {
                    Console.Clear();
                    healthBar.SetArea(Console.WindowWidth - 30, 19, Console.WindowWidth - 1, 19);
                    actions.SetArea(1, 3, width: Console.WindowWidth - 40);
                }
                //Console.WriteLine(Console.WindowWidth);

                if (disease - 1 > 0)
                    diseaseBar.Text = $"{avatar.name} болеет ({disease} сек.)";
                else if (disease - 1 == 0)
                    diseaseBar.Text = $"{avatar.name} не болеет";

                logic(ref c);
                
                healthBar.Text = $"Здоровье: {Health} / 100";
                foodBar.Text = $"Сытость: {Satiety} / 100";
                joyBar.Text = $"Радость: {Joy} / 100";
                fatigueBar.Text = $"Усталость: {Fatigue} / 100";


                CheckPossibleFeed();
                CheckPossibleWork();
                CheckPossiblePlay();
                CheckPossibleRelax();

                int state = getAvatarState();
                if (avatarState != state || WResized)
                {
                    avatarState = state;
                    drawAvatar();
                }

                Wheight = Console.WindowHeight;
                Wwidth = Console.WindowWidth;
                //Console.Title = $"{Wwidth} x {Wheight}";

                Thread.Sleep(500);
            }

        }

        static void Main(string[] args)
        {
            //GetConsoleMode(GetStdHandle((int)StdHandle.STD_INPUT_HANDLE), out saveConsoleMode);
            //helper.mb(saveConsoleMode);
            //helper.mb(Actions.Cure.GetAttributeOfType<DescriptionAttribute>().Description);

            /*for (int i = 0; i < 10; i++) Console.Write(i);
            Console.WriteLine();
            for (int i = 0; i < 1; i++)  Console.WriteLine(String.Join("", helper.RndItems(10, 4).Select( a => a+1)));*/
            /*int[] res = new int[10];
            for (int i = 0; i < 10; i++) res[i] = 0;
            for (int i = 0; i < 1000; i++) res[rnd.Next(0, 10)]++;
            Console.WriteLine(String.Join("|", res));*/

            /*Console.ReadKey();
            return;*/

            avatar = AvatarSelection();

            Thread back = new Thread(new ThreadStart(loop));

            balanceBar = new ConsoleText(1, 0, Console.WindowWidth - 40, 0, "Баланс: 100$", center: false);
            healthBar = new ConsoleText(Console.WindowWidth - 30, 19, Console.WindowWidth - 1, 19, $"Здоровье: {health} / 100", center: true);
            foodBar = new ConsoleText(Console.WindowWidth - 30, 20, Console.WindowWidth - 1, 20, $"Сытость: {satiety} / 100", center: true);
            joyBar = new ConsoleText(Console.WindowWidth - 30, 21, Console.WindowWidth - 1, 21, $"Радость: {joy} / 100", center: true);
            fatigueBar = new ConsoleText(Console.WindowWidth - 30, 22, Console.WindowWidth - 1, 22, $"Усталость: {fatigue} / 100", center: true);
            diseaseBar = new ConsoleText(Console.WindowWidth - 30, 23, Console.WindowWidth - 1, 23, $"{avatar.name} не болеет", center: true);

            actions = new ConsoleRadioGroup(
                x1: 1, y1: 3,
                new List<string>(
                    Enum.GetValues(typeof(Actions))
                        .Cast<Actions>()
                        .Select(v => v.Description())
                        .ToList()
                ),
                interval: 1,
                width: Console.WindowWidth - 40
            );

            back.Start();

            ConsoleEventHandler += new ConsoleEventDelegate(ConsoleEventCallback); //Обработка закрытия
            SetConsoleCtrlHandler(ConsoleEventHandler, true);

            int sel = 0;
            for (; ; )
            {
                sel = actions.Choice(sel);
                Actions act = (Actions)(sel);

                switch (act)
                {
                    case Actions.Feed:
                        {
                            Feed();
                        }
                        break;
                    case Actions.Work:
                        {
                            Work();
                        }
                        break;
                    case Actions.Play:
                        {
                            Play();
                        }
                        break;
                    case Actions.Relax:
                        {
                            Relax();
                        }
                        break;
                    case Actions.Cure:
                        {
                            disease = 0;
                            CheckDisease();
                        }
                        break;
                }
                //helper.mb(act);
            }

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
                ConsoleText hint1 = new ConsoleText(0, 6 + 16 + 2, Console.WindowWidth, 6 + 16 + 2, "<-- | -->", center: true);
                ConsoleText hint2 = new ConsoleText(0, 6 + 16 + 3, Console.WindowWidth, 6 + 16 + 3, "Enter", center: true);
            }
            draw();

            Thread resizeWatcher = new Thread(new ThreadStart(() =>
            {
                for (; ; )
                {
                    if (Console.WindowWidth != Wwidth || Wheight != Console.WindowHeight)
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
                    }
                    return false;
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