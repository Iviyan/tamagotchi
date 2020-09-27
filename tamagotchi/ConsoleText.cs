using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    public class ConsoleText
    {
        public Point p1;
        public Point p2;
        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                write();
            }
        }
        public bool center;
        public ConsoleText(Point p1, Point p2, string text = "", bool center = false)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.text = text;
            this.center = center;
            write();
        }
        public ConsoleText(int x1, int y1, int x2, int y2, string text = "", bool center = false) : this(new Point(x1, y1), new Point(x2, y2), text, center) {}

        public void SetArea(Point p1, Point p2, bool rewrite = true) { this.p1 = p1; this.p2 = p2; if (rewrite) write(); }
        public void SetArea(int x1, int y1, int x2, int y2, bool rewrite = true) => SetArea(new Point(x1, y1), new Point(x2, y2), rewrite);

        public void write()
        {
            int width = p2.X - p1.X + 1;
            int height = p2.Y - p1.Y + 1;
            int length = width * height;
            string text_ = text; 
            if (text_.Length > length) text_ = text_.Substring(0, length);
            else if (text_.Length < length)
            {
                if (center)
                {
                    int mod = text_.Length % width; //длина последней строки
                    int div = text_.Length / width; 
                    if (mod > 0) text_ = (div > 0 ? text_.Substring(0, width * div) : "") + helper.mul(" ", (width - mod) / 2) + text_.Substring(width * div, mod);

                }
                text_ += helper.mul(" ", length - text_.Length);
            }
            lock (G.consoleLock)
            {
                for (int h = 0; h < height; h++)
                {
                    Console.SetCursorPosition(p1.X, p1.Y + h);
                    Console.Write(text_.Substring(width * h, width));
                }
            }
        }

    }
}
