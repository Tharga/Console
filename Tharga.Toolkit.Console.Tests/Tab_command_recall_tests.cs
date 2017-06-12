using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;

namespace Tharga.Toolkit.Console.Tests
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
            var selection = new[] { new CommandTreeNode<string>("First", "1"), new CommandTreeNode<string>("Second", "2"), new CommandTreeNode<string>("Last", "x") };

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(selection.First().Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + selection.First().Value));
        }

        [Test]
        public void Should_recall_last_command_when_pressing_Shift_Tab()
        {
            //Arrange
            var input = new[] { new ConsoleKeyInfo('\t', ConsoleKey.Tab, true, false, false), new ConsoleKeyInfo((char)13, ConsoleKey.Enter, false, false, false) };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);
            var selection = new[] { new CommandTreeNode<string>("First", "1"), new CommandTreeNode<string>("Second", "2"), new CommandTreeNode<string>("Last", "x") };

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(selection.Last().Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + selection.Last().Value));
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
            var selection = new[] { new CommandTreeNode<string>("First", "1"), new CommandTreeNode<string>("Second", "2"), new CommandTreeNode<string>("Last", "x") };

            //Act
            var r = inputInstance.ReadLine(selection, true);

            //Assert
            Assert.That(r, Is.EqualTo(selection[1].Key));
            Assert.That(consoleManager.LineOutput.First(), Is.EqualTo(Constants.Prompt + selection[1].Value));
        }
    }
}
