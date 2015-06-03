using System;
using System.Runtime.InteropServices;

namespace ClockOverlay
{
    public static class NativeMethods
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd,
        int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd,
        int index, int newStyle);

        public static void makeTransparent(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle |
            WS_EX_TRANSPARENT);
        }

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
    }
}
