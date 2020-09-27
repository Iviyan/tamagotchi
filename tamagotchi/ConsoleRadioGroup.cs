using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    public class ConsoleRadioGroup
    {
        public Point p1;
        //public Point p2;
        public int width;
        public int height;
        private List<string> choices;
        public List<string> Choices
        {
            get => choices;
            set
            {
                choices = value;
                Write();
            }
        }
        int interval;
        public ConsoleRadioGroup(Point p1, List<string> choices, int interval = 0, int width = 0, int height = 0)
        {
            this.p1 = p1;
            this.choices = choices;
            this.width = width;
            this.height = height;
            this.interval = interval;
            Write();
        }
        public ConsoleRadioGroup(int x1, int y1, List<string> choices, int interval = 0, int width = 0, int height = 0) : this(new Point(x1, y1), choices, interval, width, height) {}

        public void SetArea(Point p1, int width = 0, int height = 0, bool rewrite = true) { this.p1 = p1; this.width = width; this.height = height; if (rewrite) Write(); }
        public void SetArea(int x1, int y1, int width = 0, int height = 0, bool rewrite = true) => SetArea(new Point(x1, y1), width, height, rewrite);

        public void Write()
        {
            lock (G.consoleLock)
            {
                int width_ = (width == 0) ? Console.WindowWidth : width;
                int maxLength = width_ - 2;

                int line = 0;
                for (int h = 0; h < choices.Count; h++)
                {
                    string[] lines = helper.split(choices[h], maxLength);

                    selHelper.Add(h, new SelHelper(line, lines.Length > 1 ? width_ : lines[0].Length, lines.Length));

                    for (int j = 0; j < lines.Length; j++)
                    {
                        if (height > 0 && line >= height) return;
                        Console.SetCursorPosition(p1.X + 1, p1.Y + line);
                        Console.Write(lines[j]);

                        line++;
                    }
                    line += interval;
                }
            }
        }

        struct SelHelper {
            public int top, width, height;
            public SelHelper(int top, int width, int height) { this.top = top; this.width = width; this.height = height; }
        }
        Dictionary<int, SelHelper> selHelper = new Dictionary<int, SelHelper>();

        public int Choice()
        {
            int count = choices.Count;
            int lastChoice = 0;
            int choice = 0;

            void select_(int sel, char cStart = '>', char cEnd = '<')
            {
                SelHelper sh = selHelper[sel]; //helper.mb(sh.top, " ", sh.width, " ", sh.height);
                for (int i = 0; i < sh.height; i++)
                {
                    lock (G.consoleLock)
                    {
                        Console.SetCursorPosition(p1.X, p1.Y + sh.top);
                        Console.Write(cStart);
                        Console.SetCursorPosition(p1.X + 1 + sh.width, p1.Y + sh.top);
                        Console.Write(cEnd);
                    }
                }
            }
            void select()
            {
                select_(lastChoice, ' ', ' ');
                select_(choice);
                lastChoice = choice;
            }
            select();

            ConsoleKeyInfo info;
            while (true)
            {
                info = Console.ReadKey(true);

                if (info.Key == ConsoleKey.DownArrow)
                {
                    if (choice + 1 < count) choice++;
                    select();
                }
                else if (info.Key == ConsoleKey.UpArrow)
                {
                    if (choice > 0) choice--;
                    select();
                }
                else if (info.Key == ConsoleKey.Enter) { return choice; }
            }
        }

    }
}
