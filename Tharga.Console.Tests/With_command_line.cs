using System.Threading.Tasks;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Xunit;

namespace Tharga.Console.Tests
{

    class TestAction : ActionCommandBase
    {
        internal bool WasRun = false;
        string[] Params = null;
        public TestAction(string name) : base(name) { }

        public override void Invoke(string[] param)
        {
            WasRun = true;
            Params = param;
        }
    }

    class TestContainerCommand : ContainerCommandBase
    {
        internal TestAction inner = new TestAction("Inner");
        public TestContainerCommand() : base("Outer")
        {
            RegisterCommand(this.inner);
        }
    }

    public class With_command_line
    {
        private TestAction simple;
        private TestContainerCommand containerCommand;
        private CommandEngine commandEngine;
        private RootCommand command;

        public With_command_line()
        {
            this.simple = new TestAction("Simple");
            this.containerCommand = new TestContainerCommand();
            this.command = new RootCommand(new TestConsole(new FakeConsoleManager()));
            this.commandEngine = new CommandEngine(this.command);
            command.RegisterCommand(this.simple);
            command.RegisterCommand(this.containerCommand);
        }

        [Fact]
        public void With_no_args()
        {
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            Assert.False(simple.WasRun);
            Assert.False(containerCommand.inner.WasRun);
        }

        [Fact]
        public void With_top_level_command()
        {
            var args = new string[] { "Simple" };
            commandEngine.Start(args);

            Assert.True(simple.WasRun, "EXPECTED SIMPLE TO BE RUN");
            Assert.False(containerCommand.inner.WasRun, "EXPECTED INNER NOT TO BE RUN");
        }

        [Fact]
        public void With_subcommand()
        {
            var args = new string[] { "Outer Inner" };
            commandEngine.Start(args);

            Assert.False(simple.WasRun);
            Assert.True(containerCommand.inner.WasRun);
        }
    }
}