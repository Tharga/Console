using NUnit.Framework;
using Tharga.Console.Entities;

namespace Tharga.Console.Tests
{
    [TestFixture]
    public class When_writing_outside_the_buffer_height
    {
        [Test]
        public void Should_a_short_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs("B"));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo("B"));
            Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
        }

        [Test]
        public void Should_write_one_rows()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 1 * console.BufferWidth)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo(new string('B', consoleManager.BufferWidth)));
            Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
        }

        [Test]
        public void Should_write_two_rows()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 2 * console.BufferWidth)));

            //Assert
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 4], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('B', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo(new string('B', consoleManager.BufferWidth)));
            Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
        }

        [Test]
        public void Should_write_two_rows_v2()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 1 * console.BufferWidth + 1)));

            //Assert
            Assert.That(consoleManager.LineOutput, Is.Not.Empty);
            Assert.That(consoleManager.LineOutput[0], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 4], Is.EqualTo(new string('A', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 3], Is.EqualTo(new string('B', consoleManager.BufferWidth)));
            Assert.That(consoleManager.LineOutput[consoleManager.BufferHeight - 2], Is.EqualTo("B"));
            Assert.That(consoleManager.CursorTop, Is.EqualTo(consoleManager.BufferHeight - 1));
        }
    }
}