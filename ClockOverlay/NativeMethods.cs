using System;
using System.Runtime.InteropServices;

namespace ClockOverlay
{
    public static class NativeMethods
    {
        public const int WsExTransparent = 0x00000020;
        public const int GwlExstyle = (-20);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd,
        int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd,
        int index, int newStyle);

        public static void MakeTransparent(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GwlExstyle);
            SetWindowLong(hwnd, GwlExstyle, extendedStyle |
            WsExTransparent);
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
