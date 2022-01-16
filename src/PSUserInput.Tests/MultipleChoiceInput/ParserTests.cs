using PSUserInput.Parsers.MultipleChoice;
using Xunit;

namespace PSUserInput.Tests;

public class ParserTests
{
    public static string[][] ImproperRangeInputs = {
        new string [] { "1 2 3-1" },
        new string [] { "10-" },
        new string [] { "-700" },
    };

    [Fact]
    public void Parse_EmptyString_ReturnsInvalidPair()
    {
        var parser = new Parser(new string[] { "test", "test" }, "accept", "accept");
        var result = parser.Parse("");
        Assert.False(result.Item1);
        Assert.True(result.Item2.Count == 0);
    }

    [Theory]
    [MemberData(nameof(ImproperRangeInputs))]
    public void Parse_ImproperRanges_ReturnsInvalidPair(string input)
    {
        var parser = new Parser(new string[] { "test", "test" }, "accept", "accept");
        var result = parser.Parse(input);
        Assert.False(result.Item1);
        Assert.True(result.Item2.Count == 0);
    }
}
