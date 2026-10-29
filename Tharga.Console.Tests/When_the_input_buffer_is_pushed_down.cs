using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;
using Xunit;

namespace Tharga.Console.Tests
{
    // The handler used to write through System.Console, which other tests in this collection replace.
    [Collection(ProcessStateCollection.Name)]
    public class When_the_input_buffer_is_pushed_down
    {
        [Fact]
        public void Should_make_free_lines_after_the_input_and_put_the_cursor_back()
        {
            //Arrange: a prompt with an entry on the last line of the buffer.
            var keys = new ScriptedKeyInputEngine();
            var consoleManager = new FakeConsoleManager(keys);
            var console = new PushingConsole(consoleManager);
            var last = consoleManager.BufferHeight - 1;
            for (var i = 0; i < last; i++)
                console.Output(new WriteEventArgs($"Line {i}"));

            string[] screen = null;
            (int Left, int Top) cursor = default;
            keys.Keys("abc")
                .Then(() => console.PushBufferDown(2))
                .Then(() =>
                {
                    screen = (string[])consoleManager.LineOutput.Clone();
                    cursor = (consoleManager.CursorLeft, consoleManager.CursorTop);
                })
                .Keys("d\r");

            //Act
            var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

            //Assert: the screen scrolls two lines, which leaves two free lines after the entry, and the cursor stays in the entry.
            screen[last - 3].Should().Be($"Line {last - 1}");
            screen[last - 2].Should().Be(Constants.Prompt + "abc");
            screen[last - 1].Should().BeNullOrEmpty();
            screen[last].Should().BeNullOrEmpty();
            cursor.Should().Be((Constants.Prompt.Length + 3, last - 2));
            entry.Should().Be("abcd");
        }

        private class PushingConsole : ConsoleBase
        {
            public PushingConsole(IConsoleManager consoleManager)
                : base(consoleManager)
            {
            }

            public void PushBufferDown(int lineCount)
            {
                OnPushBufferDownEvent(lineCount);
            }
        }
    }
}
