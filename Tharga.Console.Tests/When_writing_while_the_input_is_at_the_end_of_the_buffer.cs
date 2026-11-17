using System;
using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_writing_while_the_input_is_at_the_end_of_the_buffer
    {
        [Fact]
        public void Should_scroll_the_screen_and_keep_the_prompt_on_the_last_line_for_multi_line_output()
        {
            //Arrange
            var keys = new ScriptedKeyInputEngine();
            var consoleManager = new FakeConsoleManager(keys);
            var console = new TestConsole(consoleManager);
            var last = consoleManager.BufferHeight - 1;
            for (var i = 0; i < last; i++)
                console.Output(new WriteEventArgs($"Line {i}"));

            string[] screen = null;
            (int Left, int Top) cursor = default;
            keys.Keys("abc")
                .Then(() => console.Output(new WriteEventArgs("one\ntwo\nthree")))
                .Then(() =>
                {
                    screen = (string[])consoleManager.LineOutput.Clone();
                    cursor = (consoleManager.CursorLeft, consoleManager.CursorTop);
                })
                .Keys("d\r");

            //Act
            var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

            //Assert
            screen[last - 4].Should().Be($"Line {last - 1}");
            screen[last - 3].Should().Be("one");
            screen[last - 2].Should().Be("two");
            screen[last - 1].Should().Be("three");
            screen[last].Should().Be(Constants.Prompt + "abc");
            cursor.Should().Be((Constants.Prompt.Length + 3, last));
            entry.Should().Be("abcd");
        }

        [Fact]
        public void Should_keep_both_lines_of_the_entry_and_the_cursor_when_writing_with_the_cursor_at_the_start()
        {
            //Arrange
            var keys = new ScriptedKeyInputEngine();
            var consoleManager = new FakeConsoleManager(keys);
            var console = new TestConsole(consoleManager);
            var last = consoleManager.BufferHeight - 1;
            for (var i = 0; i < last - 1; i++)
                console.Output(new WriteEventArgs($"Line {i}"));

            string[] screen = null;
            (int Left, int Top) cursor = default;
            keys.Keys(new string('x', consoleManager.BufferWidth + 1))
                .Key(ConsoleKey.Home)
                .Then(() =>
                {
                    console.Output(new WriteEventArgs("one\ntwo"));
                    screen = (string[])consoleManager.LineOutput.Clone();
                    cursor = (consoleManager.CursorLeft, consoleManager.CursorTop);
                })
                .Keys("\r");

            //Act
            var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

            //Assert
            screen[last - 3].Should().Be("one");
            screen[last - 2].Should().Be("two");
            screen[last - 1].Should().Be(Constants.Prompt + new string('x', consoleManager.BufferWidth - Constants.Prompt.Length));
            screen[last].Should().Be(new string('x', Constants.Prompt.Length + 1));
            cursor.Should().Be((Constants.Prompt.Length, last - 1));
            entry.Should().Be(new string('x', consoleManager.BufferWidth + 1));
        }
    }
}
