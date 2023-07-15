using System.Threading.Tasks;
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

        [Fact]
        public void Should_work_with_engines()
        {
            //Arrange
            var consoleManager1 = new FakeConsoleManager();
            var consoleManager2 = new FakeConsoleManager();
            var console1 = new TestConsole(consoleManager1);
            var console2 = new TestConsole(consoleManager2);
            var command1 = new RootCommand(console1);
            var command2 = new RootCommand(console2);
            var commandEngine1 = new CommandEngine(command1);
            var commandEngine2 = new CommandEngine(command2);
            Task.Run(() => { commandEngine1.Start(new string[] { }); }).Wait(100);
            Task.Run(() => { commandEngine2.Start(new string[] { }); }).Wait(100);
            console2.Output(new WriteEventArgs(new string('C', console1.BufferWidth * (console1.BufferHeight - 1))));
            console1.Output(new WriteEventArgs(new string('A', console1.BufferWidth * (console1.BufferHeight - 1))));

            //Act
            console1.Output(new WriteEventArgs("B"));

            //Assert
            consoleManager1.LineOutput[0].Should().Be(new string('A', consoleManager1.BufferWidth));
            consoleManager1.LineOutput[consoleManager1.BufferHeight - 3].Should().Be(new string('A', consoleManager1.BufferWidth));
            consoleManager1.LineOutput[consoleManager1.BufferHeight - 2].Should().Be("B");
            //Assert.That(consoleManager1.LineOutput[consoleManager1.BufferHeight - 1], Is.EqualTo("> "));
            //Assert.That(consoleManager1.CursorTop, Is.EqualTo(consoleManager1.BufferHeight - 1));
            //Assert.That(consoleManager1.CursorLeft, Is.EqualTo(2));

            consoleManager2.LineOutput[0].Should().Be(new string('C', consoleManager1.BufferWidth));
            consoleManager2.LineOutput[consoleManager1.BufferHeight - 3].Should().Be(new string('C', consoleManager1.BufferWidth));
            //Assert.That(consoleManager2.LineOutput[0], Is.EqualTo("> "));
        }
    }
}