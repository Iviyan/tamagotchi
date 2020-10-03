using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    public static class G
    {
        public static readonly object consoleLock = new object();
        public static Random rnd = new Random();
    }
}
