using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_a_task_runner_is_closed_before_it_started
    {
        [Fact]
        public void Should_not_throw_when_closing_a_runner_that_was_never_started()
        {
            //Arrange
            var runner = new TaskRunner(() => { });

            //Act
            var act = () => runner.Close();

            //Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Should_write_no_error_when_the_engine_is_stopped_before_it_starts()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var root = new RootCommand(new TestConsole(consoleManager));
            var ran = false;
            var engine = new CommandEngine(root) { TaskRunners = new[] { new TaskRunner(() => ran = true) } };
            engine.Stop();

            //Act
            engine.Start(new[] { "exit" });

            //Assert
            consoleManager.LineOutput.Where(x => x != null && x.Contains("Exception")).Should().BeEmpty();
            ran.Should().BeTrue("the runners are started by the engine, not by a task that may never run");
        }
    }
}
