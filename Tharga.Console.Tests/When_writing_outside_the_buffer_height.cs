using FluentAssertions;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_writing_outside_the_buffer_height
    {
        [Fact]
        public void Should_a_short_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs("B"));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be("B");
            consoleManager.CursorTop.Should().Be(consoleManager.BufferHeight - 1);
        }

        [Fact]
        public void Should_write_one_rows()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 1 * console.BufferWidth)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(new string('B', consoleManager.BufferWidth));
            consoleManager.CursorTop.Should().Be(consoleManager.BufferHeight - 1);
        }

        [Fact]
        public void Should_write_two_rows()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 2 * console.BufferWidth)));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 4].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('B', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(new string('B', consoleManager.BufferWidth));
            consoleManager.CursorTop.Should().Be(consoleManager.BufferHeight - 1);
        }

        [Fact]
        public void Should_write_two_rows_v2()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs(new string('A', console.BufferWidth * (console.BufferHeight - 1))));

            //Act
            console.Output(new WriteEventArgs(new string('B', 1 * console.BufferWidth + 1)));

            //Assert
            consoleManager.LineOutput.Should().NotBeEmpty();
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 4].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 3].Should().Be(new string('B', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be("B");
            consoleManager.CursorTop.Should().Be(consoleManager.BufferHeight - 1);
        }
    }
}