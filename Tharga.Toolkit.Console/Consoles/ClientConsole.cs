using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ClientConsole : ConsoleBase
    {
        private readonly IConsoleConfiguration _consoleConfiguration;
        private bool _topMost;

        public ClientConsole(IConsoleConfiguration consoleConfiguration = null)
            : base(new ConsoleManager(System.Console.Out, System.Console.In))
        {
            _consoleConfiguration = consoleConfiguration ?? new ConsoleConfiguration();

            SetLocation();
            SetTopMost(_consoleConfiguration.TopMost);
            SetColor();
            UpdateTitle();
            ShowSplashScreen();
            ShowAssemblyInfo();
        }

        public bool TopMost
        {
            get { return _topMost; }
            set { SetTopMost(value); }
        }

        //TODO: Make it possible to set what screen the window should be placed on
        private void SetLocation()
        {
            if (_consoleConfiguration.StartLocation == null) return;

            var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(hWnd, new IntPtr(0), _consoleConfiguration.StartLocation.Left, _consoleConfiguration.StartLocation.Top, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

            //TODO: Store the last used location
            //RECT rct;
            //GetWindowRect(hWnd, out rct);
            //Console.WriteLine(rct.Left.ToString(), OutputLevel.Warning);
        }

        //TODO: Move to Console Manager
        private void SetColor()
        {
            if (System.Console.BackgroundColor == _consoleConfiguration.BackgroundColor && System.Console.ForegroundColor == _consoleConfiguration.DefaultTextColor) return;

            System.Console.BackgroundColor = _consoleConfiguration.BackgroundColor;
            System.Console.ForegroundColor = _consoleConfiguration.DefaultTextColor;
            System.Console.Clear();
        }

        //TODO: Move to Console Manager
        private void UpdateTitle()
        {
            try
            {
                System.Console.Title = _consoleConfiguration.Title ?? AssemblyHelper.GetAssemblyInfo() ?? "Tharga Console";
            }
            catch (IOException exception)
            {
                Trace.TraceError($"Cannot set console title. {exception.Message}");
            }
        }

        private void ShowSplashScreen()
        {
            if (string.IsNullOrEmpty(_consoleConfiguration.SplashScreen))
                return;

            Output(new WriteEventArgs(_consoleConfiguration.SplashScreen, OutputLevel.Default));
        }

        private void ShowAssemblyInfo()
        {
            if (_consoleConfiguration.ShowAssemblyInfo)
            {
                var info = AssemblyHelper.GetAssemblyInfo();
                if (!string.IsNullOrEmpty(info))
                {
                    Output(new WriteEventArgs(info, OutputLevel.Default));
                }
            }
        }

        private void SetTopMost(bool value)
        {
            if (value == _topMost) return;
            _topMost = value;

            var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            if (_consoleConfiguration.TopMost)
            {
                SetWindowPos(hWnd, new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            else
            {
                SetWindowPos(hWnd, new IntPtr(HWND_NOTOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        #region User32

        private const int HWND_TOPMOST = -1;
        private static readonly int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const short SWP_NOZORDER = 0X4;
        private const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }

        //private const int HWND_TOPMOST = -1;
        //private const int SWP_NOMOVE = 0x0002;
        //private const int SWP_NOSIZE = 0x0001;

        #endregion
    }
}