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
        public int width;
        public int Width { get => (width == 0) ? Console.WindowWidth : width; }
        public int height;
        public int currentHeight = 0;
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
        private HashSet<int> disabled = new HashSet<int>();
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

        public void Write(int startIndex = 0)
        {
            lock (G.consoleLock)
            {
                int maxLength = Width - 2;

                int line = selHelper.ContainsKey(startIndex) ? selHelper[startIndex].top : 0;
                
                for (int h = startIndex; h < choices.Count; h++)
                {
                    string[] lines = helper.split(choices[h], maxLength);
                    int lineHeight = lines.Length;
                    if (height > 0 && line + lineHeight - 1 >= height) lineHeight = height - line;

                    selHelper[h] =
                        new SelHelper(
                            line,
                            lines.Length > 1 ? Width - 2 : lines[0].Length,
                            lineHeight
                        );
                    ConsoleHelper.ClearArea(p1.X, p1.Y + line, p1.X + width - 1, p1.Y + line + lineHeight + interval);

                    for (int j = 0; j < lineHeight; j++)
                    {
                        Console.SetCursorPosition(p1.X + 1, p1.Y + line);
                        bool disable = disabled.Contains(h);
                        if (disable) Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(lines[j]);
                        if (disable) Console.ForegroundColor = ConsoleColor.White;
                        
                        line++;
                    }
                    line += interval;
                }
                currentHeight = line - interval;
            }
        }

        public void Edit(int index, string newText, bool enable = false)
        {
            if (newText == choices[index] && enable == false) return;
            lock (G.consoleLock)
            {
                SelHelper sh = selHelper[index];
                choices[index] = newText;

                int maxLength = Width - 2;
                string[] lines = helper.split(newText, maxLength);

                if (sh.height == lines.Length || index + 1 == choices.Count)
                {
                    ConsoleHelper.ClearArea(p1.X, p1.Y + sh.top, p1.X + width - 1, p1.Y + sh.top + sh.height - 1);
                    for (int i = 0; i < sh.height; i++)
                    {
                        Console.SetCursorPosition(p1.X + 1, p1.Y + sh.top + i);
                        bool disable = disabled.Contains(index);
                        if (disable) Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(lines[i]);
                        if (disable) Console.ForegroundColor = ConsoleColor.White;
                        
                    }
                } else
                {
                    Write(index);
                }
                selHelper[index].width = lines.Length > 1 ? Width - 2 : lines[0].Length;
            }
            select();
        }

        class SelHelper {
            public int top, width, height;
            public SelHelper(int top, int width, int height) { this.top = top; this.width = width; this.height = height; }
        }
        Dictionary<int, SelHelper> selHelper = new Dictionary<int, SelHelper>();

        public void select(int sel, char cStart = '>', char cEnd = '<')
        {
            SelHelper sh = selHelper[sel];
            for (int i = 0; i < sh.height; i++)
            {
                lock (G.consoleLock)
                {
                    Console.SetCursorPosition(p1.X, p1.Y + sh.top + i);
                    Console.Write(cStart);
                    Console.SetCursorPosition(p1.X + 1 + sh.width, p1.Y + sh.top + i);
                    Console.Write(cEnd);
                }
            }
        }

        int lastChoice = 0;
        int choice = 0;
        public void select()
        {
            select(lastChoice, ' ', ' ');
            select(choice);
            lastChoice = choice;
        }

        public void Disable(int index)
        {
            if (!disabled.Contains(index))
            {
                disabled.Add(index);
                Edit(index, choices[index], enable: true);
            }
        }
        public void Enable(int index)
        {
            if (disabled.Contains(index))
            {
                disabled.Remove(index);
                Edit(index, choices[index], enable: true);
            }
        }

        public int Choice(int selectIndex = 0)
        {
            int count = choices.Count;
            
            choice = selectIndex;

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
                else if (info.Key == ConsoleKey.Enter && !disabled.Contains(choice)) { return choice; }
            }
        }

    }
}
