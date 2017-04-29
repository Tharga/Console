using System;
using Moq;
using NUnit.Framework;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;
using Tharga.Toolkit.Console.Exceptions;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class CommandEngineTests
    {
        [Test]
        public void When_providing_the_exit_command_the_command_engine_should_exit()
        {
            //Arrange
            var command = new RootCommand(new ClientConsole());
            
            //Act
            new CommandEngine(command).Run(new[] { "exit" });

            //Assert
            Assert.True(true);
        }

        [Test]
        public void When_registering_two_commands_with_the_same_name()
        {
            //Arrange
            var console = new ClientConsole();
            var command = new RootCommand(console);
            var cmd1 = new Mock<ICommand>(MockBehavior.Strict);
            cmd1.Setup(x => x.Name).Returns("A");
            cmd1.Setup(x => x.Names).Returns(new string[]{});
            cmd1.Setup(x => ((CommandBase)x).AttachConsole(console));
            var cmd2 = new Mock<ICommand>(MockBehavior.Strict);
            cmd2.Setup(x => x.Name).Returns("A");
            cmd2.Setup(x => x.Names).Returns(new[] { "A" });
            cmd2.Setup(x => ((CommandBase)x).AttachConsole(console));
            command.RegisterCommand(cmd1.Object);
            Exception exceptionThrown = null;

            //Act
            try
            {
                command.RegisterCommand(cmd2.Object);
            }
            catch (Exception exception)
            {
                exceptionThrown = exception;
            }

            //Assert
            Assert.That(exceptionThrown, Is.Not.Null);
            Assert.That(exceptionThrown.GetType(), Is.EqualTo(typeof(CommandAlreadyRegisteredException)));
        }

        //[Test]
        //public void When_()
        //{
        //    //Arrange

        //    //Act

        //    //Assert
        //}
    }
}