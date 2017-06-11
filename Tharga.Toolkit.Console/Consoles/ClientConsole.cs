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

            Initiate();
        }

        private void Initiate()
        {
            ConsoleManager.Clear();
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
            var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            Position position = null;

            if (_consoleConfiguration.StartPosition != null)
            {
                position = _consoleConfiguration.StartPosition;
            }
            else if (_consoleConfiguration.RememberStartLocation)
            {
                position = GetPosition();
                SubscribeToWindowMovement(hWnd);
            }

            SetWidth(position);
            SetHeight(position);

            if (position != null)
            {
                SetWindowPos(hWnd, new IntPtr(0), position.Left, position.Top, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            }
        }

        private Position GetPosition()
        {
            try
            {
                var val = Registry.GetSetting("StartPosition", Registry.RegistryHKey.CurrentUser, string.Empty);
                if (string.IsNullOrEmpty(val)) return null;
                var segments = val.Split('|');
                var pos = segments[0].Split(':');
                var wz = segments[1].Split(':');
                var bz = segments[2].Split(':');
                return new Position(int.Parse(pos[0]), int.Parse(pos[1]), int.Parse(wz[0]), int.Parse(wz[1]), int.Parse(bz[0]), int.Parse(bz[1]));
            }
            catch (Exception exception)
            {
                OutputError(exception);
                return null;
            }
        }

        private void SetWidth(Position position)
        {
            try
            {
                if (position == null) return;

                if (position.Width != null)
                    ConsoleManager.WindowWidth = position.Width.Value;

                if (position.BufferWidth != null)
                    ConsoleManager.BufferWidth = position.BufferWidth.Value;
                else if (ConsoleManager.WindowWidth > 0)
                    ConsoleManager.BufferWidth = ConsoleManager.WindowWidth;
            }
            catch (Exception exception)
            {
                OutputError(exception);
            }
        }

        private void SetHeight(Position position)
        {
            try
            {
                if (position == null) return;

                if (position.Height != null)
                    ConsoleManager.WindowHeight = position.Height.Value;

                if (position.BufferHeight != null)
                    ConsoleManager.BufferHeight = position.BufferHeight.Value;
            }
            catch (Exception exception)
            {
                OutputError(exception);
            }
        }

        private void SetColor()
        {
            if (ConsoleManager.BackgroundColor == _consoleConfiguration.BackgroundColor && ConsoleManager.ForegroundColor == _consoleConfiguration.DefaultTextColor) return;

            ConsoleManager.BackgroundColor = _consoleConfiguration.BackgroundColor;
            ConsoleManager.ForegroundColor = _consoleConfiguration.DefaultTextColor;
            ConsoleManager.Clear();
        }

        private void UpdateTitle()
        {
            try
            {
                ConsoleManager.Title = _consoleConfiguration.Title ?? AssemblyHelper.GetAssemblyInfo() ?? "Tharga Console";
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

            Output(new WriteEventArgs(_consoleConfiguration.SplashScreen));
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

        protected internal override void Reset()
        {
            Registry.ClearAllSettings();
            Initiate();
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

        //TODO: Move to Console Manager
        private void WhenWindowMoveStartsOrEnds(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (hwnd != _target)
                    return;

                if (eventType == 10) //Movement started
                {
                }
                else if (eventType == 11) //Movement ended
                {
                    RECT rct;
                    GetWindowRect(hwnd, out rct);

                    //TODO: Find out what screen we are on (when using multiple screens)

                    var val = $"{rct.Left}:{rct.Top}|{ConsoleManager.WindowWidth}:{ConsoleManager.WindowHeight}|{ConsoleManager.BufferWidth}:{ConsoleManager.BufferHeight}";
                    //ConsoleManager.WriteLine(val);

                    Registry.SetSetting("StartPosition", val, Registry.RegistryHKey.CurrentUser);
                }
            }
            catch (Exception exception)
            {
                OutputError(exception);
            }
        }

        #endregion
    }
}