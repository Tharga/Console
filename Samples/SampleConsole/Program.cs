using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = new RootCommand();
            command.RegisterCommand(new SomeContainerCommand());
            new CommandEngine(command).Run(args);
        }
    }

    class SomeContainerCommand : ContainerCommandBase
    {
        public SomeContainerCommand() 
            : base("some")
        {
            RegisterCommand(new SomeListCommand());
            RegisterCommand(new SomeItemCommand());
        }
    }

    class SomeItemCommand : ActionCommandBase
    {
        public SomeItemCommand()
            : base("item", "Gets a single item")
        {

        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var id = QueryParam<Guid>("Some Id", GetParam(paramList, index), KeyNameList);

            OutputInformation("Some data for {0}", id);

            return true;
        }

        private List<KeyValuePair<Guid, string>> KeyNameList()
        {
            return new List<KeyValuePair<Guid, string>>() { new KeyValuePair<Guid, string>(Guid.Parse("4779177e-2c27-432a-825d-22f9f151391e"), "A"), new KeyValuePair<Guid, string>(Guid.NewGuid(), "B") };
        }
    }

    class SomeListCommand : ActionCommandBase
    {
        public SomeListCommand() 
            : base("list", "Lists some information")
        {

        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            for (var i = 0; i < 5; i++)
                OutputInformation("Some data {0}", i);
            return true;
        }
    }
}
