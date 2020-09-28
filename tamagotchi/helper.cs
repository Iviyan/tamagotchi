using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace tamagotchi
{
    public static class helper
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public static int MessageBox(string msg, string caption, int type) => MessageBox(IntPtr.Zero, msg, caption, type);
        public static int MessageBox(string msg, string caption) => MessageBox(IntPtr.Zero, msg, caption, 0);
        public static int mb(params object[] msg) => MessageBox(IntPtr.Zero, msg.Aggregate("", (string acc, object str) => acc += str.ToString()), "", 0);

        public static double read(string s)
        {
            Console.Write(s);
            return Convert.ToDouble(Console.ReadLine());
        }
        public static int ask(string text, int default_ = 0)
        {
            Console.Write(text);
            string inp = Console.ReadLine();
            int res;
            if (!int.TryParse(inp, out res) && default_ != 0) res = default_;
            return res;
        }
        public static int Choice(string[] list)
        {
            int select = 0;
            int pos = Console.CursorTop;
            int posEnd = pos + list.Length - 1;
            Console.Write(list.Aggregate("> ", (acc, str) => acc += str + '\n'));
            Console.SetCursorPosition(0, pos);
            ConsoleKeyInfo info;

            while (true)
            {
                info = Console.ReadKey();
                Console.CursorLeft = 0;

                if (info.Key == ConsoleKey.DownArrow)
                {
                    Console.Write($"{list[select]}  "); Console.CursorLeft = 0;
                    if (Console.CursorTop < posEnd)
                    {
                        select++;
                        Console.CursorTop += 1;
                    } else
                    {
                        select = 0;
                        Console.CursorTop = pos;
                    }
                    Console.Write($"> {list[select]}");

                } else if (info.Key == ConsoleKey.UpArrow)
                {
                    Console.Write($"{list[select]}  "); Console.CursorLeft = 0;
                    if (Console.CursorTop > pos)
                    {
                        select--;
                        Console.CursorTop -= 1;

                    } else
                    {
                        select = list.Length - 1;
                        Console.CursorTop = posEnd;
                    }
                    Console.Write($"> {list[select]}");

                } else if (info.Key == ConsoleKey.Enter) { Console.CursorTop = posEnd + 1; return select; }
            }
        }
        public static string Choice_str(string[] list)
        {
            int c = Choice(list);
            return list[c];
        }

        public static string mul(string str, int count)
        {
            string ret = "";
            for (int i = 0; i < count; i++) ret += str;
            return ret;
        }

        public static string[] split(string str, int len)
        {
            if (str.Length <= len) return new string[] { str };

            List<string> res = new List<string>();
            string temp = "";
            for (int i = 0; i < str.Length; i++) {
                temp += str[i];
                if (temp.Length == len)
                {
                    res.Add(temp);
                    temp = "";
                }
            }
            if (temp != "") res.Add(temp);
            return res.ToArray();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
