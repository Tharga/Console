using System.Threading.Tasks;
using NUnit.Framework;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Toolkit.Console.Tests
{
    internal class TestAction : ActionCommandBase
    {
        internal bool WasRun;
        private string[] Params;

        public TestAction(string name) : base(name)
        {
        }

        public override void Invoke(string[] param)
        {
            WasRun = true;
            Params = param;
        }
    }

    internal class TestContainerCommand : ContainerCommandBase
    {
        internal TestAction inner = new TestAction("Inner");

        public TestContainerCommand() : base("Outer")
        {
            RegisterCommand(inner);
        }
    }

    [TestFixture]
    public class With_command_line
    {
        [SetUp]
        public void SetUp()
        {
            simple = new TestAction("Simple");
            containerCommand = new TestContainerCommand();
            command = new RootCommand(new TestConsole(new FakeConsoleManager()));
            commandEngine = new CommandEngine(command);
            command.RegisterCommand(simple);
            command.RegisterCommand(containerCommand);
        }

        private TestAction simple;
        private TestContainerCommand containerCommand;
        private CommandEngine commandEngine;
        private RootCommand command;

        [Test]
        public void With_no_args()
        {
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            Assert.False(simple.WasRun);
            Assert.False(containerCommand.inner.WasRun);
        }

        [Test]
        public void With_subcommand()
        {
            var args = new[] {"Outer Inner"};
            commandEngine.Start(args);

            Assert.False(simple.WasRun);
            Assert.True(containerCommand.inner.WasRun);
        }

        [Test]
        public void With_top_level_command()
        {
            var args = new[] {"Simple"};
            commandEngine.Start(args);

            Assert.True(simple.WasRun, "EXPECTED SIMPLE TO BE RUN");
            Assert.False(containerCommand.inner.WasRun, "EXPECTED INNER NOT TO BE RUN");
        }
    }
}