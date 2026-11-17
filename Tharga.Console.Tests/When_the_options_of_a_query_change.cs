using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_the_options_of_a_query_change
    {
        private readonly List<KeyValuePair<int, string>> _users = new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(1, "Alice") };
        private readonly PickUserCommand _command;
        private readonly CommandEngine _engine;

        public When_the_options_of_a_query_change()
        {
            _command = new PickUserCommand(_users);
            var root = new RootCommand(new TestConsole(new FakeConsoleManager()));
            _engine = new CommandEngine(root);
            root.RegisterCommand(_command);
        }

        [Fact]
        public void Should_select_an_option_added_after_the_command_was_created()
        {
            //Arrange
            _users.Add(new KeyValuePair<int, string>(2, "Bob"));

            //Act
            _engine.Start(new[] { "PickUser Bob" });

            //Assert
            _command.Picked.Should().Be(2);
        }

        [Fact]
        public void Should_list_an_option_added_after_the_command_was_created_for_tab_completion()
        {
            //Arrange
            _users.Add(new KeyValuePair<int, string>(2, "Bob"));

            //Act
            var options = _command.GetOptionList().Single().ToArray();

            //Assert
            options.Should().Equal("Alice", "Bob");
        }

        private class PickUserCommand : ActionCommandBase
        {
            public PickUserCommand(List<KeyValuePair<int, string>> users)
                : base("PickUser")
            {
                RegisterQuery("user", "User", () => users.ToList());
            }

            public int Picked { get; private set; }

            public override void Invoke(string[] param)
            {
                Picked = GetParam<int>("user");
            }
        }
    }
}
