using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_the_last_command_fails_with_the_e_flag
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void Should_keep_the_console_open_and_read_keys()
        {
            //Arrange
            using var keysRead = new ManualResetEventSlim();
            var keyEngine = new ScriptedKeyInputEngine().Then(() => keysRead.Set());
            var root = new RootCommand(new TestConsole(new FakeConsoleManager(keyEngine)));
            var engine = new CommandEngine(root);

            //Act
            var run = Task.Run(() => engine.Start(new[] { "nosuchcommand", "/e" }));
            var readKeys = keysRead.Wait(Timeout);
            engine.Stop();
            var stopped = run.Wait(Timeout);

            //Assert
            readKeys.Should().BeTrue("the engine continues in console mode after the failure");
            stopped.Should().BeTrue();
        }

        [Fact]
        public void Should_stop_when_the_last_command_fails_without_the_e_flag()
        {
            //Arrange
            using var keysRead = new ManualResetEventSlim();
            var keyEngine = new ScriptedKeyInputEngine().Then(() => keysRead.Set());
            var root = new RootCommand(new TestConsole(new FakeConsoleManager(keyEngine)));
            var engine = new CommandEngine(root);

            //Act
            var run = Task.Run(() => engine.Start(new[] { "nosuchcommand" }));

            //Assert
            run.Wait(Timeout).Should().BeTrue("the engine stops after the failed command");
            keysRead.IsSet.Should().BeFalse();
        }
    }
}
