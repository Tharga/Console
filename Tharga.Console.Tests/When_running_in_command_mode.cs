using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_running_in_command_mode
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void Should_run_the_last_command_before_the_engine_stops()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(new[] { ConsoleKey.A, ConsoleKey.B, ConsoleKey.C, ConsoleKey.Enter }));
            var root = new RootCommand(new TestConsole(consoleManager));
            var command = new QueryCommand();
            root.RegisterCommand(command);
            var engine = new CommandEngine(root);

            //Act
            var run = Task.Run(() => engine.Start(new[] { "query" }));

            //Assert
            run.Wait(Timeout).Should().BeTrue("the engine stops after the last command");
            command.WasCancelled.Should().BeFalse();
            command.Value.Should().Be("ABC", "the missing parameter is queried");
        }

        private class QueryCommand : ActionCommandBase
        {
            public QueryCommand()
                : base("query")
            {
            }

            public bool WasCancelled { get; private set; }
            public string Value { get; private set; }

            public override void Invoke(string[] param)
            {
                WasCancelled = CancellationToken.IsCancellationRequested;
                Value = QueryParam<string>("Value", param);
            }
        }
    }
}
