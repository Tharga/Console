using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    // The long query line tests leave the static input state of InputInstance (#59) describing a two-line entry.
    // Every line written in the process reads that state, so the class runs apart from the parallel tests.
    [Collection(ProcessStateCollection.Name)]
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

        // Pins the current, wrong placement; see #62. The output should be on line 2, below both lines of the entry.
        [Fact]
        public void Currently_outputs_inside_a_long_query_line_entered_with_cursor_at_end()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(LongEntry().Append(ConsoleKey.Enter).ToArray()));
            var console = new TestConsole(consoleManager);

            try
            {
                var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

                //Act
                console.Output(new WriteEventArgs("z"));

                //Assert
                entry.Should().Be(new string('X', consoleManager.BufferWidth + 1));
                consoleManager.LineOutput[0].Should().Be(Constants.Prompt + new string('X', consoleManager.BufferWidth - Constants.Prompt.Length));
                consoleManager.LineOutput[1].Should().Be("z");
                consoleManager.LineOutput[2].Should().Be(new string('X', Constants.Prompt.Length + 1), "the rest of the entry is pushed below the output");
            }
            finally
            {
                ResetInputState();
            }
        }

        // Pins the current, wrong placement; see #62. The output should be on line 2, below both lines of the entry.
        [Fact]
        public void Currently_outputs_inside_a_long_query_line_entered_with_cursor_at_beginning()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(LongEntry().Append(ConsoleKey.Home).Append(ConsoleKey.Enter).ToArray()));
            var console = new TestConsole(consoleManager);

            try
            {
                var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

                //Act
                console.Output(new WriteEventArgs("z"));

                //Assert
                entry.Should().Be(new string('X', consoleManager.BufferWidth + 1));
                consoleManager.LineOutput[0].Should().Be(Constants.Prompt + new string('X', consoleManager.BufferWidth - Constants.Prompt.Length));
                consoleManager.LineOutput[1].Should().Be("z");
                consoleManager.LineOutput[2].Should().Be(new string('X', Constants.Prompt.Length + 1), "the rest of the entry is pushed below the output");
            }
            finally
            {
                ResetInputState();
            }
        }

        [Fact]
        public void Should_write_above_an_entry_of_two_lines_that_ends_on_the_last_line_of_the_buffer()
        {
            //Arrange: the entry wraps from the second last line onto the last line, then a line is written.
            var keys = new ScriptedKeyInputEngine();
            var consoleManager = new FakeConsoleManager(keys);
            var console = new TestConsole(consoleManager);
            var last = consoleManager.BufferHeight - 1;
            for (var i = 0; i < last - 1; i++)
                console.Output(new WriteEventArgs($"Line {i}"));

            string[] screen = null;
            (int Left, int Top) cursor = default;
            keys.Keys(new string('X', consoleManager.BufferWidth + 1))
                .Then(() => console.Output(new WriteEventArgs("z")))
                .Then(() =>
                {
                    screen = (string[])consoleManager.LineOutput.Clone();
                    cursor = (consoleManager.CursorLeft, consoleManager.CursorTop);
                })
                .Keys("Y\r");

            try
            {
                //Act
                var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

                //Assert: the screen scrolls one line, the output goes above the entry and the entry stays whole.
                screen[last - 3].Should().Be($"Line {last - 2}");
                screen[last - 2].Should().Be("z");
                screen[last - 1].Should().Be(Constants.Prompt + new string('X', consoleManager.BufferWidth - Constants.Prompt.Length));
                screen[last].Should().Be(new string('X', Constants.Prompt.Length + 1));
                cursor.Should().Be((Constants.Prompt.Length + 1, last));
                entry.Should().Be(new string('X', consoleManager.BufferWidth + 1) + "Y");
            }
            finally
            {
                ResetInputState();
            }
        }

        [Fact]
        public void Should_truncate_a_single_line_to_the_width_of_the_console()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager(bufferWidth: 50);
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs(new string('A', 70), trunkateSingleLine: true));

            //Assert
            consoleManager.LineOutput[0].Should().Be(new string('A', 50));
            consoleManager.LineOutput[1].Should().BeNull();
        }

        // One character more than the buffer width, so with the prompt the entry wraps onto a second line.
        private static ConsoleKey[] LongEntry()
        {
            return Enumerable.Repeat(ConsoleKey.X, new FakeConsoleManager().BufferWidth + 1).ToArray();
        }

        // Puts the static input state back to a one-line entry with the cursor on it, as it is when no entry was long.
        private static void ResetInputState()
        {
            InputInstance.CursorLineOffset = 0;
            typeof(InputInstance).GetField("_currentBufferLineCount", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 0);
        }
    }
}
