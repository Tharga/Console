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
            if (_consoleConfiguration.StartPosition != null)
            {
                SetWidth();
                SetHeight();

                var hWnd = Process.GetCurrentProcess().MainWindowHandle;
                SetWindowPos(hWnd, new IntPtr(0), _consoleConfiguration.StartPosition.Left, _consoleConfiguration.StartPosition.Top, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            }
            else if (_consoleConfiguration.RememberStartLocation)
            {
                var hWnd = Process.GetCurrentProcess().MainWindowHandle;
                SubscribeToWindowMovement(hWnd);

                //TODO: Read last start position from registry
                //TODO: Position the window
            }
        }

        private void SetWidth()
        {
            try
            {
                if (_consoleConfiguration.StartPosition.Width != null)
                    System.Console.WindowWidth = _consoleConfiguration.StartPosition.Width.Value;

                if (_consoleConfiguration.StartPosition.BufferWidth != null)
                    System.Console.BufferWidth = _consoleConfiguration.StartPosition.BufferWidth.Value;
                else
                    System.Console.BufferWidth = System.Console.WindowWidth;
            }
            catch (Exception exception)
            {
                OutputError(exception);
            }
        }

        private void SetHeight()
        {
            try
            {
                if (_consoleConfiguration.StartPosition.Height != null)
                    System.Console.WindowHeight = _consoleConfiguration.StartPosition.Height.Value;

                if (_consoleConfiguration.StartPosition.BufferHeight != null)
                    System.Console.BufferHeight = _consoleConfiguration.StartPosition.BufferHeight.Value;
            }
            catch (Exception exception)
            {
                OutputError(exception);
            }
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

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

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
        #region Window movement subscription

        private IntPtr _target;
        private uint _processId, _threadId;
        private WinEventDelegate _winEventDelegate;

        private void SubscribeToWindowMovement(IntPtr hWnd)
        {
            _target = hWnd;

            // 10 = window move start, 11 = window move end, 0 = fire out of context
            _winEventDelegate = WhenWindowMoveStartsOrEnds;
            var hook = SetWinEventHook(10, 11, _target, _winEventDelegate, _processId, _threadId, 0);
        }

        private void WhenWindowMoveStartsOrEnds(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd != _target)
                return;

            if (eventType == 10) //Movement Starts
            {
            }
            else if (eventType == 11)
            {
                RECT rct;
                GetWindowRect(hwnd, out rct);
                //System.Console.WriteLine(rct.Left + ":" + rct.Top + $" [{rct.Right}:{rct.Bottom}]", OutputLevel.Warning);

                var w = System.Console.WindowWidth;
                var h = System.Console.WindowHeight;

                var bw = System.Console.BufferWidth;
                var bh = System.Console.BufferHeight;

                //System.Console.WriteLine($"{rct.Left}:{rct.Top} [{w}:{h}] [{bw}:{bh}]", OutputLevel.Warning);

                //TODO: Find out what screen we are on (when using multiple screens)

                //TODO: Store location in registry
            }
        }

        #endregion
    }
}