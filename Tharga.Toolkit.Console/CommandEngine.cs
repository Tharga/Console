using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console
{
    public class CommandEngine : ICommandEngine
    {
        private const string FlagContinueInConsoleMode = "c";
        private const string FlagContinueInConsoleModeIfError = "e";
        private readonly RootCommandBase _rootCommand;
        private bool _running = true;
        private bool _commandMode;

        internal CommandEngine(IConsole console)
        {
            _rootCommand = new RootCommand(console, Stop);
        }

        public CommandEngine(RootCommandBase rootCommand)
        {
            rootCommand.SetStopAction(Stop);
            _rootCommand = rootCommand;
        }

        public string SplashScreen { get; set; }
        public IConsole Console => _rootCommand.Console;

        public void Run(string[] args)
        {
            _commandMode = args.Length > 0;

            var commands = GetCommands(args);
            var flags = GetFlags(args);

            ShowSplashScreen();

            ShowAssemblyInfo();

            var commandIndex = 0;

            _rootCommand.Initiate();

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
        }

        private void ShowSplashScreen()
        {
            if(string.IsNullOrEmpty(SplashScreen))
                return;

            if (!_commandMode)
            {
                //_rootCommand.OutputInformation(SplashScreen);
                _rootCommand.Output(SplashScreen, OutputLevel.Default, true);
            }
        }

        [ExcludeFromCodeCoverage]
        private void ShowAssemblyInfo()
        {
            var assembly = Assembly.GetEntryAssembly();
            //if (assembly != null) _rootCommand.OutputInformationLine($"{assembly.GetName().Name} (Version {assembly.GetName().Version})", _commandMode);
            if (!_commandMode && assembly != null)
            {
                //_rootCommand.OutputInformation($"{assembly.GetName().Name} (Version {assembly.GetName().Version})");
                _rootCommand.Output($"{assembly.GetName().Name} (Version {assembly.GetName().Version})", OutputLevel.Default, true);
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