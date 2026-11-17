using System;
using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_writing_on_a_console_with_a_background_colour
    {
        [Fact]
        public void Should_write_a_line_on_the_background_of_the_console()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager { BackgroundColor = ConsoleColor.DarkBlue };
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("hello"));
            console.OutputError("failed");

            //Assert
            consoleManager.Writes.Where(x => x.Value == "hello" || x.Value == "failed").Select(x => x.BackgroundColor).Should().OnlyContain(x => x == ConsoleColor.DarkBlue);
            consoleManager.BackgroundColor.Should().Be(ConsoleColor.DarkBlue);
        }

        [Fact]
        public void Should_use_the_given_background_for_a_line_and_restore_the_console_background()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager { BackgroundColor = ConsoleColor.DarkBlue };
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("hello", OutputLevel.Default, null, ConsoleColor.Red));

            //Assert
            consoleManager.Writes.Single(x => x.Value == "hello").BackgroundColor.Should().Be(ConsoleColor.Red);
            consoleManager.BackgroundColor.Should().Be(ConsoleColor.DarkBlue);
        }
    }
}
