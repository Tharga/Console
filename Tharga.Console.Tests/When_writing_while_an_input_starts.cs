using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_writing_while_an_input_starts
    {
        [Fact]
        public void Should_write_a_line_that_arrives_while_the_input_is_created()
        {
            //Arrange: output from another thread arrives while the new input reads the cursor position.
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            consoleManager.OnNextCursorRead = () => console.Output(new WriteEventArgs("startup"));

            //Act
            var act = () => new InputInstance(console, Constants.Prompt, null, CancellationToken.None);

            //Assert
            act.Should().NotThrow();
            consoleManager.LineOutput[0].Should().Be("startup");
            consoleManager.LineOutput[1].Should().BeNull();
        }
    }
}
