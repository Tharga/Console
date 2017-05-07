using System.Threading.Tasks;
using NUnit.Framework;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class When_running_engine_outside_the_buffer_height
    {
        [Test]
        public void Should_a_short_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Run(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs("B"));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo("B"));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Test]
        public void Should_a_full_line_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Run(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', console.BufferWidth)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo(new string('B', console.BufferWidth)));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Test]
        public void Should_two_full_line_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Run(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 2 * console.BufferWidth)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 4], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('B', console.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo(new string('B', console.BufferWidth)));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Test]
        public void Should_two_full_line_string_v2()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Run(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', console.BufferWidth + 1)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 4], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('B', console.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo("B"));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Test]
        public void Should_two_full_line_string_with_offset()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Run(new string[] { }); }).Wait(10);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 2))));

            //Act
            console.Output(new WriteEventArgs(new string('B', console.BufferWidth + 1)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 4], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('B', console.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo("B"));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }

        [Test]
        public void Should_three_full_line_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);
            Task.Run(() => { commandEngine.Run(new string[] { }); }).Wait(100);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 2))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 3 * console.BufferWidth)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 5], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 4], Is.EqualTo(new string('B', console.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('B', console.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo(new string('B', console.BufferWidth)));
            //TODO: Fix on build server! Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 1], Is.EqualTo("> "));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
            //TODO: Fix on build server! Assert.That(consoleManager.CursorLeft, Is.EqualTo(2));
        }
    }
}