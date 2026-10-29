using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Console.Consoles;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_reading_from_a_null_console
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void Should_wait_for_a_key_until_the_read_is_cancelled()
        {
            //Arrange
            using var console = new NullConsole();
            using var cancellationTokenSource = new CancellationTokenSource();

            //Act
            var read = Task.Run(() => console.ReadKey(cancellationTokenSource.Token));
            var returnedAtOnce = read.Wait(TimeSpan.FromMilliseconds(200));
            cancellationTokenSource.Cancel();

            //Assert
            returnedAtOnce.Should().BeFalse("there is no key to read");
            var act = () => read.Wait(Timeout);
            act.Should().Throw<OperationCanceledException>();
        }

        [Fact]
        public void Should_end_a_query_with_an_empty_entry_when_cancelled()
        {
            //Arrange
            using var console = new NullConsole();
            using var cancellationTokenSource = new CancellationTokenSource();
            var inputInstance = new InputInstance(console, "Name", null, cancellationTokenSource.Token);

            //Act
            var query = Task.Run(() => inputInstance.ReadLine<string>(null, true));
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));

            //Assert
            query.Wait(Timeout).Should().BeTrue("the query should end when it is cancelled");
            query.Result.Should().BeEmpty();
        }
    }
}
