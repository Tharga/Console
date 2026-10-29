using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_a_container_cannot_execute
    {
        private readonly FakeConsoleManager _consoleManager = new FakeConsoleManager();
        private readonly RootCommand _root;
        private readonly AccountCommand _account = new AccountCommand();

        public When_a_container_cannot_execute()
        {
            _root = new RootCommand(new TestConsole(_consoleManager));
            _root.RegisterCommand(_account);
            _ = new CommandEngine(_root);
        }

        [Fact]
        public void Should_not_run_a_sub_command_and_say_why()
        {
            //Act
            var success = _root.Execute("account list");

            //Assert
            success.Should().BeFalse();
            _account.List.WasRun.Should().BeFalse();
            Output().Should().Contain(x => x.Contains("Log in first. Inherited by parent."));
        }

        [Fact]
        public void Should_run_the_sub_command_once_the_container_can_execute()
        {
            //Arrange
            _root.Execute("account list");
            _account.LoggedIn = true;

            //Act
            var success = _root.Execute("account list");

            //Assert
            success.Should().BeTrue();
            _account.List.WasRun.Should().BeTrue();
        }

        [Fact]
        public void Should_run_the_sub_command_once_the_container_can_execute_after_asking_for_help()
        {
            //Arrange
            _root.Execute("help account list");
            _account.LoggedIn = true;

            //Act
            var success = _root.Execute("account list");

            //Assert
            success.Should().BeTrue();
            _account.List.WasRun.Should().BeTrue();
        }

        [Fact]
        public void Should_keep_the_can_execute_set_on_the_sub_command()
        {
            //Arrange: the sub command allows itself while the container refuses, then refuses itself.
            var allowed = true;
            _account.List.SetCanExecute(() => allowed ? null : "Not allowed.");
            _root.Execute("account list");
            _account.LoggedIn = true;
            allowed = false;

            //Act
            var refused = _root.Execute("account list");
            allowed = true;
            var success = _root.Execute("account list");

            //Assert
            refused.Should().BeFalse();
            Output().Last().Should().Contain("Not allowed.");
            success.Should().BeTrue();
            _account.List.WasRun.Should().BeTrue();
        }

        private string[] Output()
        {
            return _consoleManager.LineOutput.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        private class AccountCommand : ContainerCommandBase
        {
            public AccountCommand()
                : base("account")
            {
                RegisterCommand(List);
            }

            public TestAction List { get; } = new TestAction("list");

            public bool LoggedIn { get; set; }

            public override bool CanExecute(out string reasonMessage)
            {
                if (!LoggedIn)
                {
                    reasonMessage = "Log in first.";
                    return false;
                }

                return base.CanExecute(out reasonMessage);
            }
        }
    }
}
