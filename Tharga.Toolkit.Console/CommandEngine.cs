using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    public sealed class CommandEngine
    {
        private IRootCommand _rootCommand;
        private IInputManager _inputManager;

        internal static readonly object SyncRoot = new object();

        public IRootCommand RootCommand => _rootCommand;
        internal IInputManager InputManager => _inputManager;
        internal CancellationToken CancellationToken => _cancellationTokenSource.Token;

        private const string FlagContinueInConsoleMode = "c";
        private const string FlagContinueInConsoleModeIfError = "e";

        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _commandMode;

        public CommandEngine(IRootCommand rootCommand)
            : this(rootCommand, new InputManager(rootCommand.Console))
        {            
        }

        internal CommandEngine(IRootCommand rootCommand, IInputManager inputManager)
        {
            if (rootCommand == null) throw new ArgumentNullException(nameof(rootCommand), "No root command provided.");

            _rootCommand = rootCommand;
            _inputManager = inputManager;
            _cancellationTokenSource = new CancellationTokenSource();
            _rootCommand.RequestCloseEvent += (sender, e) => { Stop(); };

            //TODO: Try reversing the dependency, so that the root command has an engine instead of the engine having a root command.
            var rc = _rootCommand as RootCommand;
            rc?.Attach(this);
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
                    entry = _rootCommand.QueryRootParam();
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
            var success = _rootCommand.Execute(entry);

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