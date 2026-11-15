using System.Linq;
using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_an_entry_wraps_on_the_last_line_of_the_buffer
    {
        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public void Should_return_the_entry_in_order_without_errors(int promptLineFromLast)
        {
            //Arrange
            var consoleManager = new FakeConsoleManager(new ScriptedKeyInputEngine().Keys(new string('x', 79) + "yz" + new string('w', 20) + "\r"));
            consoleManager.SetCursorPosition(0, consoleManager.BufferHeight - 1 - promptLineFromLast);
            var console = new TestConsole(consoleManager);

            //Act
            var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

            //Assert
            entry.Should().Be(new string('x', 79) + "yz" + new string('w', 20));
            consoleManager.LineOutput.Where(x => x != null && x.Contains("Exception")).Should().BeEmpty("no key is lost to an error");
        }

        [Fact]
        public void Should_keep_the_prompt_and_the_entry_on_the_last_two_lines()
        {
            //Arrange
            string[] screen = null;
            FakeConsoleManager consoleManager = null;
            var keys = new ScriptedKeyInputEngine()
                .Keys(new string('x', 79) + "yz")
                .Then(() => screen = consoleManager.LineOutput.ToArray())
                .Keys("\r");
            consoleManager = new FakeConsoleManager(keys);
            consoleManager.SetCursorPosition(0, consoleManager.BufferHeight - 1);
            var console = new TestConsole(consoleManager);

            //Act
            var entry = new InputInstance(console, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

            //Assert
            entry.Should().Be(new string('x', 79) + "yz");
            screen[consoleManager.BufferHeight - 2].Should().Be(Constants.Prompt + new string('x', consoleManager.BufferWidth - Constants.Prompt.Length));
            screen[consoleManager.BufferHeight - 1].Should().Be("xyz");
        }
    }
}
