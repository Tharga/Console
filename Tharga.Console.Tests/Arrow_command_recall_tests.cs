using System;
using System.Threading;
using AwesomeAssertions;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests;

public class Arrow_command_recall_tests
{
    [Fact]
    public void Should_recall_last_executed_when_pressing_arrow_up()
    {
        //Arrange
        const string paramName = "Arrow up recall";
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var earlierEntry = new InputInstance(new TestConsole(new FakeConsoleManager(new FakeKeyInputEngine(new[] { ConsoleKey.A, ConsoleKey.B, ConsoleKey.C, ConsoleKey.Enter }))), paramName, null, timeout.Token);
        earlierEntry.ReadLine<string>(null, true);

        var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(new[] { ConsoleKey.UpArrow, ConsoleKey.Enter }));
        var inputInstance = new InputInstance(new TestConsole(consoleManager), paramName, null, timeout.Token);

        //Act
        var r = inputInstance.ReadLine<string>(null, true);

        //Assert
        r.Should().Be("ABC");
        consoleManager.LineOutput[0].Should().Be($"{paramName}: ABC");
    }
}