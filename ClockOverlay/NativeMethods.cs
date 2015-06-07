using System;
using System.Runtime.InteropServices;

namespace ClockOverlay
{
    /// <summary>
    ///     The methods within this class all relate to or wrap functions from within the winAPI.
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        ///     Hit test transparency.
        /// </summary>
        public const int WsExTransparent = 0x00000020;

        /// <summary>
        ///     Sets a new extended window style.
        /// </summary>
        public const int GwlExstyle = (-20);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd,
        int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd,
        int index, int newStyle);

        /// <summary>
        ///     
        /// </summary>
        /// <param name="hwnd"></param>
        internal static void MakeTransparent(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GwlExstyle);
            SetWindowLong(hwnd, GwlExstyle, extendedStyle |
            WsExTransparent);
        }

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
       
        /// <summary>
        ///     The structure of a window's four corners.
        /// </summary>
        public struct Rect
        {
            /// <summary>
            ///     The left point..
            /// </summary>
            public int Left { get; set; }
            /// <summary>
            ///     The top point..
            /// </summary>
            public int Top { get; set; }
            /// <summary>
            ///     The right point..
            /// </summary>
            public int Right { get; set; }
            /// <summary>
            ///     The bottom point..
            /// </summary>
            public int Bottom { get; set; }
        }
    }
}
