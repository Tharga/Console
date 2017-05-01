using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tharga.Toolkit.Console.Consoles.Base;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ClientConsole : ConsoleBase
    {
        #region User32

        private bool _topMost;
        private const int HWND_TOPMOST = -1;
        static readonly int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        #endregion

        public bool TopMost
        {
            get { return _topMost; }
            set
            {
                SetTopMost(value);
                _topMost = value;
            }
        }

        private void SetTopMost(bool value)
        {
            var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            if (value)
            {
                SetWindowPos(hWnd, new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            else
            {
                SetWindowPos(hWnd, new IntPtr(HWND_NOTOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        public ClientConsole()
            : base(System.Console.Out)
        {
        }
    }
}