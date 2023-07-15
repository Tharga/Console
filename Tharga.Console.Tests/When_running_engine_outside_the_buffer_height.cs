using System.Threading.Tasks;
using FluentAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_running_engine_outside_the_buffer_height
    {
        [Fact]
        public void Should_a_short_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs("B"));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be("B");
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_a_full_line_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', console.BufferWidth)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(new string('B', console.BufferWidth));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_two_full_line_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 2 * console.BufferWidth)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 4].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('B', console.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(new string('B', console.BufferWidth));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_two_full_line_string_v2()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', console.BufferWidth + 1)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 4].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('B', console.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be("B");
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_two_full_line_string_with_offset()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(10);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 2))));

            //Act
            console.Output(new WriteEventArgs(new string('B', console.BufferWidth + 1)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 4].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('B', console.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be("B");
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_three_full_line_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 2))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 3 * console.BufferWidth)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 5].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 4].Should().Be(new string('B', console.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('B', console.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(new string('B', console.BufferWidth));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }
    }
}