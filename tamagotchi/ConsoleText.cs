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

        public void SetArea(Point p1, Point p2) { this.p1 = p1; this.p2 = p2; }
        public void SetArea(int x1, int y1, int x2, int y2) => SetArea(new Point(x1, y1), new Point(x2, y2));

        public void write()
        {
            int width = p2.X - p1.X + 1;
            int height = p2.Y - p1.Y + 1;
            int length = width * height;
            string text_ = text; //helper.mb($"|{text_}| ", text_.Length);
            if (text_.Length > length) text_ = text_.Substring(0, length);
            else if (text_.Length < length)
            {
                if (center)
                {
                    int mod = text_.Length % width; //длина последней строки
                    int div = text_.Length / width; //helper.mb(text_.Length, " : ", width * (div + 1) - 1);
                    if (mod > 0)
                    {
                        //helper.mb($"{div} x {mod}");
                        //helper.mb($"|{helper.mul(" ", width / 2 - mod + ((width % 2 == 1) ? 1 : 0))}| ", width / 2 - mod + ((width % 2 == 1) ? 1 : 0));
                        text_ = (div > 0 ? text_.Substring(0, width * div) : "") + helper.mul(" ", (width - mod) / 2) + text_.Substring(width * div, mod);
                    }

                } //else
                
                //helper.mb($"|{helper.mul(" ", length - text_.Length)}| ", length - text_.Length);
                //helper.mb($"|{text_}| ", text_.Length);
                text_ += helper.mul(" ", length - text_.Length);
                
            }
            //helper.mb($"|{text_}| ", text_.Length);
            for (int h = 0; h < height; h++)
            {
                Console.SetCursorPosition(p1.X, p1.Y + h); //helper.mb(text_.Length, ": ", width * h, " ", width); 
                Console.Write(text_.Substring(width * h, width));
            }
        }

    }
}
