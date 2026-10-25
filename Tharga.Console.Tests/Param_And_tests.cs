using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Xunit;

namespace Tharga.Console.Tests;

public class Param_And_tests
{
    [Fact]
    public void Adding_a_new_value_appends_it_last()
    {
        var options = new[] { new KeyValuePair<int, string>(1, "1") };

        var result = options.And(2).ToArray();

        result.Should().Equal(new KeyValuePair<int, string>(1, "1"), new KeyValuePair<int, string>(2, "2"));
    }

    [Fact]
    public void Adding_a_value_already_offered_keeps_one()
    {
        var result = Param.Build<bool>().And(true).ToArray();

        result.Should().Equal(new KeyValuePair<bool, string>(true, "True"), new KeyValuePair<bool, string>(false, "False"));
    }

    [Fact]
    public void Duplicates_already_in_the_options_are_removed()
    {
        var options = new[] { new KeyValuePair<int, string>(1, "1"), new KeyValuePair<int, string>(1, "1") };

        var result = options.And(2).ToArray();

        result.Should().Equal(new KeyValuePair<int, string>(1, "1"), new KeyValuePair<int, string>(2, "2"));
    }

    [Fact]
    public void Same_key_with_a_different_text_is_kept()
    {
        var options = new[] { new KeyValuePair<int, string>(1, "1") };

        var result = options.And((1, "One")).ToArray();

        result.Should().Equal(new KeyValuePair<int, string>(1, "1"), new KeyValuePair<int, string>(1, "One"));
    }

    [Fact]
    public void Text_is_compared_case_sensitively()
    {
        var options = new[] { new KeyValuePair<int, string>(1, "one") };

        var result = options.And((1, "One")).ToArray();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void Null_keys_with_the_same_text_are_equal()
    {
        var options = new[] { new KeyValuePair<string, string>(null, "None") };

        var result = options.And((null, "None")).ToArray();

        result.Should().ContainSingle().Which.Should().Be(new KeyValuePair<string, string>(null, "None"));
    }

    [Fact]
    public void Adding_a_dictionary_entry_already_present_keeps_one()
    {
        IDictionary<int, string> options = new Dictionary<int, string> { { 1, "1" }, { 2, "2" } };

        var result = options.And(2).ToArray();

        result.Should().Equal(new KeyValuePair<int, string>(1, "1"), new KeyValuePair<int, string>(2, "2"));
    }
}
