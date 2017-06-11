using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
            _initialPosition = GetCurrentPosition();
            Initiate(_consoleConfiguration);
        }

        private void Initiate(IConsoleConfiguration consoleConfiguration)
        {
            SetLocation(consoleConfiguration);
            SetTopMost(consoleConfiguration.TopMost);
            SetColor(consoleConfiguration);
            UpdateTitle(consoleConfiguration);
            ShowSplashScreen(consoleConfiguration);
            ShowAssemblyInfo(consoleConfiguration);
        }

        public bool TopMost
        {
            get { return _topMost; }
            set { SetTopMost(value); }
        }

        private void SetLocation(IConsoleConfiguration consoleConfiguration)
        {
            var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            Position position = null;

            if (consoleConfiguration.StartPosition != null)
            {
                position = consoleConfiguration.StartPosition;
            }
            else if (consoleConfiguration.RememberStartPosition)
            {
                position = GetStoredPosition();
                SubscribeToWindowMovement(hWnd);
            }

            if (position != null)
            {
                SetWidth(position);
                SetHeight(position);

                //TODO: Do not send the window where it cannot be visible. For instance, a secondary screen that is no longer attached.
                var monitors = GetMonitors();
                var monitor = VisibleOnMonitor(monitors, Offset(GetWindowRect(), position));
                if (monitor != null)
                {
                    SetWindowPos(hWnd, IntPtr.Zero, position.Left, position.Top, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                }

                //TODO: This code will reposition the window at startup, the same way as a "scr reset" command will.
                //For some reason the "SetWindowPos" does not act the same when run directly when the console starts as it does when the application has been running for a short while.
                //Task.Run(() =>
                //{
                //    var monitors = GetMonitors();
                //    var monitor = VisibleOnMonitor(monitors, Offset(GetWindowRect(), position));
                //    if (monitor != null)
                //    {
                //        System.Threading.Thread.Sleep(250);
                //        SetWindowPos(hWnd, IntPtr.Zero, position.Left, position.Top, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                //    }
                //});
                //System.Console.WriteLine($"set: {position.Left}:{position.Top} {hWnd}");
            }
        }

        private RECT Offset(RECT rect, Position position)
        {
            return new RECT
            {
                Top = rect.Top + position.Top,
                Left = rect.Left + position.Left,
                Bottom = rect.Bottom + position.Top,
                Right = rect.Right + position.Left
            };
        }

        private int? VisibleOnMonitor(List<RECT> monitors, RECT window)
        {
            var index = 0;    
            foreach (var monitor in monitors)
            {
                if (window.Right >= monitor.Left && (window.Left <= monitor.Right && (window.Bottom >= monitor.Top && window.Top <= monitor.Bottom)))
                {
                    return index;
                }

                index++;
            }

            return null;
        }

        private static List<RECT> GetMonitors()
        {
            var monitors = new List<RECT>();
            MonitorEnumProc callback = (IntPtr hDesktop, IntPtr hdc, ref RECT prect, int d) => { monitors.Add(prect); return true; };
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, 0);
            return monitors;
        }

        private Position GetCurrentPosition()
        {
            var rct = GetWindowRect();

            return new Position(rct.Left, rct.Top, ConsoleManager.WindowWidth, ConsoleManager.WindowHeight, ConsoleManager.BufferWidth, ConsoleManager.BufferHeight);
        }

        private static RECT GetWindowRect()
        {
            var hWnd = Process.GetCurrentProcess().MainWindowHandle;

            RECT rct;
            GetWindowRect(hWnd, out rct);

            //var wp = new WINDOWPLACEMENT();
            //GetWindowPlacement(hWnd, ref wp);
            //System.Console.WriteLine($"get: {wp.rcNormalPosition.Left}:{wp.rcNormalPosition.Top}");

            return rct;
        }

        private Position GetStoredPosition()
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
                if (position.Width != null && position.Width.Value > 0)
                    ConsoleManager.WindowWidth = position.Width.Value;

                if (position.BufferWidth != null)
                    ConsoleManager.BufferWidth = position.BufferWidth.Value;
                else if (ConsoleManager.WindowWidth > 0)
                    ConsoleManager.BufferWidth = ConsoleManager.WindowWidth;
            }
            catch (Exception exception)
            {
                //OutputError(exception);
            }
        }

        private void SetHeight(Position position)
        {
            try
            {
                if (position.Height != null)
                    ConsoleManager.WindowHeight = position.Height.Value;

                if (position.BufferHeight != null)
                    ConsoleManager.BufferHeight = position.BufferHeight.Value;
            }
            catch (Exception exception)
            {
                //OutputError(exception);
            }
        }

        private void SetColor(IConsoleConfiguration consoleConfiguration)
        {
            if (ConsoleManager.BackgroundColor == consoleConfiguration.BackgroundColor && ConsoleManager.ForegroundColor == consoleConfiguration.DefaultTextColor) return;

            ConsoleManager.BackgroundColor = consoleConfiguration.BackgroundColor;
            ConsoleManager.ForegroundColor = consoleConfiguration.DefaultTextColor;
            ConsoleManager.Clear();
        }

        private void UpdateTitle(IConsoleConfiguration consoleConfiguration)
        {
            try
            {
                ConsoleManager.Title = consoleConfiguration.Title ?? AssemblyHelper.GetAssemblyInfo() ?? "Tharga Console";
            }
            catch (IOException exception)
            {
                Trace.TraceError($"Cannot set console title. {exception.Message}");
            }
        }

        private void ShowSplashScreen(IConsoleConfiguration consoleConfiguration)
        {
            if (string.IsNullOrEmpty(consoleConfiguration.SplashScreen))
                return;

            Output(new WriteEventArgs(consoleConfiguration.SplashScreen));
        }

        private void ShowAssemblyInfo(IConsoleConfiguration consoleConfiguration)
        {
            if (consoleConfiguration.ShowAssemblyInfo)
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
            if (value)
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
            ConsoleManager.Clear();

            var consoleConfiguration = new ConsoleConfiguration
            {
                BackgroundColor = _consoleConfiguration.BackgroundColor,
                RememberStartPosition = _consoleConfiguration.RememberStartPosition,
                TopMost = _consoleConfiguration.TopMost,
                SplashScreen = _consoleConfiguration.SplashScreen,
                Title = _consoleConfiguration.Title,
                DefaultTextColor = _consoleConfiguration.DefaultTextColor,
                ShowAssemblyInfo = _consoleConfiguration.ShowAssemblyInfo,
                StartPosition = _consoleConfiguration.StartPosition ?? _initialPosition,
            };

            Initiate(consoleConfiguration);
        }

        #region User32

        private const int HWND_TOPMOST = -1;
        private static readonly int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const short SWP_NOZORDER = 0X4;
        private const int SWP_SHOWWINDOW = 0x0040;

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref RECT pRect, int dwData);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        //[DllImport("user32.dll", SetLastError = true)]
        //static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        [DllImport("user32")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, MonitorEnumProc callback, int dwData);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

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

        [StructLayout(LayoutKind.Sequential)]
        internal struct Pt
        {
            public int X;
            public int Y;
        }

        //internal struct Rc
        //{
        //    public int X;
        //    public int Y;
        //    public int Width;
        //    public int Height;
        //}

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public Pt ptMinPosition;
            public Pt ptMaxPosition;
            public RECT rcNormalPosition;
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        //private const int HWND_TOPMOST = -1;
        //private const int SWP_NOMOVE = 0x0002;
        //private const int SWP_NOSIZE = 0x0001;

        #endregion
        #region  Show and hide console

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        #endregion
        #region Window movement subscription

        private IntPtr _target;
        private uint _processId, _threadId;
        private readonly Position _initialPosition;

        const int EVENT_SYSTEM_MOVESIZESTART = 0x000A; //An MSAA event indicating that a window is being moved or resized.
        const int EVENT_SYSTEM_MOVESIZEEND = 0x000B; //An MSAA event indicating that the movement or resizing of a window is finished.
        const int EVENT_SYSTEM_MINIMIZESTART = 0x0016; //An MSAA event indicating that a window object is about to be minimized or maximized.
        const int EVENT_SYSTEM_MINIMIZEEND = 0x0017; //An MSAA event indicating that a window object was minimized or maximized.
        const int EVENT_SYSTEM_FOREGROUND = 0x0003; //An MSAA event indicating that the foreground window changed.

        /*
Global Const $EVENT_SYSTEM_SOUND = 0x0001 ;An MSAA event indicating that a sound was played.
Global Const $EVENT_SYSTEM_ALERT = 0x0002 ;An MSAA event indicating that an alert was generated.
Global Const $EVENT_SYSTEM_MENUSTART = 0x0004 ;An MSAA event indicating that a menu item on the menu bar was selected.
Global Const $EVENT_SYSTEM_MENUEND = 0x0005 ;An MSAA event indicating that a menu from the menu bar was closed.
Global Const $EVENT_SYSTEM_MENUPOPUPSTART = 0x0006 ;An MSAA event indicating that a pop-up menu was displayed.
Global Const $EVENT_SYSTEM_MENUPOPUPEND = 0x0007 ;An MSAA event indicating that a pop-up menu was closed.
Global Const $EVENT_SYSTEM_CAPTURESTART = 0x0008 ;An MSAA event indicating that a window has received mouse capture.
Global Const $EVENT_SYSTEM_CAPTUREEND = 0x0009 ;An MSAA event indicating that a window has lost mouse capture.
Global Const $EVENT_SYSTEM_DIALOGSTART = 0x0010 ;An MSAA event indicating that a dialog box was displayed.
Global Const $EVENT_SYSTEM_DIALOGEND = 0x0011 ;An MSAA event indicating that a dialog box was closed.
Global Const $EVENT_SYSTEM_SCROLLINGSTART = 0x0012 ;An MSAA event indicating that scrolling has started on a scroll bar.
Global Const $EVENT_SYSTEM_SCROLLINGEND = 0x0013 ;An MSAA event indicating that scrolling has ended on a scroll bar.
Global Const $EVENT_SYSTEM_SWITCHSTART = 0x0014 ;An MSAA event indicating that the user pressed ALT+TAB, which activates the switch window.
Global Const $EVENT_SYSTEM_SWITCHEND = 0x0015 ;An MSAA event indicating that the user released ALT+TAB.
Global Const $EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C ;An MSAA event indicating that a window entered context-sensitive Help mode.
Global Const $EVENT_SYSTEM_CONTEXTHELPEND = 0x000D  ;An MSAA event indicating that a window exited context-sensitive Help mode.
Global Const $EVENT_SYSTEM_DRAGDROPSTART = 0x000E ;An MSAA event indicating that an application is about to enter drag-and-drop mode.
Global Const $EVENT_SYSTEM_DRAGDROPEND = 0x000F ;An MSAA event indicating that an application is about to exit drag-and-drop mode.

; EVENT_OBJECT events are triggered quite often, handle with care...
Global Const $EVENT_OBJECT_CREATE = 0x8000 ;An MSAA event indicating that an object was created.
Global Const $EVENT_OBJECT_DESTROY = 0x8001 ;An MSAA event indicating that an object was destroyed.
Global Const $EVENT_OBJECT_SHOW = 0x8002 ;An MSAA event indicating that a hidden object is being shown.
Global Const $EVENT_OBJECT_HIDE = 0x8003 ;An MSAA event indicating that an object is being hidden.
Global Const $EVENT_OBJECT_REORDER = 0x8004 ;An MSAA event indicating that a container object has added, removed, or reordered its children.
Global Const $EVENT_OBJECT_FOCUS = 0x8005 ;An MSAA event indicating that an object has received the keyboard focus.
Global Const $EVENT_OBJECT_SELECTION = 0x8006 ;An MSAA event indicating that the selection within a container object changed.
Global Const $EVENT_OBJECT_SELECTIONADD = 0x8007 ;An MSAA event indicating that an item within a container object was added to the selection.
Global Const $EVENT_OBJECT_SELECTIONREMOVE = 0x8008 ;An MSAA event indicating that an item within a container object was removed from the selection.
Global Const $EVENT_OBJECT_SELECTIONWITHIN = 0x8009 ;An MSAA event indicating that numerous selection changes occurred within a container object.
Global Const $EVENT_OBJECT_HELPCHANGE = 0x8010 ;An MSAA event indicating that an object's MSAA Help property changed.
Global Const $EVENT_OBJECT_DEFACTIonchange = 0x8011 ;An MSAA event indicating that an object's MSAA DefaultAction property changed.
Global Const $EVENT_OBJECT_ACCELERATORCHANGE = 0x8012 ;An MSAA event indicating that an object's MSAA KeyboardShortcut property changed.
Global Const $EVENT_OBJECT_INVOKED = 0x8013 ;An MSAA event indicating that an object has been invoked; for example, the user has clicked a button.
Global Const $EVENT_OBJECT_TEXTSELECTIonchangeD = 0x8014 ;An MSAA event indicating that an object's text selection has changed.
Global Const $EVENT_OBJECT_CONTENTSCROLLED = 0x8015 ;An MSAA event indicating that the scrolling of a window object has ended.
Global Const $EVENT_OBJECT_STATECHANGE = 0x800A ;An MSAA event indicating that an object's state has changed.
Global Const $EVENT_OBJECT_LOCATIonchange = 0x800B ;An MSAA event indicating that an object has changed location, shape, or size.
Global Const $EVENT_OBJECT_NAMECHANGE = 0x800C ;An MSAA event indicating that an object's MSAA Name property changed.
Global Const $EVENT_OBJECT_DESCRIPTIonchange = 0x800D ;An MSAA event indicating that an object's MSAA Description property changed.
Global Const $EVENT_OBJECT_VALUECHANGE = 0x800E ;An MSAA event indicating that an object's MSAA Value property changed.
Global Const $EVENT_OBJECT_PARENTCHANGE = 0x800F ;An MSAA event indicating that an object has a new parent object.

;minimum and maximum events
;to monitor one event type only use same event for min/max
Global $EVENT_Min = $EVENT_SYSTEM_SOUND
Global $EVENT_Max = $EVENT_SYSTEM_DRAGDROPEND

             */

        private void SubscribeToWindowMovement(IntPtr hWnd)
        {
            _target = hWnd;

            SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZEEND, _target, WindowMoved, _processId, _threadId, 0);
            //SetWinEventHook(1, 0x0F0F, _target, WindowMinimized, _processId, _threadId, 0);
        }

        private void WindowMinimized(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (hwnd != _target)
                    return;

                switch (eventType)
                {
                    case EVENT_SYSTEM_FOREGROUND:
                        //NOTE: Window was brought to forground
                        System.Console.WriteLine("EVENT_SYSTEM_FOREGROUND");
                        break;
                    case EVENT_SYSTEM_MINIMIZESTART:
                        System.Console.WriteLine("EVENT_SYSTEM_MINIMIZESTART");
                        break;
                    case EVENT_SYSTEM_MINIMIZEEND:
                        System.Console.WriteLine("EVENT_SYSTEM_MINIMIZEEND");
                        break;
                    default:
                        System.Console.WriteLine(eventType);
                        break;
                }
            }
            catch (Exception exception)
            {
                OutputError(exception);
            }
        }

        private void WindowMoved(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (hwnd != _target)
                    return;

                switch (eventType)
                {
                    case EVENT_SYSTEM_MOVESIZESTART:
                        break;
                    case EVENT_SYSTEM_MOVESIZEEND:
                        var position = GetCurrentPosition();

                        var val = $"{position.Left}:{position.Top}|{position.Width}:{position.Height}|{position.BufferWidth}:{position.BufferHeight}";
                        //ConsoleManager.WriteLine(val);

                        Registry.SetSetting("StartPosition", val, Registry.RegistryHKey.CurrentUser);
                        break;
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