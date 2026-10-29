using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    // A task runner writes its errors to Instance.Console, which is shared by the whole process.
    [Collection(ProcessStateCollection.Name)]
    public class When_running_an_async_task_runner
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void Close_should_wait_for_the_runner_to_finish()
        {
            //Arrange
            var started = new ManualResetEventSlim();
            var finished = false;
            var runner = new TaskRunner(async (cancellationToken, _) =>
            {
                started.Set();
                await WaitForCancellation(cancellationToken);
                await Task.Delay(100);
                finished = true;
            });
            runner.Start();
            started.Wait(Timeout).Should().BeTrue();

            //Act
            runner.Close();

            //Assert
            finished.Should().BeTrue();
        }

        [Fact]
        public void Close_should_throw_what_the_runner_threw_and_the_error_should_be_written()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager();
            var console = new TestConsole(consoleManager);
            var originalConsole = Instance.Console;
            Instance.Console = console;
            try
            {
                var started = new ManualResetEventSlim();
                var runner = new TaskRunner(async (_, _) =>
                {
                    started.Set();
                    await Task.Yield();
                    throw new InvalidOperationException("Failed in the runner.");
                });
                runner.Start();
                started.Wait(Timeout).Should().BeTrue();

                //Act
                var act = () => runner.Close();

                //Assert
                act.Should().Throw<InvalidOperationException>().WithMessage("Failed in the runner.");
                consoleManager.LineOutput.Should().Contain(x => x != null && x.Contains("Failed in the runner."));
            }
            finally
            {
                Instance.Console = originalConsole;
            }
        }

        [Fact]
        public void Close_should_not_throw_when_the_runner_ends_because_it_was_cancelled()
        {
            //Arrange
            var started = new ManualResetEventSlim();
            var runner = new TaskRunner(async (cancellationToken, _) =>
            {
                started.Set();
                await Task.Delay(System.Threading.Timeout.Infinite, cancellationToken);
            });
            runner.Start();
            started.Wait(Timeout).Should().BeTrue();

            //Act
            var act = () => runner.Close();

            //Assert
            act.Should().NotThrow();
        }

        private static async Task WaitForCancellation(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(System.Threading.Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
