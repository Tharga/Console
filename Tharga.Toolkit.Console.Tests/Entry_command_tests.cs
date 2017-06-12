using System;
using System.Threading;
using NUnit.Framework;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class Entry_command_tests
    {
        [Test]
        public void Should_return_the_string_that_was_entered()
        {
            //Arrange
            var input = new[] { ConsoleKey.A, ConsoleKey.B, ConsoleKey.C, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);

            var cancellationToken = new CancellationToken();
            var inputInstance = new InputInstance(console, Constants.Prompt, null, cancellationToken);

            //Act
            var r = inputInstance.ReadLine<string>(null, true);

            //Assert
            Assert.That(r, Is.EqualTo("ABC"));
        }
    }
}