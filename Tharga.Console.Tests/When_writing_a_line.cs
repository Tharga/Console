using FluentAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_writing_a_line
    {
        [Fact]
        public void Should_write_short_string()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("A"));

            //Assert
            consoleManager.LineOutput.Should().NotBeEmpty();
            consoleManager.LineOutput[0].Should().Be("A");
            consoleManager.LineOutput[1].Should().BeNull();
            consoleManager.CursorTop.Should().Be(1);
        }

        [Fact]
        public void Should_write_a_string_long_as_the_buffer_width()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', consoleManager.BufferWidth)));

            //Assert
            consoleManager.LineOutput.Should().NotBeEmpty();
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[1].Should().BeNull();
            consoleManager.CursorTop.Should().Be(1);
        }

        [Fact]
        public void Should_write_a_string_longer_than_the_buffer_width()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', consoleManager.BufferWidth + 1)));

            //Assert
            consoleManager.LineOutput.Should().NotBeEmpty();
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[1].Should().Be("A");
            consoleManager.LineOutput[2].Should().BeNull();
            consoleManager.CursorTop.Should().Be(2);
        }

        [Fact]
        public void Should_write_a_string_as_long_as_the_buffer_height()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', consoleManager.BufferWidth * (consoleManager.BufferHeight - 1))));

            //Assert
            consoleManager.LineOutput.Should().NotBeEmpty();
            consoleManager.LineOutput[0].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(new string('A', consoleManager.BufferWidth));
            consoleManager.CursorTop.Should().Be(consoleManager.BufferHeight - 1);
        }

        [Fact]
        public void Should_write_line_feeds_as_long_as_the_buffer_height()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('\n', consoleManager.BufferHeight - 2)));

            //Assert
            consoleManager.LineOutput.Should().NotBeEmpty();
            consoleManager.LineOutput[0].Should().Be(string.Empty);
            consoleManager.LineOutput[consoleManager.BufferHeight - 2].Should().Be(string.Empty);
            consoleManager.CursorTop.Should().Be(consoleManager.BufferHeight - 1);
        }

        [Fact]
        //TODO: There is an issue with this case, find a good way to set up the test for this.
        public void Should_output_on_new_line_when_entering_a_long_query_line_with_cursor_at_end()
        {
            //TODO: Prepare a query entry that is longer than one line
            //TODO: Place cursor at end of the input buffer and press enter.

            //Arrange
            var fakeKeyInput = new FakeKeyInputEngine();
            var consoleManager = new FakeConsoleManager(fakeKeyInput);
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);
            var commandEngine = new CommandEngine(command);

            console.Output(new WriteEventArgs("a"));
            command.Execute(new string('x', console.BufferWidth + 1));
            console.Output(new WriteEventArgs("z"));

            //Assert.That(consoleManager.LineOutput[0], Is.EqualTo("abc"));
            //Assert.That(consoleManager.LineOutput[1], Is.Null);

            //Act
            //Assert
        }

        [Fact]
        //TODO: There is an issue with this case, find a good way to set up the test for this.
        public void Should_output_on_new_line_when_entering_a_long_query_line_with_cursor_at_beginning()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            //TODO: Prepare a query entry that is longer than one line
            //TODO: Place cursor at start (on first line) and press enter.

            //Act
            //TODO: Press enter

            //Assert
            //TODO: See where the response output appears
        }
    }
}
