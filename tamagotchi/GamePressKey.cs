using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tamagotchi
{
    public class GamePressKey
    {

        public int interval = 500;
        public int count = 30;
        public const int waitTimes = 4;

        const int rightAreaWidth = 20;

        Point[,] points;

        int press = 0,
            missed = 0,
            misprints = 0;

        public static readonly object lettersLock = new object();

        static Random rnd = new Random();
        public static char GetRndLetter()
        {
            // This method returns a random lowercase letter.
            // ... Between 'a' and 'z' inclusize.
            int num = rnd.Next(0, 26); // Zero to 25
            char let = (char)('A' + num);
            return let;
        }

        class Key
        {
            public char letter;
            public int timesLeft;
            public int X, Y;
            public Key(char letter, int X, int Y, int timesLeft = waitTimes)
            {
                this.letter = letter;
                this.timesLeft = timesLeft;
                this.X = X;
                this.Y = Y;
            }
        }
        Dictionary<char, Key> Letters = new Dictionary<char, Key>();
        //HashSet<char> letterList = new HashSet<char>();
        HashSet<Point> usedPointList = new HashSet<Point>();

        void AddLetter(char letter, int x, int y)
        {
            Letters.Add(letter, new Key(letter, x, y));
            //letterList.Add(letter);
            usedPointList.Add(new Point(x, y));
            WriteLetter(letter, x, y);
        }

        public GamePressKey(int interval = 800, int count = 30)
        {
            this.interval = interval;
            this.count = count;
        }

        int Wpoints, Hpoints;

        ConsoleText leftBar, pressBar, missBar, misprintBar;

        bool End = false;

        public int start()
        {
            Console.Clear();
            /*Console.WindowWidth = 35;
            Console.WindowHeight = 35;*/

            int WWidth = Console.WindowWidth;
            int WHeight = Console.WindowHeight;

            int dist = 1;
            Wpoints = (WWidth - 4 - (rightAreaWidth + 1) + 1) / 4; 
            Hpoints = (WHeight - 2 + 1) / 4;// helper.mb(Hpoints);

            Thread t = new Thread(loop);
            lock (G.consoleLock)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(new string('#', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write(new string('#', Console.WindowWidth));
                for (int i = 1; i < Console.WindowHeight - 1; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("#");
                    Console.SetCursorPosition(Console.WindowWidth - 1 - (rightAreaWidth + 1), i);
                    Console.Write("#");
                    Console.SetCursorPosition(Console.WindowWidth - 1, i);
                    Console.Write("#");
                }
                Console.SetCursorPosition(0, 0);
            }

            points = new Point[Hpoints, Wpoints];
            for (int y = 0; y < Hpoints; y += 1)
                for (int x = 0; x < Wpoints; x += 1)
                    points[y, x] = new Point(3 + x * 4, 2 + y * 4);

            //helper.mb(points.GetLength(0), " ", points.GetLength(1));

            /*for (int y = 0; y < Hpoints; y += 1)
                for (int x = 0; x < Wpoints; x += 1)
                {
                    int x_ = points[y, x].X, y_ = points[y, x].Y;
                    // helper.mb($"{y}: {y_} X {x}: {x_}");
                    Console.SetCursorPosition(x_, y_);
                    Console.Write("X");
                    Console.SetCursorPosition(x_ - 1, y_ - 1); Console.Write("/");
                    Console.SetCursorPosition(x_ + 1, y_ - 1); Console.Write("\\");
                    Console.SetCursorPosition(x_ + 1, y_ + 1); Console.Write("/");
                    Console.SetCursorPosition(x_ - 1, y_ + 1); Console.Write("\\");
                }*/

            leftBar = new ConsoleText(Console.WindowWidth - rightAreaWidth, 2, Console.WindowWidth - 3, 2, $"Осталось: {count}/{count}", center: true);
            pressBar = new ConsoleText(Console.WindowWidth - rightAreaWidth, 4, Console.WindowWidth - 3, 4, $"Нажато: 0/{count}", center: true);
            missBar = new ConsoleText(Console.WindowWidth - rightAreaWidth, 5, Console.WindowWidth - 3, 5, $"Пропущено: 0", center: true);
            misprintBar = new ConsoleText(Console.WindowWidth - rightAreaWidth, 6, Console.WindowWidth - 3, 6, $"Опечатки: 0", center: true);

            new ConsoleText(Console.WindowWidth - rightAreaWidth, Console.WindowHeight - 1 - 3, Console.WindowWidth - 3, Console.WindowHeight - 1 - 3, $"Интервал: {interval / 1000d} сек.", center: true);


            t.Start();

            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey(true);
                char letter = char.ToUpper(key.KeyChar);
                //helper.mb(letter);
                if (!End)
                {
                    if (Letters.Keys.Contains(letter))
                    {
                        Key k = Letters[letter];
                        WriteLetter(k.letter, k.X, k.Y, 0);
                        usedPointList.Remove(new Point(k.X, k.Y));
                        lock (lettersLock)
                            Letters.Remove(letter);

                        press++;
                        pressBar.Text = $"Нажато: {press}/{count}";
                    } else
                    {
                        misprints++;
                        misprintBar.Text = $"Опечатки: {misprints}";
                    }
                } else
                {
                    if (key.Key == ConsoleKey.Spacebar)
                    {
                        Console.Clear();
                        return press;
                    }
                }
            }
            
            return 1;
        }

        void loop()
        {
            int left = count;
            for (; ;)
            {
                if (left > 0)
                {
                    int XR, YR;
                    do
                    {
                        XR = rnd.Next(0, Wpoints);
                        YR = rnd.Next(0, Hpoints);
                    } while (usedPointList.Contains(new Point(XR, YR)));

                    char letter;
                    do letter = GetRndLetter(); while (Letters.Keys.Contains(letter));
                    AddLetter(letter, XR, YR);

                    left--;
                    leftBar.Text = $"Осталось: {left}/{count}";
                }

                List<char> delete = new List<char>();
                lock (lettersLock)
                {
                    foreach (Key item in Letters.Values)
                    {
                        int x = item.X, y = item.Y;
                        WriteLetter(item.letter, x, y, item.timesLeft);
                        item.timesLeft--;
                        if (item.timesLeft == -1)
                        {
                            missed++;
                            usedPointList.Remove(new Point(x, y));
                            delete.Add(item.letter);
                            //Letters.Remove(item.letter);
                            missBar.Text = $"Пропущено: {missed}";
                        }
                    }
                    foreach (char item in delete) Letters.Remove(item);
                }

                if (left == 0 && Letters.Count == 0)
                {
                    End = true;

                    new ConsoleText(Console.WindowWidth - rightAreaWidth, 8, Console.WindowWidth - 3, 9, $"Нажмите на пробел, чтобы продолжить", center: true);

                    return;
                }
                
                //helper.mb(Letters.Aggregate("", (acc, key) => acc += key.letter.ToString()));
                Thread.Sleep(interval);
            }
        }

        void WriteLetter(char letter, int x, int y, int state = 4)
        {
            lock (G.consoleLock)
            {
                switch (state)
                {
                    case 3: Console.ForegroundColor = ConsoleColor.Green; break;
                    case 2: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case 1: Console.ForegroundColor = ConsoleColor.Red; break;
                }

                int x_ = points[y, x].X, y_ = points[y, x].Y;
                // helper.mb($"{y}: {y_} X {x}: {x_}");
                Console.SetCursorPosition(x_, y_);
                Console.Write(state == 0 ? ' ' : letter);
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x_ - 1, y_ - 1); Console.Write(state >= 4 ? "/" : " ");
                Console.SetCursorPosition(x_ + 1, y_ - 1); Console.Write(state >= 3 ? "\\" : " ");
                Console.SetCursorPosition(x_ + 1, y_ + 1); Console.Write(state >= 2 ? "/" : " ");
                Console.SetCursorPosition(x_ - 1, y_ + 1); Console.Write(state >= 1 ? "\\" : " ");
            }
        }
    }
}
/*/ \  _
   x  |x|
  \ /  -*/