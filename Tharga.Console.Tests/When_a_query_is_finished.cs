using System;
using System.Reflection;
using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_a_query_is_finished
    {
        [Fact]
        public void Should_remove_the_input_handlers_from_the_console()
        {
            //Arrange
            var console = new DisposeTrackingConsole(new FakeConsoleManager(new FakeKeyInputEngine(new[] { ConsoleKey.A, ConsoleKey.Enter })));

            //Act
            var entry = new InputManager(console).ReadLine<string>("Name", null, true, CancellationToken.None, null, null);

            //Assert
            entry.Should().Be("A");
            HandlerCount(console, nameof(IConsole.LinesInsertedEvent)).Should().Be(0);
            HandlerCount(console, nameof(IConsole.PushBufferDownEvent)).Should().Be(0);
        }

        [Fact]
        public void Should_not_dispose_the_console()
        {
            //Arrange
            var console = new DisposeTrackingConsole(new FakeConsoleManager(new FakeKeyInputEngine(new[] { ConsoleKey.A, ConsoleKey.Enter })));

            //Act
            new InputManager(console).ReadLine<string>("Name", null, true, CancellationToken.None, null, null);

            //Assert
            console.Disposed.Should().BeFalse("the console belongs to the application, not to the query");
        }

        private static int HandlerCount(ConsoleBase console, string eventName)
        {
            var handlers = (Delegate)typeof(ConsoleBase).GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(console);
            return handlers?.GetInvocationList().Length ?? 0;
        }

        private class DisposeTrackingConsole : ConsoleBase
        {
            public DisposeTrackingConsole(IConsoleManager consoleManager)
                : base(consoleManager)
            {
                DisposeEvent += (_, _) => Disposed = true;
            }

            public bool Disposed { get; private set; }
        }
    }
}
