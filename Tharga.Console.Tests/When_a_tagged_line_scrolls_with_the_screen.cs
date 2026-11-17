using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_a_tagged_line_scrolls_with_the_screen
    {
        [Fact]
        public void Should_update_the_tagged_line_where_it_is_now()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            for (var i = 0; i < consoleManager.BufferHeight - 4; i++)
                console.Output(new WriteEventArgs($"Line {i}"));
            console.Output(new WriteEventArgs("progress", tag: "t"));
            for (var i = 0; i < 10; i++)
                console.Output(new WriteEventArgs($"After {i}"));

            //Act
            console.Output(new WriteEventArgs(" 50%", lineFeed: false, tag: "t"));

            //Assert
            consoleManager.LineOutput.Count(x => x == "progress 50%").Should().Be(1);
            consoleManager.LineOutput.Count(x => x != null && x.StartsWith("After")).Should().Be(10);
        }

        [Fact]
        public void Should_write_a_new_line_when_the_tagged_line_has_scrolled_out_of_the_buffer()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            console.Output(new WriteEventArgs("progress", tag: "t"));
            for (var i = 0; i < consoleManager.BufferHeight + 5; i++)
                console.Output(new WriteEventArgs($"After {i}"));

            //Act
            console.Output(new WriteEventArgs(" 50%", lineFeed: false, tag: "t"));

            //Assert
            consoleManager.LineOutput.Should().NotContain(x => x != null && x.Contains("progress"));
            consoleManager.LineOutput.Count(x => x == " 50%").Should().Be(1);
        }
    }
}
