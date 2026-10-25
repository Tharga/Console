using System.Collections.Generic;
using System.IO;
using AwesomeAssertions;
using Moq;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;
using Xunit;

namespace Tharga.Console.Tests
{
    [Collection(ProcessStateCollection.Name)]
    public class When_intercepting_the_system_console
    {
        [Fact]
        public void Should_route_standard_output_and_standard_error_to_the_console()
        {
            //Arrange
            var originalOut = System.Console.Out;
            var originalError = System.Console.Error;
            var originalIn = System.Console.In;
            var outputs = new List<IOutput>();
            var console = new Mock<IConsole>();
            console.Setup(x => x.Output(It.IsAny<IOutput>())).Callback<IOutput>(outputs.Add);

            try
            {
                new ConsoleManager(TextWriter.Null).Intercept(console.Object);

                //Act
                System.Console.Out.WriteLine("output text");
                System.Console.Error.WriteLine("error text");
            }
            finally
            {
                System.Console.SetOut(originalOut);
                System.Console.SetError(originalError);
                System.Console.SetIn(originalIn);
            }

            //Assert
            outputs.Should().ContainSingle(x => x.Message == "output text").Which.OutputLevel.Should().Be(OutputLevel.Default);
            outputs.Should().ContainSingle(x => x.Message == "error text").Which.OutputLevel.Should().Be(OutputLevel.Error);
        }
    }
}
