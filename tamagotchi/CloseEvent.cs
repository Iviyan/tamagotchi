using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    public static class CloseEvent
    {
        //https://docs.microsoft.com/en-us/windows/console/handlerroutine?redirectedfrom=MSDN

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handler, bool add);

        public delegate bool ConsoleEventDelegate(CtrlType sig);
        public static ConsoleEventDelegate ConsoleEventHandler;

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        public static bool ConsoleEventCallback(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    return false;
            }
        }
    }
}
