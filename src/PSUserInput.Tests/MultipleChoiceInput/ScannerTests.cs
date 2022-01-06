using Xunit;
using System;
using System.Collections.Generic;
using PSUserInput.Parsers.MultipleChoice;

namespace PSUserInput.Tests;

public class TestDataPair
{
    public string      Input  { get; set; } = "";
    public List<Token> Tokens { get; set; } = new List<Token>();

    public TestDataPair(string input, List<Token> tokens)
    {
        Input  = input;
        Tokens = tokens;
    }
}

public class ScannerTests
{
    public static TestDataPair[][] ValidNumbersInput = {
        new TestDataPair[] {
            new TestDataPair("1 2 3 4 5", new List<Token> {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.Number, "3"),
                new Token(TokenType.Number, "4"),
                new Token(TokenType.Number, "5"),
            })
        },

        new TestDataPair[] {
            new TestDataPair("5 10 15", new List<Token> {
                new Token(TokenType.Number, "5"),
                new Token(TokenType.Number, "10"),
                new Token(TokenType.Number, "15"),
            })
        },

        new TestDataPair[] {
            new TestDataPair("100 10006 1232323 9988 4555", new List<Token> {
                new Token(TokenType.Number, "100"),
                new Token(TokenType.Number, "10006"),
                new Token(TokenType.Number, "1232323"),
                new Token(TokenType.Number, "9988"),
                new Token(TokenType.Number, "4555"),
            })
        },

        new TestDataPair[] {
            new TestDataPair("1 2 3000 400000 500 6000", new List<Token> {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.Number, "3000"),
                new Token(TokenType.Number, "400000"),
                new Token(TokenType.Number, "500"),
                new Token(TokenType.Number, "6000"),
            })
        },
    };

    public static TestDataPair[][] ValidRangeInput = {
        new TestDataPair[] {
            new TestDataPair("1 - 10", new List<Token> {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.RangeSeparator, "-"),
                new Token(TokenType.Number, "10"),
            })
        },

        new TestDataPair[] {
            new TestDataPair("5 : 15 6", new List<Token> {
                new Token(TokenType.Number, "5"),
                new Token(TokenType.RangeSeparator, ":"),
                new Token(TokenType.Number, "15"),
                new Token(TokenType.Number, "6"),
            })
        },

        new TestDataPair[] {
            new TestDataPair("1 2 - 4 5", new List<Token> {
                new Token(TokenType.Number, "1"),
                new Token(TokenType.Number, "2"),
                new Token(TokenType.RangeSeparator, "-"),
                new Token(TokenType.Number, "4"),
                new Token(TokenType.Number, "5"),
            })
        },
    };

    public static string[][] InvalidInput = {
        new string[] { "public string[] InvalidInput = {DATA};" },
        new string[] { "Hello World!" },
        new string[] { "\t\t 4 - \r\nInvalid 8 2 4" },
        new string[] { "Invalid data" },
    };

    [Fact]
    public void Tokenise_IgnoreWhitespace_ReturnsEOFToken()
    {
        var scanner = new Scanner();
        var (success, token) = scanner.Tokenise("  \t\r\t \n\r\n   \t");

        Assert.True(success);
        Assert.True(token.Count == 1);
        Assert.True(token[0].Type == TokenType.EOF);
    }

    [Theory]
    [MemberData(nameof(InvalidInput))]
    public void Tokenise_GarbageInput_ReturnsInvalidPair(string input)
    {
        var scanner     = new Scanner();
        var invalidData = (false, new List<Token>());
        var result      = scanner.Tokenise(input);

        Assert.True(result.Item1 == invalidData.Item1);
        Assert.True(result.Item2.Count == invalidData.Item2.Count);
    }

    [Theory]
    [MemberData(nameof(ValidNumbersInput))]
    public void Tokenise_CorrectNumbersInput_ReturnsValidPair(TestDataPair pair)
    {
        var (input, expectedTokens) = (pair.Input, pair.Tokens);
        var scanner = new Scanner();
        var (success, tokens) = scanner.Tokenise(input);
        expectedTokens.Add(new Token(TokenType.EOF, ""));

        Assert.True(success);
        Assert.True(tokens.Count == expectedTokens.Count);
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            Assert.True(tokens[i].Type == expectedTokens[i].Type);
            Assert.True(tokens[i].Value == expectedTokens[i].Value);
        }
    }

    [Theory]
    [MemberData(nameof(ValidRangeInput))]
    public void Tokenise_CorrectRangeInput_ReturnsValidPair(TestDataPair pair)
    {
        var (input, expectedTokens) = (pair.Input, pair.Tokens);
        var scanner = new Scanner();
        var (success, tokens) = scanner.Tokenise(input);
        expectedTokens.Add(new Token(TokenType.EOF, ""));

        Assert.True(success);
        Assert.True(tokens.Count == expectedTokens.Count);
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            Assert.True(tokens[i].Type == expectedTokens[i].Type);
            Assert.True(tokens[i].Value == expectedTokens[i].Value);
        }
    }
}
