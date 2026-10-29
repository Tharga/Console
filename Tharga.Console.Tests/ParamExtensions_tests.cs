using System.Linq;
using AwesomeAssertions;
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

    [Fact]
    public void Quoted_part_inside_an_argument_is_kept_with_its_spaces()
    {
        var result = "a=\"b c\" d".ToInput().ToArray();

        result.Should().Equal("a=b c", "d");
    }

    [Fact]
    public void Quoted_part_between_letters_is_joined_to_them()
    {
        var result = "x\"y\"z".ToInput().ToArray();

        result.Should().Equal("xyz");
    }

    [Fact]
    public void Unterminated_quote_runs_to_the_end_of_the_input()
    {
        var result = "echo \"abc def".ToInput().ToArray();

        result.Should().Equal("echo", "abc def");
    }

    [Fact]
    public void Every_space_outside_quotes_separates_arguments()
    {
        var result = "a  b".ToInput().ToArray();

        result.Should().Equal("a", "", "b");
    }
}
