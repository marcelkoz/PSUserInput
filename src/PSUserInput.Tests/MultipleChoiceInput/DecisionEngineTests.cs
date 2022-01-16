using System;
using System.Collections.Generic;
using PSUserInput.Parsers.MultipleChoice;
using Xunit;

namespace PSUserInput.Tests;

public class DecisionEngineTests
{
    [Fact]
    public void ValidateChoices_AcceptSingleInput_ReturnsCorrectPair()
    {
        var engine = new DecisionEngine(new string[] { "test", "test", "test" }, "deny", "deny");

        var result = engine.ValidateChoices(new List<int> { 2 });
        Assert.True(result.Item1);
        Assert.True(result.Item2.Count == 1);
    }

    [Fact]
    public void ValidateChoices_AcceptListInput_ReturnsCorrectPair()
    {
        var engine = new DecisionEngine(new string[] { "test", "test", "test" }, "accept", "deny");

        var result = engine.ValidateChoices(new List<int> { 1, 3, 2 });
        Assert.True(result.Item1);
        Assert.True(result.Item2.Count == 3);
    }

    [Fact]
    public void ValidateChoices_AcceptDuplicateInput_ReturnsCorrectPair()
    {
        var engine = new DecisionEngine(new string[] { "test", "test", "test" }, "accept", "accept");

        var result = engine.ValidateChoices(new List<int> { 1, 3, 3, 1, 2, 1 });
        Assert.True(result.Item1);
        Assert.True(result.Item2.Count == 6);
    }

    [Fact]
    public void ValidateChoices_DenyDuplicateInput_ReturnsCorrectPair()
    {
        var engine = new DecisionEngine(new string[] { "test", "test", "test" }, "accept", "deny");

        var result = engine.ValidateChoices(new List<int> { 1, 3, 3, 1, 2, 1 });
        Assert.False(result.Item1);
        Assert.True(result.Item2.Count == 0);
    }
}
