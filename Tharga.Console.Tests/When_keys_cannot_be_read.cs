using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    // Console.ReadKey throws InvalidOperationException when standard input is redirected.
    public class When_keys_cannot_be_read
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void Reading_a_key_should_throw_instead_of_waiting()
        {
            //Arrange
            var keyInputEngine = new KeyInputEngine(() => throw new InvalidOperationException("Input is redirected."));

            //Act
            var read = Task.Run(() => keyInputEngine.ReadKey(CancellationToken.None));

            //Assert
            var act = () => read.Wait(Timeout).Should().BeTrue("a read should not wait for keys that cannot come");
            act.Should().Throw<InvalidOperationException>().WithMessage("*Input is redirected.*");
        }

        [Fact]
        public void Keys_read_before_the_failure_should_be_returned_first()
        {
            //Arrange
            var reads = 0;
            var keyInputEngine = new KeyInputEngine(() => reads++ == 0
                ? new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false)
                : throw new InvalidOperationException("Input is redirected."));

            //Act
            var read = Task.Run(() => keyInputEngine.ReadKey(CancellationToken.None));

            //Assert
            read.Wait(Timeout).Should().BeTrue();
            read.Result.KeyChar.Should().Be('a');
        }

        [Fact]
        public void The_engine_should_stop_and_say_why()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager(new KeyInputEngine(() => throw new InvalidOperationException("Input is redirected.")));
            var engine = new CommandEngine(new RootCommand(new TestConsole(consoleManager)));

            //Act
            var run = Task.Run(() => engine.Start(new string[] { }));

            //Assert
            run.Wait(Timeout).Should().BeTrue("the engine should not wait for keys that cannot come");
            consoleManager.LineOutput.Where(x => x != null).Should().Contain(x => x.Contains("Input is redirected."));
        }
    }
}
