using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Commands.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    public sealed class CommandEngine
    {
        private static IInputManager _inputManager;
        private static IRootCommand _rootCommand;
        private static CancellationToken _cancellationToken;

        internal static IRootCommand RootCommand
        {
            get
            {
                if (_rootCommand == null) throw new InvalidOperationException("Cannot access the root command before the command engine has been created.");
                return _rootCommand;
            }
        }

        internal static IInputManager InputManager
        {
            get
            {
                if (_inputManager == null) throw new InvalidOperationException("Cannot access the input manager before the command engine has been created.");
                return _inputManager;
            }
        }

        internal static CancellationToken CancellationToken
        {
            get
            {
                if (_cancellationToken == null) throw new InvalidOperationException("Cannot access the cancellation token before the command engine has been created.");
                return _cancellationToken;
            }
        }

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const short SWP_NOZORDER = 0X4;
        private const int SWP_SHOWWINDOW = 0x0040;

        private const string FlagContinueInConsoleMode = "c";
        private const string FlagContinueInConsoleModeIfError = "e";

        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _commandMode;
        
        public CommandEngine(IRootCommand rootCommand)
        {
            if (rootCommand == null) throw new ArgumentNullException(nameof(rootCommand), "No root command provided.");

            _cancellationTokenSource = new CancellationTokenSource();

            _inputManager = new InputManager(rootCommand.Console);
            _rootCommand = rootCommand;
            _cancellationToken = _cancellationTokenSource.Token;

            _rootCommand.RequestCloseEvent += (sender, e) => { Stop(); };

            ShowAssemblyInfo = true;
            BackgroundColor = System.Console.BackgroundColor;
            DefaultForegroundColor = System.Console.ForegroundColor;
        }

        //public string Title { get; set; }
        public string SplashScreen { get; set; }
        public bool ShowAssemblyInfo { get; set; }
        //public bool TopMost { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor DefaultForegroundColor { get; set; }
        public Runner[] Runners { get; set; }

        public void Run(string[] args)
        {
            //var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            //if (TopMost) SetWindowPos(hWnd, new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

            //TODO: Remember windoews location
            //NOTE: Set specific window location
            //SetWindowPos(hWnd, new IntPtr(0), 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

            //TODO: Get the current windows position
            //RECT rct;
            //GetWindowRect(hWnd, out rct);
            //Console.WriteLine(rct.Left.ToString(), OutputLevel.Warning);

            //System.Console.Beep();

            SetColor();
            //SetTitle();
            ShowSplashScreen();
            DoShowAssemblyInfo();

            _commandMode = args.Length > 0;

            var commands = GetCommands(args);
            var flags = GetFlags(args);

            var commandIndex = 0;

            //TODO: This is only used by the voice console. Solve that some other way!
            //_rootCommand.Initiate();

            if (Runners != null)
            {
                foreach (var runner in Runners)
                {
                    runner.Start();
                }
            }

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                string entry;
                if (_commandMode)
                {
                    entry = GetCommandModeEntry(commands, ref commandIndex, flags);
                }
                else
                {
                    //NOTE: This is where the program is waiting for an input.
                    //When stop is triggered, this should be released and continue with no input somehow.
                    entry = RootCommand.QueryRootParam();
                }

                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (!Execute(entry))
                    {
                        if (_commandMode && HasFlag(args, FlagContinueInConsoleModeIfError))
                        {
                            _commandMode = false;
                            continue;
                        }

                        break;
                    }
                }
            }

            if (Runners != null)
            {
                foreach (var runner in Runners)
                {
                    runner.Close();
                }
            }
        }

        private void SetColor()
        {
            if (System.Console.BackgroundColor == BackgroundColor && System.Console.ForegroundColor == DefaultForegroundColor) return;

            System.Console.BackgroundColor = BackgroundColor;
            System.Console.ForegroundColor = DefaultForegroundColor;
            System.Console.Clear();
        }

        private void ShowSplashScreen()
        {
            if(string.IsNullOrEmpty(SplashScreen))
                return;

            if (!_commandMode)
            {
                RootCommand.Console.Output(new WriteEventArgs(SplashScreen, OutputLevel.Default));
            }
        }

        [ExcludeFromCodeCoverage]
        private void DoShowAssemblyInfo()
        {
            if (!_commandMode && ShowAssemblyInfo)
            {
                var info = AssemblyHelper.GetAssemblyInfo();
                if (!string.IsNullOrEmpty(info))
                {
                    _rootCommand.Console.Output(new WriteEventArgs(info, OutputLevel.Default));
                }
            }
        }

        private static List<string> GetCommands(IEnumerable<string> args)
        {
            return args.Where(x => !x.StartsWith("/")).ToList();
        }

        private static List<string> GetFlags(IEnumerable<string> args)
        {
            return args.Where(x => x.StartsWith("/")).ToList();
        }

        private static bool HasFlag(IEnumerable<string> flags, string flag)
        {
            return flags.Any(x => string.Compare(x.Replace("/", string.Empty), flag.Replace("/", string.Empty), StringComparison.OrdinalIgnoreCase) == 0);
        }

        private string GetCommandModeEntry(IEnumerable<string> commands, ref int commandIndex, IEnumerable<string> flags)
        {
            var cmds = commands as string[] ?? commands.ToArray();
            var entry = cmds.ToList()[commandIndex++];

            if (commandIndex >= cmds.Count())
            {
                if (HasFlag(flags, FlagContinueInConsoleMode))
                {
                    _commandMode = false;
                }
                else
                {
                    _cancellationTokenSource.Cancel();
                }
            }

            //_rootCommand.Console.OutputInformation($"Command {commandIndex}: {entry}");
            _rootCommand.Console.Output(new WriteEventArgs($"Command {commandIndex}: {entry}", OutputLevel.Information));

            return entry;
        }

        private bool Execute(string entry)
        {
            var success = RootCommand.Execute(entry);

            if (_commandMode && !success)
            {
                //_rootCommand.Console.OutputError("Terminating command chain.");
                _rootCommand.Console.Output(new WriteEventArgs("Terminating command chain.", OutputLevel.Error));
                return false;
            }

            return true;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}