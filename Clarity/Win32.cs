using System;
using System.Runtime.InteropServices;

namespace Clarity
{
    internal class Win32
    {
        #region Constants

        public const UInt32 WM_LBUTTONDBLCLK = 0x0203;
        public const UInt32 WM_LBUTTONDOWN = 0x0201;
        public const UInt32 WM_LBUTTONUP = 0x0202;
        public const UInt32 WM_NCCALCSIZE = 0x0083;
        public const UInt32 WM_WINDOWPOSCHANGING = 0x0046;
        public const UInt32 WS_POPUP = 0x80000000;
        public const Int32 WS_CLIPCHILDREN = 0x02000000;
        public const Int32 WS_CLIPSIBLINGS = 0x04000000;
        public const Int32 WS_EX_LAYERED = 0x80000;
        public const Int32 WS_EX_TOOLWINDOW = 0x80;

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }
        public struct RECT { public int Left, Top, Right, Bottom; }
        public struct NCCALCSIZE_PARAMS
        {
            public RECT rcNewWindow;
            public RECT rcOldWindow;
            public RECT rcClient;
            IntPtr lppos;
        }

        #region Methods

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean BitBlt(IntPtr hdc, Int32 nXDest, Int32 nYDest, Int32 nWidth, Int32 nHeight, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, UInt32 dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}