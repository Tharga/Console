using System.Threading.Tasks;
using NUnit.Framework;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class When_running_in_parallel
    {
        [Test]
        public void Should_work_with_consoles()
        {
            //Arrange
            var console1 = new TestConsole(new FakeConsoleManager());
            var console2 = new TestConsole(new FakeConsoleManager());

            //Act
            console1.Output(new WriteEventArgs("A"));

            //Assert
            Assert.That(console1.CursorLeft, Is.EqualTo(0));
            Assert.That(console2.CursorLeft, Is.EqualTo(0));
            Assert.That(console1.CursorTop, Is.EqualTo(1));
            Assert.That(console2.CursorTop, Is.EqualTo(0));
        }

        [Test]
        public void Should_work_with_consoles_v2()
        {
            //Arrange
            var console1 = new TestConsole(new FakeConsoleManager());
            var console2 = new TestConsole(new FakeConsoleManager());

            //Act
            console1.Output(new WriteEventArgs("A", lineFeed: false));

            //Assert
            Assert.That(console1.CursorLeft, Is.EqualTo(1));
            Assert.That(console2.CursorLeft, Is.EqualTo(0));
            Assert.That(console1.CursorTop, Is.EqualTo(0));
            Assert.That(console2.CursorTop, Is.EqualTo(0));
        }

        [Test]
        public void Should_work_with_commands()
        {
            ////Arrange
            //var console1 = new TestConsole(new FakeConsoleManager());
            //var console2 = new TestConsole(new FakeConsoleManager());
            //var command1 = new RootCommand(console1);
            //var command2 = new RootCommand(console2);

            ////Act
            //command1.Execute("help");

            ////Assert
            //Assert.That(console1.CursorLeft, Is.EqualTo(0));
            //Assert.That(console2.CursorLeft, Is.EqualTo(0));
            //Assert.That(console1.CursorTop, Is.EqualTo(0));
            //Assert.That(console2.CursorTop, Is.EqualTo(0));
        }

        [Test]
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
            Task.Run(() => { commandEngine1.Run(new string[] { }); }).Wait(100);
            Task.Run(() => { commandEngine2.Run(new string[] { }); }).Wait(100);
            console1.Output(new WriteEventArgs(new string('A', console1.BufferWidth * (console1.BufferHeight - 1))));

            //Act
            console1.Output(new WriteEventArgs("B"));

            //Assert
            Assert.That(consoleManager2.LineOutput[0], Is.EqualTo("> "));

            Assert.That(consoleManager1.LineOutput[0], Is.EqualTo(new string('A', consoleManager1.BufferWidth)));
            Assert.That(consoleManager1.LineOutput[consoleManager1.BufferHeight - 3], Is.EqualTo(new string('A', consoleManager1.BufferWidth)));
            Assert.That(consoleManager1.LineOutput[consoleManager1.BufferHeight - 2], Is.EqualTo("B"));
            Assert.That(consoleManager1.LineOutput[consoleManager1.BufferHeight - 1], Is.EqualTo("> "));
            Assert.That(consoleManager1.CursorTop, Is.EqualTo(consoleManager1.BufferHeight - 1));
            Assert.That(consoleManager1.CursorLeft, Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class When_writing_a_line
    {
        [Test]
        public void Should_write_short_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("A"));

            //Assert
            Assert.That(consoleManager.LineOutput, Is.Not.Empty);
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo("A"));
            Assert.That(consoleManager.LineOutput[1], Is.Null);
            Assert.That(consoleManager.CursorTop, Is.EqualTo(1));
        }

        [Test]
        public void Should_write_a_string_long_as_the_buffer_width()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', consoleManager.BufferWidth)));

            //Assert
            Assert.That(consoleManager.LineOutput, Is.Not.Empty);
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[1], Is.Null);
            Assert.That(consoleManager.CursorTop, Is.EqualTo(1));
        }

        [Test]
        public void Should_write_a_string_longer_than_the_buffer_width()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', consoleManager.BufferWidth + 1)));

            //Assert
            Assert.That(consoleManager.LineOutput, Is.Not.Empty);
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[1], Is.EqualTo("A"));
            Assert.That(consoleManager.LineOutput[2], Is.Null);
            Assert.That(consoleManager.CursorTop, Is.EqualTo(2));
        }

        [Test]
        public void Should_write_a_string_as_long_as_the_buffer_height()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', consoleManager.BufferWidth * (consoleManager.BufferHeight - 1))));

            //Assert
            Assert.That(consoleManager.LineOutput, Is.Not.Empty);
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight-2], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
        }

        [Test]
        public void Should_write_line_feeds_as_long_as_the_buffer_height()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('\n', consoleManager.BufferHeight - 2)));

            //Assert
            Assert.That(consoleManager.LineOutput, Is.Not.Empty);
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(string.Empty));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo(string.Empty));
            Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
        }
    }
}
