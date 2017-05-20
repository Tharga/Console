using System.Collections.Generic;
using NUnit.Framework;
using Tharga.Toolkit.Console.Helpers;

namespace Tharga.Toolkit.Console.Tests
{
    [TestFixture]
    public class Table_Formatter_tests
    {
        [Test]
        public void Should_convert_basic_data()
        {
            //Arrange
            var data = new List<List<string>> { new List<string> { "A", "B" }, new List<string> { "C", "D" } };

            //Act
            var result = data.ToFormattedString();

            //Assert
            Assert.That(result, Is.EqualTo("A B\r\nC D\r\n1 lines."));
        }

        [Test]
        public void Should_convert_single_line()
        {
            //Arrange
            var data = new List<List<string>> { new List<string> { "A", "B" } };

            //Act
            var result = data.ToFormattedString();

            //Assert
            Assert.That(result, Is.EqualTo("A B\r\n0 lines."));
        }

        [Test]
        public void Should_convert_null_content()
        {
            //Arrange
            var data = new List<List<string>> { new List<string> { null, null }, new List<string> { null, null } };

            //Act
            var result = data.ToFormattedString();

            //Assert
            Assert.That(result, Is.EqualTo("\r\n\r\n1 lines."));
        }
    }
}
