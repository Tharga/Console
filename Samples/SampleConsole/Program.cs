﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace SampleConsole
{
    using System.IO;
    using System.Text;

    internal class Program
    {        [STAThread]
        private static void Main(string[] args)
        {
            var console = new ClientConsole();
            //var console = new VoiceConsole();
            //var console = new ServerConsole(string.Empty);

            var command = new RootCommand(console);
            command.RegisterCommand(new SomeContainerCommand());
            command.RegisterCommand(new EngineContainerCommand());
            command.RegisterCommand(new MathContainerCommand());
            command.RegisterCommand(new StatusCommand());
            new CommandEngine(command).Run(args);
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
        }
    }

    internal class SomeItemCommand : ActionCommandBase
    {
        public SomeItemCommand()
            : base("item", "Gets a single item")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = await QueryParamAsync("Some Id", GetParam(paramList, index++), KeyNameList);

            OutputInformation("Some data for {0}", id);

            return true;
        }

        private async Task<IDictionary<Guid, string>> KeyNameList()
        {
            System.Threading.Thread.Sleep(2000); //Simulate that it takes a while to get this data
            return new Dictionary<Guid, string>
            {
                { Guid.Parse("4779177e-2c27-432a-825d-22f9f151391e"), "A" },
                { Guid.NewGuid(), "BB" },
                { Guid.NewGuid(), "CCC" },
                { Guid.NewGuid(), "D" },
                { Guid.NewGuid(), "EEEEE" },
                { Guid.NewGuid(), "F" },
            };
        }
    }

    internal class SomeHugeItemCommand : ActionCommandBase
    {
        public SomeHugeItemCommand()
            : base("huge", "Gets a single huge item")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = QueryParam<Guid>("Some Huge Id", GetParam(paramList, index++), HugeKeyNameList());

            OutputInformation("Some data for {0}", id);

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
                new KeyValuePair<Guid, string>(Guid.NewGuid(), "FFFFFFFFFF"),
            };
        }
    }

    internal class SomeStringItemCommand : ActionCommandBase
    {
        public SomeStringItemCommand()
            : base("string", "Make some string input")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = QueryParam<string>("Some string", GetParam(paramList, index++), new Dictionary<string, string> { { "A", "A" }, { "B", "B" } });

            OutputInformation("Entered string was: {0}", id);

            return true;
        }
    }

    internal class SomeListCommand : ActionCommandBase
    {
        public SomeListCommand()
            : base("list", "Lists some information")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            for (var i = 0; i < 5; i++) OutputInformation("Some data {0}", i);

            return true;
        }
    }

    internal class SomeTableCommand : ActionCommandBase
    {
        public SomeTableCommand()
            : base("table", "Output information in a table")
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

    public class EngineContainerCommand : ContainerCommandBase
    {
        public EngineContainerCommand()
            : base("Engine")
        {
            RegisterCommand(new WorkingOutputCommand());
            RegisterCommand(new FailingOutputCommand());
        }
    }

    public class WorkingOutputCommand : ActionCommandBase
    {
        private readonly Timer _timer;

        public WorkingOutputCommand()
            : base("work", "An engine output example that works")
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

            switch (rng.Next(4))
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

                case 3:
                    OutputEvent(output);
                    break;
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            _timer.Start();

            return true;
        }
    }

    public class FailingOutputCommand : ActionCommandBase
    {
        private readonly Timer _timer;

        public FailingOutputCommand()
            : base("fail", "An engine output example that fails")
        {
            _timer = new Timer { Interval = 3000 };
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var rng = new Random();
            //var output = new string('X', rng.Next(3, 10));

            var output = "X";

            //Console.WriteLine(output); //Do not use Console.WriteLine or Console.Write, use OutputLine instead.
            //Console.WriteLine('X');
            //Console.WriteLine(new[] { 'A', 'B', 'C' });

            //Console.Write(new string('x', 100));

            Console.Write("ABCDEF_");
            Console.Write("ABCDEF_");
            Console.Write("ABCDEF_");
            Console.Write("ABCDEF_");
            Console.Write("xxx_");
            //Console.WriteLine("yyy.");

            //Console.Write("ABCDEFGHIJKLMNOPQRTSUVXYZ_");
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
            : base("add", "Adds two values together")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var val1 = QueryParam<int>("First value", GetParam(paramList, index++));
            var val2 = QueryParam<int>("Second value", GetParam(paramList, index++));

            OutputInformation("{0} + {1} = {2}", val1, val2, val1 + val2);

            return true;
        }
    }

    public class MathAddMultipleCommand : ActionCommandBase
    {
        public MathAddMultipleCommand()
            : base("addm", "Adds multiple values together")
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

            OutputInformation("{0}", vals.Sum());

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

        public async override Task<bool> InvokeAsync(string paramList)
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

        public async override Task<bool> InvokeAsync(string paramList)
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

        public async override Task<bool> InvokeAsync(string paramList)
        {
            throw new InvalidOperationException("Some crash.");
        }
    }
}