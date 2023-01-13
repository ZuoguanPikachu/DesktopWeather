using System;
using System.Windows;

namespace DesktopWeather.Util
{
    internal static class SetWindowToDesktop
    {
        public static IntPtr sendMsgToProgman()
        {
            IntPtr programHandle = Win32Func.FindWindow("Progman", null);
            Win32Func.SendMessageTimeout(programHandle, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 2, IntPtr.Zero);

            return programHandle;
        }

        public static void setWindowToWorkerW1(Window window)
        {
            sendMsgToProgman();

            Win32Func.EnumWindows((hwnd, lParam) =>
            {
                if (Win32Func.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                {
                    Win32Func.SetParent(new System.Windows.Interop.WindowInteropHelper(window).Handle, hwnd);
                }
                return true;
            }, IntPtr.Zero);
        }

        public static void setWindowToPM(Window window)
        {
            IntPtr programHandle = sendMsgToProgman();

            Win32Func.EnumWindows((hwnd, lParam) =>
            {
                if (Win32Func.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                {

                    IntPtr tempHwnd = Win32Func.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                    Win32Func.ShowWindow(tempHwnd, 0);
                }
                return true;
            }, IntPtr.Zero);

            Win32Func.SetParent(new System.Windows.Interop.WindowInteropHelper(window).Handle, programHandle);
        }

        public static void MoveWindow(Window window, int x, int y)
        {
            if (x < 0)
            {
                int screenWidth = (int)SystemParameters.WorkArea.Width;
                int windowWidth = (int)window.Width;

                x = screenWidth - windowWidth + x;
            }

            if (y < 0)
            {
                int screenHeight = (int)SystemParameters.WorkArea.Height;
                int windowHeight = (int)window.Height;

                y = screenHeight - windowHeight + y;
            }

            Win32Func.MoveWindow(new System.Windows.Interop.WindowInteropHelper(window).Handle, x, y, (int)window.Width, (int)window.Height, true);
        }
    }
}
