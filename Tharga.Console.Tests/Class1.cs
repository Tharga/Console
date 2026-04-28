using FluentAssertions;
using Xunit;

namespace Tharga.Console.Tests;

public class Class1
{
    [Fact]
    public void A()
    {
        true.Should().BeTrue();
    }
}