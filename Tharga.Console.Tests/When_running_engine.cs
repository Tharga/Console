using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tharga.Console.Commands;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_running_engine
    {
        [Fact]
        public void Should_prompt_cursor()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);

            //Act
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            //Assert
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[0], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(0));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_prompt_cursor_after_line_output()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            //Act
            console.Output(new WriteEventArgs("A"));

            //Assert
            consoleManager.LineOutput[0].Should().Be("A");
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact(Skip = "Need to fix the buffer move function in FakeConsoleManager.")]
        public void Should_keep_buffer_after_line_output()
        {
            //Arrange
            var inputEngine = new Mock<IKeyInputEngine>(MockBehavior.Strict);
            inputEngine.Setup(x => x.ReadKey(It.IsAny<CancellationToken>())).Returns(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));
            var consoleManager = new FakeConsoleManager(inputEngine.Object);
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            //Act
            console.Output(new WriteEventArgs("A"));

            //Assert
            consoleManager.LineOutput[0].Should().Be("A");
            consoleManager.LineOutput[1].Should().Be($"{Constants.Prompt}A");
            consoleManager.CursorTop.Should().Be(1);
            consoleManager.CursorLeft.Should().Be(5);
        }

        [Fact]
        public void Should_prompt_cursor_after_full_line_output()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            //Act
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', console.BufferWidth));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Fact]
        public void Should_prompt_cursor_after_more_than_full_line_output()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Start(new string[] { }); }).Wait(100);

            //Act
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth + 1)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', console.BufferWidth));
            consoleManager.LineOutput[1].Should().Be("A");
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[2], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(2));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }
    }
}