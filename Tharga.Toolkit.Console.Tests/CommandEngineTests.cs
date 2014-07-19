using NUnit.Framework;
using Tharga.Toolkit.Console.Command;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class CommandEngineTests
    {
        [Test]
        public void When_providing_the_exit_command_the_command_engine_should_exit()
        {
            //Arrange
            var command = new RootCommand();
            
            //Act
            new CommandEngine(command).Run(new[] { "exit" });

            //Assert
            Assert.True(true);
        }
    }
}