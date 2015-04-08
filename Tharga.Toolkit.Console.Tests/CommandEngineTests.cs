using NUnit.Framework;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

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
    }

    [TestFixture]
    public class TextInputTests
    {
        [Test]
        public void When_()
        {
            //Arrange
            //var consoleMock = Mock<
            //var inputManager = new InputManager(consoleMock, null, "> ");

            //Act


            //Assert
        }
    }
}