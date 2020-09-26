using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi
{
    class ConsoleAPI
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int ioMode);

        public enum StdHandle : int
        {
            STD_INPUT_HANDLE = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE = -12,
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        //https://stackoverflow.com/questions/39250218/code-stops-executing-when-a-user-clicks-on-the-console-window

        const int quickEditMode = 64;
        const int extendedFlags = 128;

        public static void SetQuickEdit(bool enable)
        {
            IntPtr conHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);
            int mode;

            if (!GetConsoleMode(conHandle, out mode))
            {
                helper.mb("error GetConsoleMode");
                return;
            }

            if (enable)
                mode = mode | (quickEditMode | extendedFlags);
            else
                mode = mode & ~(quickEditMode | extendedFlags);

            if (!SetConsoleMode(conHandle, mode))
            {
                helper.mb("error SetConsoleMode");
            }
        }


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



        public const int MF_BYCOMMAND = 0x00000000;
        public const int MF_GRAYED = 0x00000001;
        public const int MF_DISABLED = 0x00000002;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern bool DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);
        [DllImport("user32.dll")]
        public static extern bool InsertMenuA(IntPtr hMenu, int uPosition, int uFlags, IntPtr uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        static void CanResize(bool enable)
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                //DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                if (enable)
                {
                    //InsertMenuA(sysMenu, SC_SIZE, MF_BYCOMMAND, new IntPtr(SC_SIZE), "Размер");
                } else {
                    //DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                    //DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                   // DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
                    /*EnableMenuItem(sysMenu, SC_SIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
                    EnableMenuItem(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
                    EnableMenuItem(sysMenu, SC_MINIMIZE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);*/
                }
            }
        }


    }
}
