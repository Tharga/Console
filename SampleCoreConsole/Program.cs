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
//using log4net;

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
                //Part 1. Console.
                using (
                    //NOTE: Enable the type of console you want to use for the sample.
                    console = new DockerConsole(new ConsoleConfiguration {SplashScreen = Constants.SplashScreen})
                    //console = new LogglyConsole(new ConsoleConfiguration {SplashScreen = Constants.SplashScreen})
                    //console = new ClientConsole2(new ConsoleConfiguration { SplashScreen = Constants.SplashScreen })
                    //console = new ClientConsole(new ConsoleConfiguration { SplashScreen = Constants.SplashScreen })
                    //console = new VoiceConsole(new ConsoleConfiguration { SplashScreen = Constants.SplashScreen });
                    //console = new NullConsole();
                    //console = new ActionConsole(e => { System.Diagnostics.Debug.WriteLine(e.Message); });
                    //console = new EventConsole();
                    //console.OutputEvent += (sender, e) => { System.Diagnostics.Debug.WriteLine(e.Message); };
                    //console = new AggregateConsole(new ClientConsole(), new ActionConsole(e => { System.Diagnostics.Debug.WriteLine(e.Message); }));
                )
                {
                    //Part 2. Commands
                    //NOTE: Creating the command object and registering some commands
                    var command = new RootCommand(console);
                    command.RegisterCommand(new SomeContainerCommand());
                    command.RegisterCommand(new MathContainerCommand());
                    command.RegisterCommand(new StatusCommand());
                    command.RegisterCommand(new SomeContainerWithDisabledSubs());
                    var outputContainerCommand = new OutputContainerCommand();
                    command.RegisterCommand(outputContainerCommand);

                    //Part 3. Engine
                    var commandEngine = new CommandEngine(command)
                    {
                        //If you want the console to run some managed background process, they can be created here.
                        TaskRunners = new[]
                        {
                            //new TaskRunner((c, a) =>
                            //NOTE: You can add a runner that runs until the application exits.
                            new TaskRunner(e =>
                            {
                                var i = 0;
                                var intervalSeconds = 15;
                                while (!e.IsCancellationRequested)
                                {
                                    if (i % (10 * intervalSeconds) == 0)
                                    {
                                        //Instance.WriteLine($"First taskrunner is alive in the background. Repporting every {intervalSeconds} seconds.", OutputLevel.Information);
                                        console.Output(new WriteEventArgs($"First taskrunner is alive in the background. Repporting every {intervalSeconds} seconds.", OutputLevel.Information));
                                    }

                                    Thread.Sleep(100);
                                    i++;
                                }

                                Instance.WriteLine("First taskrunner is exiting.", OutputLevel.Information);
                            })

                            //    //NOTE: You can add a runner that contains an AutoResetEvent that triggers when the application exits.
                            //    new TaskRunner(e =>
                            //    {
                            //        Instance.WriteLine("Second taskrunner is doing some stuff at startup.", OutputLevel.Information);
                            //        e.WaitOne();
                            //        Instance.WriteLine("Second taskrunner is doing some stuff before the application exits.", OutputLevel.Information);
                            //    }),
                        }
                    };

                    ////Log4Net
                    ////Enable this section to try out the Log4Net appender provided in the nuget package "Tharga.Toolkit.Log4Net".
                    //var logger =  LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                    ////Logging on different levels
                    //logger.Debug("this Debug msg");
                    //logger.Warn("this Warn msg");
                    //logger.Info("this Info msg");
                    //logger.Error("this Error msg");
                    //logger.Fatal("this Fatal msg");

                    ////Logging exceptions
                    //try
                    //{
                    //    var i = 0;
                    //    var j = 5 / i;
                    //}
                    //catch (Exception ex)
                    //{
                    //    ex.Data.Add("AAA", "AAA1"); //Append data to the exception
                    //    logger.Error("this Error msg,中文测试", ex);
                    //}

                    //NOTE: This part starts the console engine.
                    commandEngine.Start(args);

                    //NOTE: Enable this code if you want to see what happens right before the application closes
                    //Console.WriteLine("Press any key to exit...");
                    //Console.ReadKey();
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

    internal class SomeContainerCommand : ContainerCommandBase
    {
        public SomeContainerCommand()
            : base("some")
        {
            RegisterCommand(new SomeListCommand());
            RegisterCommand(new SomeItemCommand());
            RegisterCommand(new SomeOptionCommand());
            RegisterCommand(new SomeTableCommand());
            RegisterCommand(new SomePasswordCommand());
            RegisterCommand(new SomeDisabledCommand());
            RegisterCommand(new SomeEnumCommand());
            RegisterCommand(new SomeBoolCommand());
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
            for (var i = 0; i < 5; i++)
            {
                OutputInformation($"Some data {i}");
            }
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
                {Guid.Parse("4779177e-2c27-432a-825d-22f9f151391e"), "A"},
                {Guid.NewGuid(), "BB"},
                {Guid.NewGuid(), "D"},
                {Guid.NewGuid(), "EEEEE"},
                {Guid.NewGuid(), "F"},
                {Guid.NewGuid(), "CCC"}
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
            var table = new List<string[]> {new[] {"Index", "Guid"}};
            for (var i = 0; i < 5; i++)
            {
                var line = new[] {i.ToString(), Guid.NewGuid().ToString()};
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
            One,
            Two,
            Three
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
            RegisterCommand(new MathAddCommand());
            RegisterCommand(new MathAddMultipleCommand());
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
            RegisterCommand(new StatusSuccessCommand());
            RegisterCommand(new StatusFailCommand());
            RegisterCommand(new StatusExceptionCommand());
            RegisterCommand(new CrashExceptionCommand());
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

    #endregion

    #region Disable commands

    internal class SomeContainerWithDisabledSubs : ContainerCommandBase
    {
        public SomeContainerWithDisabledSubs()
            : base("Dis")
        {
            RegisterCommand(new AutoDisabledSubs());
            RegisterCommand(new ManualDisabledSubs());
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
            var timer = new Timer {Interval = 3000};
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
}