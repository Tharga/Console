using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
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

        internal static object SyncRoot = new object();

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
        }

        public Runner[] Runners { get; set; }

        public void Run(string[] args)
        {
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

            _rootCommand.Console.Output(new WriteEventArgs($"Command {commandIndex}: {entry}", OutputLevel.Information));

            return entry;
        }

        private bool Execute(string entry)
        {
            var success = RootCommand.Execute(entry);

            if (_commandMode && !success)
            {
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