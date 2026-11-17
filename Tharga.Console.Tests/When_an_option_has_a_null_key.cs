using System.Collections.Generic;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_an_option_has_a_null_key
    {
        private readonly PickCommand _command = new PickCommand();
        private readonly FakeConsoleManager _consoleManager = new FakeConsoleManager();
        private readonly CommandEngine _engine;

        public When_an_option_has_a_null_key()
        {
            var root = new RootCommand(new TestConsole(_consoleManager));
            _engine = new CommandEngine(root);
            root.RegisterCommand(_command);
        }

        [Fact]
        public void Should_select_another_option_by_its_key()
        {
            //Act
            _engine.Start(new[] { "Pick a" });

            //Assert
            _command.Picked.Should().Be("a");
            _consoleManager.LineOutput.Should().NotContain(x => x != null && x.Contains("Exception"));
        }

        [Fact]
        public void Should_select_the_null_key_option_by_its_text()
        {
            //Act
            _engine.Start(new[] { "Pick <none>" });

            //Assert
            _command.WasRun.Should().BeTrue();
            _command.Picked.Should().BeNull();
        }

        private class PickCommand : ActionCommandBase
        {
            public PickCommand()
                : base("Pick")
            {
                RegisterQuery("value", "Value", () => new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(null, "<none>"),
                    new KeyValuePair<string, string>("a", "Alpha")
                });
            }

            public bool WasRun { get; private set; }
            public string Picked { get; private set; }

            public override void Invoke(string[] param)
            {
                WasRun = true;
                Picked = GetParam<string>("value");
            }
        }
    }
}
