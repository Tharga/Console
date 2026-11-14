using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_two_consoles_are_in_use
    {
        [Fact]
        public void Should_not_let_a_long_entry_in_one_console_disturb_output_in_the_other()
        {
            //Arrange
            var otherManager = new FakeConsoleManager();
            var otherConsole = new TestConsole(otherManager);
            var entryManager = new FakeConsoleManager();
            var entryKeys = new ScriptedKeyInputEngine()
                .Keys(new string('x', entryManager.BufferWidth + 1))
                .Then(() => otherConsole.Output(new WriteEventArgs("hello")))
                .Keys("\r");
            var entryConsole = new TestConsole(new FakeConsoleManager(entryKeys));

            //Act
            var entry = new InputInstance(entryConsole, Constants.Prompt, null, CancellationToken.None).ReadLine<string>(null, true);

            //Assert
            entry.Should().Be(new string('x', entryManager.BufferWidth + 1));
            otherManager.LineOutput[0].Should().Be("hello");
        }
    }
}
