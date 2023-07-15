using System;
using System.Threading;
using FluentAssertions;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests;

public class Entry_command_tests
{
    [Fact]
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
        r.Should().Be("ABC");
    }
}