using System;
using System.Runtime.InteropServices;

namespace TbAutoHideSwitcher
{
    internal static class Program
    {
        // Import the SHAppBarMessage call from shell32.dll
        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        private static extern AppBarState SHAppBarMessage(AppBarMessage dwMessage, ref APPBARDATA pData);

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Initiate the cbSize member (the only required parameter)
            // and get the current Taskbar state
            APPBARDATA abData = new APPBARDATA() { cbSize = Marshal.SizeOf(typeof(APPBARDATA)) };
            AppBarState currentState = SHAppBarMessage(AppBarMessage.GetState, ref abData);

            // Toggle the auto-hide state of the Taskbar and set it
            // NOTE: Starting with Windows 7 ABS_ALWAYSONTOP is no longer returned,
            // so we set it regardless of its current state
            abData.lParam = (IntPtr) (AppBarState.AlwaysOnTop | currentState ^ AppBarState.AutoHide);
            SHAppBarMessage(AppBarMessage.SetState, ref abData);
        }

        private enum AppBarMessage : uint
        {
            GetState = 0x4,
            SetState = 0xA
        }

        [Flags]
        private enum AppBarState : uint
        {
            AutoHide = 1,
            AlwaysOnTop = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int    cbSize;
            public IntPtr hWnd;
            public uint   uCallbackMessage;
            public uint   uEdge;
            public RECT   rc;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
