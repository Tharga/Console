using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Tharga.Console.Commands;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_showing_help
    {
        [Fact]
        public void Should_write_the_heading_of_a_command_with_the_title_level_and_its_text_as_help()
        {
            //Arrange
            var written = Run("Documented -?");

            //Assert
            written.Single(x => x.Value == "Help for command documented.").Level.Should().Be(OutputLevel.Title);
            written.Single(x => x.Value == "Some help text.").Level.Should().Be(OutputLevel.Help);
        }

        [Fact]
        public void Should_write_the_headings_of_the_application_help_with_the_title_level()
        {
            //Arrange
            var written = Run("help");

            //Assert
            written.Single(x => x.Value == "How to use help.").Level.Should().Be(OutputLevel.Title);
            written.Single(x => x.Value == "Switches:").Level.Should().Be(OutputLevel.Title);
            written.Single(x => x.Value.StartsWith("Application ")).Level.Should().Be(OutputLevel.Title);
        }

        private static List<(string Value, OutputLevel Level)> Run(string entry)
        {
            var console = new TestConsole(new FakeConsoleManager());
            var written = new List<(string Value, OutputLevel Level)>();
            console.LineWrittenEvent += (_, e) => written.Add((e.Value, e.Level));
            var root = new RootCommand(console);
            var engine = new CommandEngine(root);
            root.RegisterCommand(new DocumentedCommand());

            engine.Start(new[] { entry });

            return written;
        }

        private class DocumentedCommand : ActionCommandBase
        {
            public DocumentedCommand()
                : base("Documented")
            {
            }

            public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Some help text."); } }

            public override void Invoke(string[] param)
            {
            }
        }
    }
}
