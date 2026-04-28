using FluentAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_running_in_parallel
    {
        [Fact]
        public void Should_work_with_consoles()
        {
            //Arrange
            var console1 = new TestConsole(new FakeConsoleManager());
            var console2 = new TestConsole(new FakeConsoleManager());

            //Act
            console1.Output(new WriteEventArgs("A"));

            //Assert
            console1.CursorLeft.Should().Be(0);
            console2.CursorLeft.Should().Be(0);
            console1.CursorTop.Should().Be(1);
            console2.CursorTop.Should().Be(0);
        }

        [Fact]
        public void Should_work_with_consoles_v2()
        {
            //Arrange
            var console1 = new TestConsole(new FakeConsoleManager());
            var console2 = new TestConsole(new FakeConsoleManager());

            //Act
            console1.Output(new WriteEventArgs("A", lineFeed: false));

            //Assert
            console1.CursorLeft.Should().Be(1);
            console2.CursorLeft.Should().Be(0);
            console1.CursorTop.Should().Be(0);
            console2.CursorTop.Should().Be(0);
        }

        [Fact]
        public void Should_work_with_commands()
        {
            //Arrange
            var console1 = new TestConsole(new FakeConsoleManager());
            var console2 = new TestConsole(new FakeConsoleManager());
            var command1 = new RootCommand(console1);
            var command2 = new RootCommand(console2);
            var commandEngine1 = new CommandEngine(command1);
            var commandEngine2 = new CommandEngine(command2);

            //Act
            command1.Execute("help");

            //Assert
            console1.CursorLeft.Should().Be(0);
            console2.CursorLeft.Should().Be(0);
            console1.CursorTop.Should().Be(26);
            console2.CursorTop.Should().Be(0);
        }

    }
}