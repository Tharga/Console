using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests
{
    public class Table_Formatter_tests
    {
        [Fact]
        public void Should_convert_basic_data()
        {
            //Arrange
            var data = new List<List<string>> { new List<string> { "A", "B" }, new List<string> { "C", "D" } };

            //Act
            var result = data.ToFormattedString();

            //Assert
            result.Should().Be("A B\r\nC D\r\n1 lines.");
        }

        [Fact]
        public void Should_convert_single_line()
        {
            //Arrange
            var data = new List<List<string>> { new List<string> { "A", "B" } };

            //Act
            var result = data.ToFormattedString();

            //Assert
            result.Should().Be("A B\r\n0 lines.");
        }

        [Fact]
        public void Should_convert_null_content()
        {
            //Arrange
            var data = new List<List<string>> { new List<string> { null, null }, new List<string> { null, null } };

            //Act
            var result = data.ToFormattedString();

            //Assert
            result.Should().Be("\r\n\r\n1 lines.");
        }

        [Fact]
        public void Should_not_crash_if_title_is_larger_than_data()
        {
            //Arrange
            var title = new[] { "A", "B", "C" };
            var data = new string[][] { new [] { "A1" }, new[] { "A2", "B2" }, };
            var t = new[] { title }.Union(data);

            //Act
            var result = t.ToFormattedString();

            //Assert
            result.Should().Be("A  B  C\r\nA1\r\nA2 B2\r\n2 lines.");
        }

        [Fact]
        public void Should_not_crash_if_data_is_larger_than_title()
        {
            //Arrange
            var title = new[] { "A", "B", "C" };
            var data = new string[][] { new[] { "A1" }, new[] { "A2", "B2" }, new[] { "A3", "B3", "C3" }, new[] { "A4", "B4", "C4", "D4" }, };
            var t = new[] { title }.Union(data);

            //Act
            var result = t.ToFormattedString();

            //Assert
            result.Should().Be("A  B  C\r\nA1\r\nA2 B2\r\nA3 B3 C3\r\nA4 B4 C4 D4\r\n4 lines.");
        }
    }
}
