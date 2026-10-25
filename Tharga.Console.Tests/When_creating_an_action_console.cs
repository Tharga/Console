using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Tharga.Console.Consoles;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;
using Xunit;

namespace Tharga.Console.Tests
{
    public class When_creating_an_action_console
    {
        [Fact]
        public void Should_apply_the_configuration()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.Black };
            var outputs = new List<IActionConsoleOutput>();
            var configuration = new ConsoleConfiguration
            {
                Title = "Some title",
                SplashScreen = "Welcome",
                ShowAssemblyInfo = false,
                DefaultTextColor = ConsoleColor.Yellow,
                BackgroundColor = ConsoleColor.DarkBlue
            };

            //Act
            _ = new ActionConsole(consoleManager, outputs.Add, configuration);

            //Assert
            outputs.Should().ContainSingle(x => x.Message == "Welcome");
            consoleManager.Title.Should().Be("Some title");
            consoleManager.ForegroundColor.Should().Be(ConsoleColor.Yellow);
            consoleManager.BackgroundColor.Should().Be(ConsoleColor.DarkBlue);
        }

        [Fact]
        public void Should_apply_nothing_without_a_configuration()
        {
            //Arrange
            var consoleManager = new FakeConsoleManager { ForegroundColor = ConsoleColor.Gray, BackgroundColor = ConsoleColor.Black };
            var outputs = new List<IActionConsoleOutput>();

            //Act
            _ = new ActionConsole(consoleManager, outputs.Add, null);

            //Assert
            outputs.Should().BeEmpty();
            consoleManager.Title.Should().BeNull();
            consoleManager.ForegroundColor.Should().Be(ConsoleColor.Gray);
            consoleManager.BackgroundColor.Should().Be(ConsoleColor.Black);
        }
    }
}
