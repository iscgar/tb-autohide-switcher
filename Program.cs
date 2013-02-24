using System;
using System.Runtime.InteropServices;

namespace TbAutoHideSwitcher
{
    internal static class Program
    {
        private const int ABS_ALWAYSONTOP = 0x02;
        private const int ABS_AUTOHIDE = 0x01;
        private const int ABM_GETSTATE = 0x04;
        private const int ABM_SETSTATE = 0xA;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AutoHideAutoSwitch();
        }

        // Import the SHAppBarMessage call from shell32.dll
        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        private static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        private static void AutoHideAutoSwitch()
        {
            APPBARDATA abData = new APPBARDATA();
            // Initiate the cbSize member (which is a required parameter)
            abData.cbSize = Marshal.SizeOf(abData);
            // Find out what should be the new state of the Taskbar
            int tbNewState = SwitchTaskbarVisibilityState(abData);
            // Set the lParam to the new state
            abData.lParam = (IntPtr) tbNewState;
            // Call SHAppBarMessage with ABM_SETSTATE to set the new Taskbar state
            SHAppBarMessage(ABM_SETSTATE, ref abData);
        }

        private static int SwitchTaskbarVisibilityState(APPBARDATA pData)
        {
            int tbCurrentState = (int) SHAppBarMessage(ABM_GETSTATE, ref pData);
            int tbNewState = ABS_ALWAYSONTOP;
            /* Since ABM_GETSTATE can return both, either, or neither of ABS_ALWAYSONTOP and ABS_AUTOHIDE,
            * we should check if the returned value is even (which means that it doesn't include ABS_AUTOHIDE).
            * NOTE: Starting with Windows 7, ABS_ALWAYSONTOP is no longer returned */
            if (tbCurrentState % ABS_ALWAYSONTOP == 0)
                tbNewState += ABS_AUTOHIDE;
            return tbNewState;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}