using System;
using System.Collections.Generic;
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
        List<Key> Letters = new List<Key>();
        HashSet<char> letterList = new HashSet<char>();

        void AddLetter(char letter, int x, int y)
        {
            Letters.Add(new Key(letter, x, y));
            letterList.Add(letter);
            WriteLetter(letter, x, y);
        }

        public GamePressKey(int interval = 500, int count = 30)
        {
            this.interval = interval;
            this.count = count;
        }

        public int start()
        {
            Console.Clear();

            int WWidth = Console.WindowWidth;
            int WHeight = Console.WindowHeight;

            int dist = 1;
            int Wpoints = (WWidth + 1) / 2;
            int Hpoints = (WHeight + 1) / 2;

            int press = 0,
                missed = 0,
                misprints = 0;

            Thread t = new Thread(loop);

            Console.SetCursorPosition(0, 0);
            Console.Write(new string('#', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(new string('#', Console.WindowWidth));
            for (int i = 1; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("#");
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.Write("#");
            }
            Console.SetCursorPosition(0,0);

            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();
                ConsoleKey.
            }
            
            return 1;
        }

        void loop()
        {
            for (; ;)
            {


                Thread.Sleep(interval);
            }
        }

        void WriteLetter(char letter, int x, int y, int state = 4)
        {
            switch (state)
            {
                case 3: Console.ForegroundColor = ConsoleColor.Yellow; break;
                case 2: Console.ForegroundColor = ConsoleColor.Red; break;
                case 1: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
            }

            //Console.SetCursorPosition()
        }
    }
}
/*/ \  _
   x  |x|
  \ /  -*/