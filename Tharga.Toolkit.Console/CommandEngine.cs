using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    public sealed class CommandEngine
    {
        private const string FlagContinueInConsoleMode = "c";
        private const string FlagContinueInConsoleModeIfError = "e";

        internal static readonly object SyncRoot = new object();

        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _commandMode;

        public CommandEngine(IRootCommand rootCommand)
            : this(rootCommand, new InputManager(rootCommand.Console))
        {
        }

        internal CommandEngine(IRootCommand rootCommand, IInputManager inputManager)
        {
            if (rootCommand == null) throw new ArgumentNullException(nameof(rootCommand), "No root command provided.");

            RootCommand = rootCommand;
            InputManager = inputManager;
            _cancellationTokenSource = new CancellationTokenSource();
            RootCommand.RequestCloseEvent += (sender, e) => { Stop(); };

            //TODO: Try reversing the dependency, so that the root command has an engine instead of the engine having a root command.
            var rc = RootCommand as RootCommand;
            rc?.Attach(this);
        }

        public IRootCommand RootCommand { get; }

        internal IInputManager InputManager { get; }

        internal CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public TaskRunner[] TaskRunners { get; set; }

        public void Start(string[] args)
        {
            _commandMode = args.Length > 0;

            var commands = GetCommands(args);
            var flags = GetFlags(args);

            var commandIndex = 0;

            //TODO: This is only used by the voice console. Solve that some other way!
            //_rootCommand.Initiate();

            if (TaskRunners != null)
            {
                foreach (var runner in TaskRunners)
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
                    entry = RootCommand.QueryInput();
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

            if (TaskRunners != null)
            {
                Parallel.ForEach(TaskRunners, x => x.Close());
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

            RootCommand.Console.Output(new WriteEventArgs($"Command {commandIndex}: {entry}", OutputLevel.Information));

            return entry;
        }

        private bool Execute(string entry)
        {
            var success = RootCommand.Execute(entry);

            if (_commandMode && !success)
            {
                RootCommand.Console.Output(new WriteEventArgs("Terminating command chain.", OutputLevel.Error));
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