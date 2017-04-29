using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console
{
    public class Runner
    {
        private readonly Action<bool> _action;
        private Task _task;
        private CancellationToken _cancellationToken;
        private bool _running = true;

        public Runner(Action<bool> action)
        {
            _action = action;
            _cancellationToken = new CancellationToken();
        }

        public void Start()
        {
            _task = Task.Run(() =>
            {
                _action(_running);
            }, _cancellationToken);
        }

        public void Close()
        {
            _running = false;
            //_cancellationToken.Register()
            _task.Wait();
        }
    }

    public class CommandEngine : ICommandEngine
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const short SWP_NOZORDER = 0X4;
        private const int SWP_SHOWWINDOW = 0x0040;

        private const string FlagContinueInConsoleMode = "c";
        private const string FlagContinueInConsoleModeIfError = "e";
        private readonly RootCommandBase _rootCommand;
        private bool _running = true;
        private bool _commandMode;

        private CommandEngine()
        {
            ShowAssemblyInfo = true;
            BackgroundColor = System.Console.BackgroundColor;
            DefaultForegroundColor = System.Console.ForegroundColor;
        }

        internal CommandEngine(IConsole console)
            : this()
        {
            _rootCommand = new RootCommand(console, Stop);
        }

        public CommandEngine(RootCommandBase rootCommand)
            : this()
        {
            rootCommand.SetStopAction(Stop);
            _rootCommand = rootCommand;
        }

        public string Title { get; set; }
        public string SplashScreen { get; set; }
        public bool ShowAssemblyInfo { get; set; }
        public bool TopMost { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor DefaultForegroundColor { get; set; }
        //public Action<CancellationToken>[] Actions { get; set; }
        //public Action Action { get; set; }
        //public Task<CancellationToken> Task { get; set; }
        public Runner Runner { get; set; }

        public IConsole Console => _rootCommand.Console;

        public void Run(string[] args)
        {
            var hWnd = Process.GetCurrentProcess().MainWindowHandle;
            if (TopMost) SetWindowPos(hWnd, new IntPtr(HWND_TOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

            //TODO: Remember windoews location
            //NOTE: Set specific window location
            //SetWindowPos(hWnd, new IntPtr(0), 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

            //TODO: Get the current windows position
            //RECT rct;
            //GetWindowRect(hWnd, out rct);
            //Console.WriteLine(rct.Left.ToString(), OutputLevel.Warning);

            //System.Console.Beep();

            SetColor();
            SetTitle();
            ShowSplashScreen();
            DoShowAssemblyInfo();

            _commandMode = args.Length > 0;

            var commands = GetCommands(args);
            var flags = GetFlags(args);

            var commandIndex = 0;

            _rootCommand.Initiate();

            Runner.Start();
            //Task.Start();
            //Start provided tasks
            //var cancellationToken = new CancellationToken();
            //foreach (var action in Actions)
            //{
            //    //Task.Run(cancellationToken => action(cancellationToken), cancellationToken);
            //    //Task<Action<CancellationToken>>.Run(x => x(cancellationToken), cancellationToken);
            //}

            while (_running)
            {
                var entry = _commandMode ? GetCommandModeEntry(commands, ref commandIndex, flags) : _rootCommand.QueryRootParam();
                if (!Execute(entry))
                {
                    if (_commandMode && HasFlag(args, FlagContinueInConsoleModeIfError))
                    {
                        _commandMode = false;
                        _running = true;
                        continue;
                    }

                    break;
                }
            }

            //Task.Wait();
            Runner.Close();
        }

        private void SetColor()
        {
            if (System.Console.BackgroundColor == BackgroundColor && System.Console.ForegroundColor == DefaultForegroundColor) return;

            System.Console.BackgroundColor = BackgroundColor;
            System.Console.ForegroundColor = DefaultForegroundColor;
            System.Console.Clear();
        }

        private void SetTitle()
        {
            System.Console.Title = Title ?? GetAssemblyInfo() ?? "Tharga Console";
        }

        private void ShowSplashScreen()
        {
            if(string.IsNullOrEmpty(SplashScreen))
                return;

            if (!_commandMode)
            {
                _rootCommand.Output(SplashScreen, OutputLevel.Default, true);
            }
        }

        [ExcludeFromCodeCoverage]
        private void DoShowAssemblyInfo()
        {
            if (!_commandMode && ShowAssemblyInfo)
            {
                var info = GetAssemblyInfo();
                if (!string.IsNullOrEmpty(info))
                {
                    _rootCommand.Output(info, OutputLevel.Default, true);
                }
            }
        }

        private static string GetAssemblyInfo()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) return null;
            return $"{assembly.GetName().Name} (Version {assembly.GetName().Version})";
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
                    _running = false;
                }
            }

            _rootCommand.Console.OutputInformation($"Command {commandIndex}: {entry}");

            return entry;
        }

        private bool Execute(string entry)
        {
            var success = _rootCommand.Execute(entry);

            if (_commandMode && !success)
            {
                _rootCommand.Console.OutputError("Terminating command chain.");
                return false;
            }

            return true;
        }

        public void Stop()
        {
            _running = false;
        }
    }
}