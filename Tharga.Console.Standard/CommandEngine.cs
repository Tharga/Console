using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Console.Commands.Base;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console
{
    public sealed class CommandEngine
    {
        private const string FlagContinueInConsoleMode = "c";
        private const string FlagContinueInConsoleModeIfError = "e";
        private const string FlagReset = "r";

        internal static readonly object SyncRoot = new object();

        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _commandMode;

        public CommandEngine(IRootCommand rootCommand, CancellationTokenSource cancellationTokenSource = null)
            : this(rootCommand, new InputManager(rootCommand.Console), cancellationTokenSource)
        {
        }

        internal CommandEngine(IRootCommand rootCommand, IInputManager inputManager, CancellationTokenSource cancellationTokenSource)
        {
            if (rootCommand == null) throw new ArgumentNullException(nameof(rootCommand), "No root command provided.");
            if (rootCommand.Console == null) throw new ArgumentNullException(nameof(rootCommand.Console), "No console for root command provided.");

            RootCommand = rootCommand;
            InputManager = inputManager;
            _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            RootCommand.RequestCloseEvent += (sender, e) => { Stop(); };

            //TODO: Try reversing the dependency, so that the root command has an engine instead of the engine having a root command.

            var rc = RootCommand as RootCommandBase;
            rc?.Initiate(this);
        }

        public IRootCommand RootCommand { get; }

        internal IInputManager InputManager { get; }

        internal CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public TaskRunner[] TaskRunners { get; set; }

        public void Start(string[] args)
        {
            try
            {
                var commands = GetCommands(args);
                var flags = GetFlags(args);

                _commandMode = commands.Count > 0;

                var commandIndex = 0;

                //TODO: This is only used by the voice console. Solve that some other way!
                //_rootCommand.Initiate();

                if (TaskRunners != null)
                {
                    Task.Run(() =>
                    {
                        foreach (var runner in TaskRunners)
                        {
                            runner.Start();
                        }
                    }, CancellationToken);
                }

                if (flags.Any())
                {
                    HandleFlags(flags);
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
                        try
                        {
                            entry = RootCommand.QueryInput();
                        }
                        catch (CommandEscapeException)
                        {
                            continue;
                        }
                    }

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

                if (TaskRunners != null)
                {
                    Parallel.ForEach(TaskRunners, x => x.Close());
                }
            }
            catch (Exception exception)
            {
                try
                {
                    RootCommand.Console.OutputError(exception, true);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message + " Original: " + exception.Message + " @" + exception.StackTrace);
                }
            }
        }

        private void HandleFlags(List<string> flags)
        {
            if (HasFlag(flags, FlagReset))
            {
                if (RootCommand.Console is ConsoleBase consoleBase)
                {
                    consoleBase.Reset();
                    RootCommand.Console.Output(new WriteEventArgs("Reset performed, triggered by the reset flag '/r' provide as a parameter.", OutputLevel.Information));
                }
                else
                {
                    RootCommand.Console.Output(new WriteEventArgs("Cannot perform reset command since the console does not inherid from the class 'ConsoleBase'.", OutputLevel.Warning));
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

            if (commandIndex >= cmds.Length)
            {
                if (HasFlag(flags, FlagContinueInConsoleMode))
                {
                    _commandMode = false;
                }
                else
                {
                    Stop();
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
            RootCommand.Console.Close();
        }
    }
}