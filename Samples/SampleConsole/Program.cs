using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;
using Timer = System.Timers.Timer;

namespace SampleConsole
{
    internal class Program
    {
        private const string _splashscreen = "___________ __                                 \n\\__    ___/|  |__ _____ _______  _________     \n  |    |   |  |  \\\\__  \\\\_  __ \\/ ___\\__  \\    \n  |    |   |   Y  \\/ __ \\|  | \\/ /_/  > __ \\_  \n  |____|   |___|  (____  /__|  \\___  (____  /  \n                \\/     \\/     /_____/     \\/   \n";

        [STAThread]
        private static void Main(string[] args)
        {
            var console = new ClientConsole(new ConsoleConfiguration
            {
                SplashScreen = _splashscreen,
                StartLocation = new Location(10,10)
            });
            ////var console = new VoiceConsole();
            ////var console = new ServerConsole(string.Empty);
            ////var console = new ActionConsole((message) => { System.Diagnostics.Debug.WriteLine(message.Item1); });
            ////var console = new AggregateConsole(new ClientConsole(), new ActionConsole((message) => { System.Diagnostics.Debug.WriteLine(message.Item1); }));

            var command = new RootCommand(console);
            command.RegisterCommand(new SomeContainerCommand());
            command.RegisterCommand(new EngineContainerCommand());
            command.RegisterCommand(new MathContainerCommand());
            command.RegisterCommand(new StatusCommand());
            command.RegisterCommand(new ParametersCommand());
            command.RegisterCommand(new SomeContainerWithDisabledSubs());
            command.RegisterCommand(new LineFeedCommand(console));

            var commandEngine = new CommandEngine(command)
            {
                //Runners = new[]{ new Runner(e =>
                //{
                //    var i = 0;
                //    while (!e.IsCancellationRequested)
                //    {
                //        var si = i++.ToString();
                //        System.Console.WriteLine(si + new string('.', 1 * Console.BufferWidth - si.Length + 0)); //v1
                //        System.Console.WriteLine(si + new string('.', 1 * Console.BufferWidth - si.Length + 1)); //v2
                //        System.Console.WriteLine(si + new string('.', 2 * Console.BufferWidth - si.Length + 0)); //v3
                //        System.Console.WriteLine(si + new string('.', 3 * Console.BufferWidth - si.Length + 0)); //v4

                //        console.Output(new WriteEventArgs(si + new string('.', 1 * Console.BufferWidth - si.Length + 0)));
                //        Instance.WriteLine(si + new string('.', 1 * Console.BufferWidth - si.Length + 0), OutputLevel.Information);
                //        Thread.Sleep(10);
                //    }
                //}), }
            };

            //Task.Run(() => { command.QueryRootParam(); }).Wait(1000);

            commandEngine.Run(args);

            console.Dispose();
        }
    }

    internal class LineFeedCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public LineFeedCommand(IConsole console)
            : base("Line", "Line output", true)
        {
            _console = console;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            //NOTE: This will trigger the line feed bug!
            await Task.Run(() =>
            {
                _console.Output(new WriteEventArgs(new string('.', Console.BufferWidth-1), OutputLevel.Default));
                _console.Output(new WriteEventArgs(new string('.', Console.BufferWidth), OutputLevel.Default));
                _console.Output(new WriteEventArgs(new string('.', Console.BufferWidth+1), OutputLevel.Default));

                System.Console.WriteLine("abc\nabc\n" + new string('X', Console.BufferWidth * 2) + "\ns\ns");
                System.Console.WriteLine("abc\nabc\n" + new string('X', Console.BufferWidth * 2) + "\naaa");
                System.Console.WriteLine("abc\n" + new string('X', Console.BufferWidth - 2));
                System.Console.WriteLine(new string('X', Console.BufferWidth) + "\n\nx");
                System.Console.WriteLine(new string('X', Console.BufferWidth - 1));
                System.Console.WriteLine(new string('X', Console.BufferWidth));
                System.Console.WriteLine(new string('X', Console.BufferWidth + 1));
                System.Console.WriteLine(new string('X', Console.BufferWidth * 2));

                System.Console.Write(new string('X', 20));
                System.Console.Write(new string('Y', 10));
                System.Console.Write(new string('Y', Console.BufferWidth - 30));
                System.Console.Write(new string('Y', Console.BufferWidth));
                System.Console.WriteLine(new string('Y', Console.BufferWidth));

                System.Console.WriteLine(new string('X', Console.BufferWidth) + "\n");
                System.Console.WriteLine(new string('X', Console.BufferWidth) + "\n\n");
                System.Console.WriteLine("OK");
            });
            return true;
        }
    }

    internal class SomeContainerCommand : ContainerCommandBase
    {
        public SomeContainerCommand()
            : base("some")
        {
            RegisterCommand(new SomeListCommand());
            RegisterCommand(new SomeItemCommand());
            RegisterCommand(new SomeHugeItemCommand());
            RegisterCommand(new SomeStringItemCommand());
            RegisterCommand(new SomeTableCommand());
            RegisterCommand(new SomeDisabledCommand());
            RegisterCommand(new SomePasswordCommand());
            RegisterCommand(new SomePromptCommand());
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Here is some extra help for the some command.", ConsoleColor.DarkMagenta);
                yield return new HelpLine("It can be placed on several lines.", ConsoleColor.DarkMagenta);
            }
        }
    }

    internal class SomeItemCommand : ActionCommandBase
    {
        public SomeItemCommand()
            : base("item", "Gets a single item.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = await QueryParamAsync("Some Id", GetParam(paramList, index++), KeyNameList);

            OutputInformation($"Some data for {id}");

            return true;
        }

        private async Task<IDictionary<Guid, string>> KeyNameList()
        {
            Thread.Sleep(2000); //Simulate that it takes a while to get this data
            return new Dictionary<Guid, string>
            {
                { Guid.Parse("4779177e-2c27-432a-825d-22f9f151391e"), "A" },
                { Guid.NewGuid(), "BB" },
                { Guid.NewGuid(), "D" },
                { Guid.NewGuid(), "EEEEE" },
                { Guid.NewGuid(), "F" },
                { Guid.NewGuid(), "CCC" }
            };
        }
    }

    internal class SomeHugeItemCommand : ActionCommandBase
    {
        public SomeHugeItemCommand()
            : base("huge", "Gets a single huge item.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = QueryParam("Some Huge Id", GetParam(paramList, index++), HugeKeyNameList());

            OutputInformation($"Some data for {id}");

            return true;
        }

        private List<KeyValuePair<Guid, string>> HugeKeyNameList()
        {
            return new List<KeyValuePair<Guid, string>>
            {
                new KeyValuePair<Guid, string>(Guid.Parse("4779177e-2c27-432a-825d-22f9f151391e"), "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"),
                new KeyValuePair<Guid, string>(Guid.NewGuid(), "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB"),
                new KeyValuePair<Guid, string>(Guid.NewGuid(), "CCCCCCCCCC"),
                new KeyValuePair<Guid, string>(Guid.NewGuid(), "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD"),
                new KeyValuePair<Guid, string>(Guid.NewGuid(), "EEEEEEEEEEEEEEEEEEEE"),
                new KeyValuePair<Guid, string>(Guid.NewGuid(), "FFFFFFFFFF")
            };
        }
    }

    internal class SomeStringItemCommand : ActionCommandBase
    {
        public SomeStringItemCommand()
            : base("string", "Make some string input.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = QueryParam("Some string", GetParam(paramList, index++), new Dictionary<string, string> { { "A", "A" }, { "B", "B" } });

            OutputInformation($"Entered string was: {id}");

            return true;
        }
    }

    internal class SomeListCommand : ActionCommandBase
    {
        public SomeListCommand()
            : base("list", "Lists some information.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            for (var i = 0; i < 5; i++) OutputInformation($"Some data {i}");

            return true;
        }
    }

    internal class SomeTableCommand : ActionCommandBase
    {
        public SomeTableCommand()
            : base("table", "Output information in a table.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var table = new List<string[]> { new[] { "Index", "Guid" } };
            for (var i = 0; i < 5; i++)
            {
                var line = new List<string> { i.ToString(), Guid.NewGuid().ToString() };
                table.Add(line.ToArray());
            }

            OutputTable(table.ToArray());

            return true;
        }
    }

    internal class SomeDisabledCommand : ActionCommandBase
    {
        public SomeDisabledCommand()
            : base("disabled", "Command that is always disabled.")
        {
        }

        public override bool CanExecute(out string reasonMesage)
        {
            reasonMesage = "Because it is disabled. Always!";
            return false;
        }

        public override Task<bool> InvokeAsync(string paramList)
        {
            throw new NotSupportedException("Should not be able to execute this!");
        }
    }

    internal class SomePromptCommand : ActionCommandBase
    {
        public SomePromptCommand()
            : base("prompt")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var abc = QueryParam<string>("ABC", GetParam(paramList, index++));
            var ab = QueryParam<string>("AB", GetParam(paramList, index++));
            var a = QueryParam<string>("A", GetParam(paramList, index++));
            var _ = QueryParam<string>("", GetParam(paramList, index++));

            return true;
        }
    }

    internal class SomePasswordCommand : ActionCommandBase
    {
        public SomePasswordCommand()
            : base("password", "Command with password entry.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var password = QueryPassword("Some password", GetParam(paramList, index++));

            OutputInformation($"Entered password was: {password}");

            return true;
        }
    }

    public class EngineContainerCommand : ContainerCommandBase
    {
        public EngineContainerCommand()
            : base("Engine")
        {
            RegisterCommand(new WorkingOutputCommand());
            RegisterCommand(new ConsoleOutputCommand());
        }
    }

    public class WorkingOutputCommand : ActionCommandBase
    {
        private readonly Timer _timer;

        public WorkingOutputCommand()
            : base("work", "An engine output example that works.")
        {
            _timer = new Timer { Interval = 3000 };
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var rng = new Random();
            //var output = new string('X', rng.Next(30, 200));
            var output = new string('X', rng.Next(3, 20));

            //OutputEvent(output);

            switch (rng.Next(3))
            {
                case 0:
                    OutputInformation(output);
                    break;

                case 1:
                    OutputWarning(output);
                    break;

                case 2:
                    OutputError(output);
                    break;

                //case 3:
                //    OutputEvent(output);
                //    break;

                //case 4:
                //    OutputDefault(output);
                //    break;

                //case 5:
                //    OutputHelp(output);
                //    break;
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            _timer.Start();

            return true;
        }
    }

    public class ConsoleOutputCommand : ActionCommandBase
    {
        private readonly Timer _timer;

        public ConsoleOutputCommand()
            : base("console", "An engine output example that uses console.")
        {
            _timer = new Timer { Interval = 3000 };
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var rng = new Random();
            var output = new string('X', rng.Next(3, 10));

            Console.Write(output);
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            _timer.Start();

            return true;
        }
    }

    public class MathContainerCommand : ContainerCommandBase
    {
        public MathContainerCommand()
            : base("math")
        {
            RegisterCommand(new MathAddCommand());
            RegisterCommand(new MathAddMultipleCommand());
        }
    }

    public class MathAddCommand : ActionCommandBase
    {
        public MathAddCommand()
            : base("add", "Adds two values together.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var val1 = QueryParam<int>("First value", GetParam(paramList, index++));
            var val2 = QueryParam<int>("Second value", GetParam(paramList, index++));

            OutputInformation($"{val1} + {val2} = {val1 + val2}");

            return true;
        }
    }

    public class MathAddMultipleCommand : ActionCommandBase
    {
        public MathAddMultipleCommand()
            : base("addm", "Adds multiple values together.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var vals = new List<int>();

            OutputInformation("Enter multiple values to add. Enter nothing to calculate.");

            while (true)
            {
                var val = QueryParam<int?>("Value", GetParam(paramList, index++));
                if (val == null) break;
                vals.Add(val.Value);
            }

            OutputInformation($"{vals.Sum()}");

            return true;
        }
    }

    public class StatusCommand : ContainerCommandBase
    {
        public StatusCommand()
            : base("status")
        {
            RegisterCommand(new StatusSuccessCommand());
            RegisterCommand(new StatusFailCommand());
            RegisterCommand(new StatusExceptionCommand());
        }
    }

    public class StatusSuccessCommand : ActionCommandBase
    {
        public StatusSuccessCommand()
            : base("success", "An action that returns success.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            OutputInformation("This command worked.");
            return true;
        }
    }

    public class StatusFailCommand : ActionCommandBase
    {
        public StatusFailCommand()
            : base("fail", "An action that returns failure.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            OutputWarning("This command did not work.");
            return false;
        }
    }

    public class StatusExceptionCommand : ActionCommandBase
    {
        public StatusExceptionCommand()
            : base("exception", "A command that throws an exception.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var exception = new Exception("Some even deeper exception.");
            exception.Data.Add("A1", "B1");

            var innerException = new Exception("Some inner exception.", exception);
            innerException.Data.Add("A1", "B1");
            innerException.Data.Add("A2", "B2");

            var invalidOperationException = new InvalidOperationException("Some crash.", innerException);
            invalidOperationException.Data.Add("xxx", "111");

            throw invalidOperationException;
        }
    }

    public class ParametersCommand : ContainerCommandBase
    {
        public ParametersCommand()
            : base("param")
        {
            var buildParametersCommand = new BuildParametersCommand();
            RegisterCommand(buildParametersCommand);

            RegisterCommand(new ExecuteParametersCommand(buildParametersCommand));
        }
    }

    public class BuildParametersCommand : ActionCommandBase
    {
        public BuildParametersCommand()
            : base("build", "Build command parameters.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var parameters = CreateParameters(paramList);
            OutputInformation($"Created parameters: {parameters}");

            return true;
        }

        public string CreateParameters(string paramList)
        {
            var index = 0;
            var val1 = QueryParam<string>("First value", GetParam(paramList, index++));
            var val2 = QueryParam<string>("Second value", GetParam(paramList, index++));
            var val3 = QueryParam("Third value", GetParam(paramList, index++), new Dictionary<string, string> { { "A", "A" }, { "B", "B" } });
            var val4 = "some_constant";
            var val5 = DateTime.UtcNow.DayOfWeek;

            var parameters = string.Format("{0} {1} {2} {3} {4}", val1, val2, val3, val4, val5);
            return parameters;
        }
    }

    public class ExecuteParametersCommand : ActionCommandBase
    {
        private readonly BuildParametersCommand _parametersCommand;

        public ExecuteParametersCommand(BuildParametersCommand parametersCommand)
            : base("execute", "Execute a command.")
        {
            _parametersCommand = parametersCommand;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var parameters = _parametersCommand.CreateParameters(paramList);

            //TODO: Execute something using the parameters
            OutputInformation($"Execute somthing using the parameters: {parameters}");

            return true;
        }
    }

    internal class SomeContainerWithDisabledSubs : ContainerCommandBase
    {
        public SomeContainerWithDisabledSubs()
            : base("Disabled")
        {
            RegisterCommand(new SomeDisabledCommand());
            RegisterCommand(new SomeItemCommand());
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command contains sub-command that are all disabled. Therefore the group command is also disabled."); }
        }

        public override bool CanExecute(out string reasonMessage)
        {
            reasonMessage = "Because it is manually disabled.";
            return false;
        }
    }
}