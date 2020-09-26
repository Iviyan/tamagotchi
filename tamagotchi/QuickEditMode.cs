using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static tamagotchi.helper;

namespace tamagotchi
{
    public static class QuickEditMode
    {
        //https://stackoverflow.com/questions/39250218/code-stops-executing-when-a-user-clicks-on-the-console-window

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow(); //not work

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(
            IntPtr hConsoleHandle,
            out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(
            IntPtr hConsoleHandle,
            int ioMode);

        public enum StdHandle : int
        {
            STD_INPUT_HANDLE = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE = -12,
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// This flag enables the user to use the mouse to select and edit text. To enable
        /// this option, you must also set the ExtendedFlags flag.
        /// </summary>
        const int quickEditMode = 64;

        // ExtendedFlags must be combined with
        // InsertMode and QuickEditMode when setting
        /// <summary>
        /// ExtendedFlags must be enabled in order to enable InsertMode or QuickEditMode.
        /// </summary>
        const int extendedFlags = 128;

        public static void DisableQuickEdit()
        {
            IntPtr conHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);// GetConsoleWindow();
            int mode;

            if (!GetConsoleMode(conHandle, out mode))
            {
                // error getting the console mode. Exit.
                mb("error GetConsoleMode");
                return;
            }

            mode = mode & ~(quickEditMode | extendedFlags);

            if (!SetConsoleMode(conHandle, mode))
            {
                // error setting console mode.
                mb("error SetConsoleMode");
            }
        }

        public static void EnableQuickEdit()
        {
            IntPtr conHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);// GetConsoleWindow();
            int mode;

            if (!GetConsoleMode(conHandle, out mode))
            {
                // error getting the console mode. Exit.
                mb("error GetConsoleMode");
                return;
            }

            mode = mode | (quickEditMode | extendedFlags);

            if (!SetConsoleMode(conHandle, mode))
            {
                // error setting console mode.
                mb("error SetConsoleMode");
            }
        }
    }
}
