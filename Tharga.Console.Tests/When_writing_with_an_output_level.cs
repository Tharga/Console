using System;
using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_writing_with_an_output_level
    {
        [Fact]
        public void Should_use_the_level_colour_for_a_line()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("text", OutputLevel.Warning));

            //Assert
            consoleManager.Writes.Single(x => x.Value == "text").ForegroundColor.Should().Be(ConsoleColor.Yellow);
        }

        [Fact]
        public void Should_use_the_level_colour_without_a_line_feed()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("text", OutputLevel.Warning, lineFeed: false));

            //Assert
            consoleManager.Writes.Single(x => x.Value == "text").ForegroundColor.Should().Be(ConsoleColor.Yellow);
        }

        [Fact]
        public void Should_use_the_given_colours_without_a_line_feed()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("text", OutputLevel.Default, ConsoleColor.Magenta, ConsoleColor.DarkBlue, lineFeed: false));

            //Assert
            var write = consoleManager.Writes.Single(x => x.Value == "text");
            write.ForegroundColor.Should().Be(ConsoleColor.Magenta);
            write.BackgroundColor.Should().Be(ConsoleColor.DarkBlue);
        }

        [Fact]
        public void Should_restore_the_colours_after_writing_without_a_line_feed()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkBlue };
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("text", OutputLevel.Error, lineFeed: false));

            //Assert
            consoleManager.ForegroundColor.Should().Be(ConsoleColor.Gray);
            consoleManager.BackgroundColor.Should().Be(ConsoleColor.DarkBlue);
        }

        [Fact]
        public void Should_keep_the_console_background_for_default_output_without_a_line_feed()
        {
            //Arrange: typed input and the prompt are written this way, on whatever background the console has.
            var consoleManager = new FakeConsoleManager { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.DarkBlue };
            var console = new TestConsole(consoleManager);

            //Act
            console.Output(new WriteEventArgs("a", OutputLevel.Default, null, null, false, false));

            //Assert
            var write = consoleManager.Writes.Single(x => x.Value == "a");
            write.ForegroundColor.Should().Be(ConsoleColor.Gray);
            write.BackgroundColor.Should().Be(ConsoleColor.DarkBlue);
        }

        [Fact]
        public void Should_use_the_level_colour_when_updating_a_tagged_line_in_place()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs("a", OutputLevel.Warning, lineFeed: false, tag: "progress"));

            //Act
            console.Output(new WriteEventArgs("b", OutputLevel.Warning, lineFeed: false, tag: "progress"));

            //Assert
            consoleManager.LineOutput[0].Should().Be("ab");
            consoleManager.Writes.Single(x => x.Value == "b").ForegroundColor.Should().Be(ConsoleColor.Yellow);
        }
    }
}
