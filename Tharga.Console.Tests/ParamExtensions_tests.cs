using System.Linq;
using FluentAssertions;
using Tharga.Console.Helpers;
using Xunit;

namespace Tharga.Console.Tests;

public class ParamExtensions_tests
{
    [Fact]
    public void Quoted_argument_is_returned_as_a_single_token_with_inner_spaces()
    {
        var result = "transmit \"Hello World\"".ToInput().ToArray();

        result.Should().Equal("transmit", "Hello World");
    }

    [Fact]
    public void Mixed_quoted_and_unquoted_arguments_are_preserved_in_order()
    {
        var result = "cmd \"a b\" c \"d e\"".ToInput().ToArray();

        result.Should().Equal("cmd", "a b", "c", "d e");
    }

    [Fact]
    public void Unquoted_input_splits_on_space()
    {
        var result = "cmd a b c".ToInput().ToArray();

        result.Should().Equal("cmd", "a", "b", "c");
    }

    [Fact]
    public void Empty_input_returns_no_tokens()
    {
        var result = "".ToInput().ToArray();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Single_quoted_token_returns_its_contents()
    {
        var result = "\"hello world\"".ToInput().ToArray();

        result.Should().Equal("hello world");
    }
}
