using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;

namespace Tharga.Console.Tests
{
    [TestFixture]
    public class Tab_command_recall_tests
    {
        [Test]
        public void Should_recall_nothing_when_pressing_Tab_and_no_commands_are_provided()
        {
            //Arrange
            var input = new[] { ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);

            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);

            //Act
            var r = inputInstance.ReadLine<string>(null, true);

            //Assert
            Assert.That(r, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Should_recall_first_command_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>( new[] { new CommandTreeNode<string>("First", "1"), new CommandTreeNode<string>("Second", "2"), new CommandTreeNode<string>("Last", "x") });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(selection.Subs.First().Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + selection.Subs.First().Value));
        }

        [Test]
        [Ignore("Fix this issue")]
        public void Should_recall_last_command_when_pressing_Shift_Tab()
        {
            //Arrange
            var input = new[] { new ConsoleKeyInfo('\t', ConsoleKey.Tab, true, false, false), new ConsoleKeyInfo((char)13, ConsoleKey.Enter, false, false, false) };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>(new[] { new CommandTreeNode<string>("First", "1"), new CommandTreeNode<string>("Second", "2"), new CommandTreeNode<string>("Last", "x") });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            var match = selection.Subs.Last();
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + match.Value));
        }

        [Test]
        public void Should_recall_second_command_when_pressing_Tab_Twise()
        {
            //Arrange
            var input = new[] { ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>( new[] { new CommandTreeNode<string>("First", "1"), new CommandTreeNode<string>("Second", "2"), new CommandTreeNode<string>("Last", "x") });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(selection.Subs[1].Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + selection.Subs[1].Value));
        }

        [Test]
        public void Should_recall_first_command_with_a_specific_letter_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two"),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            var match = selection.Subs.ToArray()[1];
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + match.Value));
        }

        [Test]
        public void Should_recall_first_command_with_a_specific_part_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.W, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    new CommandTreeNode<string>("FirstSub", "SubOne"),
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            var match = selection.Subs.ToArray()[1];
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + match.Value));
        }

        [Test]
        public void Should_recall_first_command_with_a_specific_command_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.W, ConsoleKey.O, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    new CommandTreeNode<string>("FirstSub", "SubOne"),
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            var match = selection.Subs.ToArray()[1];
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + match.Value));
        }

        [Test]
        public void Should_recall_first_command_with_a_specific_command_with_trailing_space_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.W, ConsoleKey.O, ConsoleKey.Spacebar, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var match = new CommandTreeNode<string>("FirstSub", "SubOne");
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    match,
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + "Two " + match.Value));
        }

        [Test]
        public void Should_recall_first_sub_command_when_matching_a_sub_part_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.W, ConsoleKey.O, ConsoleKey.Spacebar, ConsoleKey.F, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var match = new CommandTreeNode<string>("FirstSub", "SubOne");
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    match,
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + "Two " + match.Value));
        }

        [Test]
        public void Should_recall_first_sub_command_when_matching_a_sub_part_with_similar_root_name_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.W, ConsoleKey.O, ConsoleKey.Spacebar, ConsoleKey.F, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var match = new CommandTreeNode<string>("FirstSub", "SubOne");
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    match,
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("SecondOther", "TwoOther"),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + "Two " + match.Value));
        }

        [Test]
        public void Should_recall_first_command_when_invalid_letter_letter_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.Z, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two"),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            var match = selection.Subs.First();
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + match.Value));
        }

        [Test]
        public void Should_recall_command_in_several_layers_when_pressing_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.W, ConsoleKey.O, ConsoleKey.Spacebar, ConsoleKey.O, ConsoleKey.N, ConsoleKey.E, ConsoleKey.Spacebar, ConsoleKey.S, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var match = new CommandTreeNode<string>("SecondSub", "S");
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    new CommandTreeNode<string>("First", "One", new []
                    {
                        new CommandTreeNode<string>("FirstSub", "ThirdOne"),
                        match,
                        new CommandTreeNode<string>("LastSub", "ThirdX"),
                    }),
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + "Two One " + match.Value));
        }

        [Test]
        public void Should_recall_command_in_several_layers_when_navigating_with_Tab()
        {
            //Arrange
            var input = new[] { ConsoleKey.T, ConsoleKey.Tab, ConsoleKey.Spacebar, ConsoleKey.O, ConsoleKey.Tab, ConsoleKey.Spacebar, ConsoleKey.S, ConsoleKey.Tab, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var match = new CommandTreeNode<string>("SecondSub", "S");
            var selection = new CommandTreeNode<string>(new[]
            {
                new CommandTreeNode<string>("First", "One"),
                new CommandTreeNode<string>("Second", "Two", new []
                {
                    new CommandTreeNode<string>("FirstA", "One", new []
                    {
                        new CommandTreeNode<string>("FirstSub", "ThirdOne"),
                        match,
                        new CommandTreeNode<string>("LastSub", "ThirdX"),
                    }),
                    new CommandTreeNode<string>("SecondSub", "SubTwo"),
                    new CommandTreeNode<string>("LastSub", "SubX"),
                }),
                new CommandTreeNode<string>("Last", "x")
            });

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(match.Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + "Two One " + match.Value));
        }
    }
}