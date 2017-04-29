using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class TextInputTests
    {
        [Test]
        public void When_()
        {
            //Arrange
            var consoleMock = new Mock<IConsole>(MockBehavior.Strict);
            consoleMock.Setup(x => x.Write(It.IsAny<string>()));
            consoleMock.SetupGet(x => ((SystemConsoleBase)x).CursorLeft).Returns(0);
            consoleMock.SetupGet(x => ((SystemConsoleBase)x).CursorTop).Returns(0);
            consoleMock.SetupGet(x => ((SystemConsoleBase)x).BufferWidth).Returns(80);
            consoleMock.Setup(x => x.NewLine());
            consoleMock.Setup(x => x.ReadKey(true)).Returns(() => new ConsoleKeyInfo((char)13, ConsoleKey.Enter, false, false, false));

            var inputManager = new InputManager(consoleMock.Object, "> ");

            //Act
            var response = inputManager.ReadLine(new KeyValuePair<string, string>[] { }, false);

            //Assert
            Assert.That(response, Is.EqualTo(string.Empty));
        }
    }
}