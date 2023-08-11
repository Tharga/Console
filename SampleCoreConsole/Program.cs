using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using SampleCoreConsole.Business;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;
using Timer = System.Timers.Timer;

namespace SampleCoreConsole
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            IConsole console = null;
            try
            {
                using (console = new VoiceConsole(new VoiceConsoleConfiguration { SplashScreen = Constants.SplashScreen }))
                {
                    var container = new WindsorContainer();

                    container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetAssembly(typeof(Program)))
                        .IncludeNonPublicTypes()
                        .BasedOn<ICommand>()
                        //.Configure(x => System.Diagnostics.Debug.WriteLine($"Registered in IOC: {x.Implementation.Name}"))
                        .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

                    container.Register(Component.For<SingletonBusiness, ISingletonBusiness>().LifeStyle.Singleton);
                    container.Register(Component.For<TransientBusiness, ITransientBusiness>().LifeStyle.Transient);

                    var command = new RootCommandWindows(console, new CommandResolver(type => (ICommand)container.Resolve(type)));

                    //command.UnregisterCommand("command");
                    command.RegisterCommand<SomeContainerCommand>();
                    command.RegisterCommand<SomeMoreCommand>();
                    command.RegisterCommand<MathContainerCommand>();
                    command.RegisterCommand<StatusCommand>();
                    command.RegisterCommand<SomeContainerWithDisabledSubs>();
                    command.RegisterCommand<OutputContainerCommand>();
                    command.RegisterCommand<ReadKeyLoop>();
                    command.RegisterCommand<InfiniteLoop>();
                    command.RegisterCommand<InjectBusinessCommand>();

                    var commandEngine = new CommandEngine(command)
                    {
                        TaskRunners = new[]
                        {
                            new TaskRunner(async (c, _) =>
                            {
                                await Task.Delay(1000, c);
                            })
                        }
                    };

                    commandEngine.Start(args);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Fatal Error.");
                console?.OutputError(exception);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            finally
            {
                console?.Dispose();
            }
        }
    }

    #region Some basic commands

    internal class SomeMoreCommand : ActionCommandBase
    {
        public SomeMoreCommand()
            : base("somemore")
        {
        }

        public override void Invoke(string[] param)
        {
            OutputInformation("Yee!");
        }
    }

    internal class SomeContainerCommand : ContainerCommandBase
    {
        public SomeContainerCommand()
            : base("some")
        {
            //var x = QueryParam<bool>("");

            RegisterCommand<SomeListCommand>();
            RegisterCommand<SomeItemCommand>();
            RegisterCommand<SomeOptionCommand>();
            RegisterCommand<SomeTableCommand>();
            RegisterCommand<SomePasswordCommand>();
            RegisterCommand<SomeDisabledCommand>();
            RegisterCommand<SomeEnumCommand>();
            RegisterCommand<SomeBoolCommand>();
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("A container command can be used to group commands together.");
                yield return new HelpLine("All commands registerd within a container command will appear as sub-commands to the container.");
                yield return new HelpLine("All sub-commands are triggered by typing 'some [command]'.");
            }
        }
    }

    internal class SomeListCommand : ActionCommandBase
    {
        public SomeListCommand()
            : base("list", "Lists some information.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The list commands basicly displays text as information.");
                yield return new HelpLine("This command is triggered by typing 'some list' in the console.");
            }
        }

        public override void Invoke(string[] param)
        {
            for (var i = 0; i < 5; i++) OutputInformation($"Some data {i}");
        }
    }

    internal class SomeItemCommand : ActionCommandBase
    {
        public SomeItemCommand()
            : base("item", "Gets a single item.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The item command shows you how to query the user for data.");
                yield return new HelpLine("This command is triggered by typing 'some item' in the console.");
                yield return new HelpLine("You can provide parameters directly inline. (Ex Type 'some item A')");
            }
        }

        public override void Invoke(string[] param)
        {
            var id = QueryParam<string>("Some Id", param);

            OutputInformation($"Some data for {id}");
        }
    }

    internal class SomeOptionCommand : ActionCommandBase
    {
        public SomeOptionCommand()
            : base("option", "Gets a single item from a list of options.")
        {
            RegisterQuery("Id", "Some Id", GetSelection);
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("This command is triggered by typing 'some option' in the console.");
                yield return new HelpLine("You can provide parameters directly inline. (Ex Type 'some option A')");
                yield return new HelpLine("Use the Tab-key to cycle through the options to choose from, or type the text directly.");
                yield return new HelpLine("The selection contains of a key and a display text. When the text is entered the key is returned.");
            }
        }

        public override void Invoke(string[] param)
        {
            OutputInformation($"Some data for {GetParam<Guid>("Id")}");
        }

        private IDictionary<Guid, string> GetSelection()
        {
            //Options to choose from
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

    internal class SomeTableCommand : ActionCommandBase
    {
        public SomeTableCommand()
            : base("table", "Output information in a table.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The table command shows you how to output data as a table.");
                yield return new HelpLine("This command is triggered by typing 'some table' in the console.");
            }
        }

        public override void Invoke(string[] param)
        {
            var table = new List<string[]> { new[] { "Index", "Guid" } };
            for (var i = 0; i < 5; i++)
            {
                var line = new[] { i.ToString(), Guid.NewGuid().ToString() };
                table.Add(line);
            }

            OutputTable(table);
        }
    }

    internal class SomePasswordCommand : ActionCommandBase
    {
        public SomePasswordCommand()
            : base("password", "Command with password entry.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The password command shows you how to query the user for a password.");
                yield return new HelpLine("This command is triggered by typing 'some password' in the console.");
                yield return new HelpLine("You can provide parameters directly inline, but then the password will not be protected from showing on screen. (Ex Type 'some password A')");
            }
        }

        public override void Invoke(string[] param)
        {
            var password = QueryPassword("Some password", GetNextParam(param));
            OutputInformation($"Entered password was: {password}");
        }
    }

    internal class SomeDisabledCommand : ActionCommandBase
    {
        public SomeDisabledCommand()
            : base("disabled", "Command that is always disabled.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The disabled command shows you how to have a command disabled.");
                yield return new HelpLine("This command is triggered by typing 'some disabled' in the console.");
                yield return new HelpLine("The function 'CanExecute' can be overrdden and depending on circumstances, return the proper state together with a reason message.");
                yield return new HelpLine("You can still get help from a command that is disabled.");
            }
        }

        public override bool CanExecute(out string reasonMesage)
        {
            reasonMesage = "Because it is manually disabled. Always!";
            return false;
        }

        public override void Invoke(string[] param)
        {
            throw new NotSupportedException("Should not be able to execute this!");
        }
    }

    internal class SomeEnumCommand : ActionCommandBase
    {
        public SomeEnumCommand()
            : base("enum", "Enum option selection.")
        {
        }

        public override void Invoke(string[] param)
        {
            var selection = QueryParam<MyEnum>("Select", param);
            OutputInformation(selection.ToString());
        }

        private enum MyEnum
        {
            B,
            A,
            C
        }
    }

    internal class SomeBoolCommand : ActionCommandBase
    {
        public SomeBoolCommand()
            : base("bool", "Choose between yes and no.")
        {
        }

        public override void Invoke(string[] param)
        {
            var selection = QueryParam<bool>("Select", param);
            OutputInformation(selection.ToString());
        }
    }

    #endregion
    #region Math commands (parameter example)

    public class MathContainerCommand : ContainerCommandBase
    {
        public MathContainerCommand()
            : base("math")
        {
            RegisterCommand<MathAddCommand>();
            RegisterCommand<MathAddMultipleCommand>();
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("The math commands demonstrates how to handle some user query parameters in different ways."); }
        }
    }

    public class MathAddCommand : ActionCommandBase
    {
        public MathAddCommand()
            : base("add", "Adds two values together.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The 'math add' command demonstrates how to read parameters from the inline query in different ways.");
                yield return new HelpLine("Try triggering by typing 'math add 1 2 3 4', 'math add 1 2 3' or by just typing 'math add'.");
            }
        }

        public override void Invoke(string[] param)
        {
            //Get the next parameter in line, if provided inline (1st value)
            var val1 = QueryParam<int>("First value", param);

            //Get the next parameter in line, if provided inline (2nd value)
            var val2 = QueryParam<int>("Second value", GetNextParam(param));

            //Get a specific parameter, if provided inline (2nd value). The parameter indexer is set to continur from here.
            var val3 = QueryParam<int>("Third value", GetParam(param, 2));

            //Get the next parameter in line, if provided inline (4th value)
            var val4 = QueryParam<int>("Forth value", param);

            OutputInformation($"{val1} + {val2} + {val3} + {val4} = {val1 + val2 + val3 + val4}");
        }
    }

    public class MathAddMultipleCommand : ActionCommandBase
    {
        public MathAddMultipleCommand()
            : base("addm", "Adds multiple values together.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("The 'math addm' command demonstrates how to query infinite number of parameters from the user.");
                yield return new HelpLine("Try triggering by typing 'math addm 1 2 3 4', 'math addm 1 2 3' or by just typing 'math addm'.");
            }
        }

        public override void Invoke(string[] param)
        {
            var vals = new List<int>();

            OutputInformation("Enter multiple values to add. Enter nothing to calculate.");

            while (true)
            {
                //Continue to get all parameters provided inline, then start querying the user.
                var val = QueryParam<int?>("Value", GetNextParam(param));
                if (val == null) break;
                vals.Add(val.Value);
            }

            OutputInformation($"Sum: {vals.Sum()}");
        }
    }

    #endregion
    #region Status commands

    public class StatusCommand : ContainerCommandBase
    {
        public StatusCommand()
            : base("status")
        {
            RegisterCommand<StatusSuccessCommand>();
            RegisterCommand<StatusFailCommand>();
            RegisterCommand<StatusExceptionCommand>();
            RegisterCommand<CrashExceptionCommand>();
            RegisterCommand<AggregateCrashExceptionCommand>();
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("Commands for showing different methods of outputing data."); }
        }
    }

    public class StatusSuccessCommand : ActionCommandBase
    {
        public StatusSuccessCommand()
            : base("success", "An action that returns success.")
        {
        }

        public override void Invoke(string[] param)
        {
            OutputInformation("This command shows information, indicating that it worked.");
        }
    }

    public class StatusFailCommand : ActionCommandBase
    {
        public StatusFailCommand()
            : base("fail", "An action that returns failure.")
        {
        }

        public override void Invoke(string[] param)
        {
            OutputWarning("This command outputs a warning, indicating that it did not work.");
        }
    }

    public class StatusExceptionCommand : ActionCommandBase
    {
        public StatusExceptionCommand()
            : base("exception", "A command that outputs an exception.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This commands throws, catches and outputs an exception."); }
        }

        public override void Invoke(string[] param)
        {
            try
            {
                throw new NotImplementedException("Some issue.");
            }
            catch (Exception e)
            {
                e.Data.Add("A", "A1");
                OutputError(e);
            }
        }
    }

    public class CrashExceptionCommand : ActionCommandBase
    {
        public CrashExceptionCommand()
            : base("crash", "A command that throws an exception.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command throws an exception."); }
        }

        public override void Invoke(string[] param)
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

    public class AggregateCrashExceptionCommand : ActionCommandBase
    {
        public AggregateCrashExceptionCommand()
            : base("aggregatecrash", "A command that throws an aggregate exception.")
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command throws an exception."); }
        }

        public override void Invoke(string[] param)
        {
            var exception1 = new Exception("First aggregate exception.");
            exception1.Data.Add("AA11", "AB11");
            exception1.Data.Add("AA12", "AB12");
            var exception2 = new Exception("Second aggregate exception.");
            exception2.Data.Add("AA21", "AB21");

            var exception = new AggregateException(exception1, exception2);
            //var exception = new Aggregate("Some even deeper exception.");
            exception.Data.Add("A1", "B1");

            var innerException = new Exception("Some inner exception.", exception);
            innerException.Data.Add("A1", "B1");
            innerException.Data.Add("A2", "B2");

            var invalidOperationException = new InvalidOperationException("Some crash.", innerException);
            invalidOperationException.Data.Add("xxx", "111");

            throw invalidOperationException;
        }
    }

    #endregion
    #region Disable commands

    internal class SomeContainerWithDisabledSubs : ContainerCommandBase
    {
        public SomeContainerWithDisabledSubs()
            : base("Dis")
        {
            RegisterCommand<AutoDisabledSubs>();
            RegisterCommand<ManualDisabledSubs>();
        }
    }

    internal class AutoDisabledSubs : ContainerCommandBase
    {
        public AutoDisabledSubs()
            : base("Auto")
        {
            RegisterCommand(new SomeDisabledCommand());
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This container command contains sub-commands that are all disabled. Therefore the group command is also disabled."); }
        }
    }

    internal class ManualDisabledSubs : ContainerCommandBase
    {
        public ManualDisabledSubs()
            : base("Man")
        {
            RegisterCommand(new SomeDisabledCommand());
            RegisterCommand(new SomeItemCommand());
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This container command is manually disabled. That makes all sub-commands disabled by inheritance."); }
        }

        public override bool CanExecute(out string reasonMessage)
        {
            reasonMessage = "Manually disabled container command and all sub commands.";
            return false;
        }
    }

    #endregion
    #region Output commands

    public class OutputContainerCommand : ContainerCommandBase
    {
        public OutputContainerCommand()
            : base("Output")
        {
            RegisterCommand(new TimerOutputCommand());
            RegisterCommand(new ConsoleOutputCommand());
            RegisterCommand(new WriteOutputCommand());
        }
    }

    public class TimerOutputCommand : ContainerCommandBase
    {
        public TimerOutputCommand()
            : base("timer", "An engine outputs examples on a timer.")
        {
            var timer = new Timer { Interval = 3000 };
            timer.Elapsed += Timer_Elapsed;

            RegisterCommand(new TimerStartCommand(timer));
            RegisterCommand(new TimerStopCommand(timer));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var rng = new Random();
            var output = new string('X', rng.Next(3, 40));

            switch (rng.Next(6))
            {
                case 0:
                    OutputInformation($"Info: {output}");
                    break;

                case 1:
                    OutputWarning($"Warning: {output}");
                    break;

                case 2:
                    OutputError($"Error: {output}");
                    break;

                case 3:
                    OutputEvent($"Event: {output}");
                    break;

                case 4:
                    OutputDefault($"Default: {output}");
                    break;

                case 5:
                    OutputHelp($"Help: {output}");
                    break;
            }
        }
    }

    public class TimerStartCommand : ActionCommandBase
    {
        private readonly Timer _timer;

        public TimerStartCommand(Timer timer)
            : base("Start")
        {
            _timer = timer;
        }

        public override void Invoke(string[] param)
        {
            _timer.Start();
            OutputInformation($"Starting the output timer. Running on {_timer.Interval}ms interval.");
        }
    }

    public class TimerStopCommand : ActionCommandBase
    {
        private readonly Timer _timer;

        public TimerStopCommand(Timer timer)
            : base("Stop")
        {
            _timer = timer;
        }

        public override void Invoke(string[] param)
        {
            _timer.Stop();
            OutputInformation("Stopping the output timer.");
        }
    }

    public class ConsoleOutputCommand : ActionCommandBase
    {
        public ConsoleOutputCommand()
            : base("console", "Output using the common console.")
        {
        }

        public override void Invoke(string[] param)
        {
            //NOTE: This shows that 'write' writes without interruption even from background processes.
            Console.Write("Basic console write without line feed. ");
            Console.Write("Works together with background output processes. ");
            Thread.Sleep(10000); //Enable this line and se how it works together with the 'first taskrunner' and the timer.
            Console.Write("Still writes without a line break!");

            //NOTE: Common Console.WriteLine works as usual.
            Console.WriteLine("Basic console write line. (Outputs as default)");

            //NOTE: Use this global singleton to make console outputs from anywhere and still select the output level.
            Instance.WriteLine("Global instance write line with control of output level.", OutputLevel.Warning);
        }
    }

    public class WriteOutputCommand : ActionCommandBase
    {
        public WriteOutputCommand()
            : base("write")
        {
        }

        public override void Invoke(string[] param)
        {
            Task.Run(() =>
            {
                //TODO: There is a minor bug here.
                //This code overwrites what is underneeth. A written line should be pushed down.
                for (var i = 0; i < 100; i++)
                {
                    Thread.Sleep(100);
                    Console.Write(".");
                }
            });
        }
    }

    #endregion
    #region Inject commands

    public class InjectBusinessCommand : ContainerCommandBase
    {
        public InjectBusinessCommand()
            : base("Inject")
        {
            RegisterCommand<InjectTransientCommand>();
            RegisterCommand<InjectSingletonCommand>();
        }
    }

    public class InjectTransientCommand : ActionCommandBase
    {
        private readonly ITransientBusiness _transientBusiness;

        public InjectTransientCommand(ITransientBusiness transientBusiness)
            : base("Transient")
        {
            _transientBusiness = transientBusiness;
        }

        public override void Invoke(string[] param)
        {
            OutputInformation($"T: {_transientBusiness.GetValue()}");
        }
    }

    internal class InjectSingletonCommand : AsyncActionCommandBase
    {
        private readonly ISingletonBusiness _singletonBusiness;

        public InjectSingletonCommand(ISingletonBusiness singletonBusiness)
            : base("Singleton")
        {
            _singletonBusiness = singletonBusiness;
        }

        public override Task InvokeAsync(string[] param)
        {
            OutputInformation($"S: {_singletonBusiness.GetValue()}");
            return Task.CompletedTask;
        }
    }

    #endregion
    #region Loop Command

    public class ReadKeyLoop : ActionCommandBase
    {
        public ReadKeyLoop()
            : base("Loop")
        {
        }

        public override void Invoke(string[] param)
        {
            OutputInformation("Press Enter or ESC to exit.");
            var buffer = string.Empty;
            while (true)
            {
                var key = QueryKey();
                buffer += key.KeyChar;
                //RootCommand.Console.Output(new WriteEventArgs(key.KeyChar.ToString(), lineFeed: false));

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        OutputInformation(buffer);
                        return;
                    case ConsoleKey.Escape:
                        OutputInformation("");
                        return;
                }
            }
        }
    }

    public class InfiniteLoop : ActionCommandBase
    {
        public InfiniteLoop()
            : base("Infinite")
        {
        }

        public override void Invoke(string[] param)
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                        return;
                    OutputInformation("This loop will run forever");
                    Thread.Sleep(2000);
                }
            }).Start();
        }
    }

    #endregion
}