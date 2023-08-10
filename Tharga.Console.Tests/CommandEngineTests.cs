﻿using System;
using FluentAssertions;
using NSubstitute;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;
using Xunit;

namespace Tharga.Console.Tests
{
    public class CommandEngineTests
    {
        [Fact]
        public void When_providing_the_exit_command_the_command_engine_should_exit()
        {
            //Arrange
            var command = new RootCommand(new ClientConsole(new ConsoleConfiguration { RememberStartPosition = false }));

            //Act
            new CommandEngine(command).Start(new[] { "exit" });

            //Assert
            Assert.True(true);
        }

        [Fact]
        public void When_typing_the_exit_command_the_command_engine_should_exit()
        {
            //Arrange
            var input = new[] { ConsoleKey.E, ConsoleKey.X, ConsoleKey.I, ConsoleKey.T, ConsoleKey.Enter };
            var consoleManager = new FakeConsoleManager(new FakeKeyInputEngine(input));
            var console = new TestConsole(consoleManager);
            var command = new RootCommand(console);

            //Act
            new CommandEngine(command).Start(new string[] { });

            //Assert
            Assert.True(true);
        }

        [Fact]
        public void When_registering_two_commands_with_the_same_name()
        {
            //Arrange
            var console = new ClientConsole(new ConsoleConfiguration { RememberStartPosition = false });
            var command = new RootCommand(console);
            var cmd1 = Substitute.For<ICommand>();
            cmd1.Name.Returns("A");
            cmd1.Names.Returns(new string[] { });
            var cmd2 = Substitute.For<ICommand>();
            cmd2.Name.Returns("A");
            cmd2.Names.Returns(new[] { "A" });
            command.RegisterCommand(cmd1);
            Exception exceptionThrown = null;

            //Act
            try
            {
                command.RegisterCommand(cmd2);
            }
            catch (Exception exception)
            {
                exceptionThrown = exception;
            }

            //Assert
            exceptionThrown.Should().NotBeNull();
            exceptionThrown.GetType().Should().Be(typeof(CommandAlreadyRegisteredException));
        }
    }
}