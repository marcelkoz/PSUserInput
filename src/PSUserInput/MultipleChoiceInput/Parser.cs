using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSUserInput.Parsers.MultipleChoice;

using Choices = List<int>;

class ForwardStream<T>
{
    private IList<T> m_elements { get; init; }
    private T m_currentElement { get; set; }
    private int m_position { get; set; }

    public T Default { get; init; }
    public T CurrentElement { get { return m_currentElement; } }
    public bool Continue
    {
        get { return m_position + 1 < m_elements.Count; }
    }

    public ForwardStream(IList<T> elements, T defaultValue)
    {
        m_elements = elements;
        m_position = -1;
        Default = defaultValue;
        m_currentElement = Default;
    }

    public T Next()
    {
#if DEBUG
        Console.WriteLine("Next Element: (Pos:{0} Next:{1} Len:{2})", m_position, m_position + 1, m_elements.Count);
#endif

        if (Continue)
        {
            m_position++;
            m_currentElement = m_elements[m_position];
        }
        else
        {
            m_currentElement = Default;
        }

        return CurrentElement;
    }

    public T Peek()
    {
        return Continue
            ? m_elements[m_position + 1]
            : Default;
    }
}

public enum TokenType
{
    EOF = 0,
    // unsigned integer
    Number,
    // - :
    RangeSeparator
}

public record Token(TokenType Type, string Value);

public class Scanner
{
    private List<Token> m_tokens { get; set; }
    private ForwardStream<char> m_input { get; set; }

    public (bool, List<Token>) Tokenise(string input)
    {
        _resetValues(input);
        var result = _tokenise();
        return (result, result
            ? m_tokens
            : new List<Token>());
    }

    private void _resetValues(string input)
    {
        m_input = new ForwardStream<char>(input.ToCharArray(), 'E');
        m_tokens = new List<Token>();
    }

    private bool _tokenise()
    {
        while (m_input.Continue)
        {
            m_input.Next();

            if (Char.IsWhiteSpace(m_input.CurrentElement)) { }
            else if (_isNumeric(m_input.CurrentElement)) _tokeniseNumber();
            else if ("-:".Contains(m_input.CurrentElement))
                m_tokens.Add(new Token(TokenType.RangeSeparator, m_input.CurrentElement.ToString()));
            else return false;
        }

        m_tokens.Add(new Token(TokenType.EOF, ""));
        return true;
    }

    private bool _isNumeric(char ch)
    {
        return "0123456789".Contains(ch);
    }

    private void _tokeniseNumber()
    {
        var number = new StringBuilder();

        number.Append(m_input.CurrentElement);
        // construct number
        while (_isNumeric(m_input.Peek()))
        {
            m_input.Next();
            number.Append(m_input.CurrentElement);
        }

        m_tokens.Add(
            new Token(TokenType.Number, number.ToString())
        );
    }
}

class ParserStop : Exception { }

public class Parser
{
    private Scanner m_scanner { get; init; }
    private DecisionEngine m_engine { get; init; }
    private Choices m_numbers { get; set; }
    private ForwardStream<Token> m_tokens { get; set; }

    public Parser(string[] answers, string list, string duplicates)
    {
        m_scanner = new Scanner();
        m_engine = new DecisionEngine(answers, list, duplicates);
    }

    public (bool, Choices) Parse(string input)
    {
        var invalidChoices = (false, new Choices());

        // tokenising
#if DEBUG
        Console.WriteLine("===== Tokenising =====");
#endif
        var (scanSuccess, tokens) = m_scanner.Tokenise(input);
        if (!scanSuccess) return invalidChoices;

        // parsing
#if DEBUG
        Console.WriteLine("===== Parsing =====");
#endif
        _resetValues(tokens);
        var parseSuccess = _parse();
        if (!parseSuccess) return invalidChoices;

        // validation
#if DEBUG
        Console.WriteLine("===== Validation =====");
#endif
        return m_engine.ValidateChoices(m_numbers);
    }

    private void _resetValues(List<Token> tokens)
    {
        m_tokens = new ForwardStream<Token>(
            tokens,
            new Token(TokenType.EOF, "")
        );
        m_numbers = new List<int>();
    }

    private bool _parse()
    {
        while (m_tokens.Continue)
        {
            m_tokens.Next();

            try
            {
                _parseToken();
            }
            catch (ParserStop)
            {
#if DEBUG
                Console.WriteLine("Parser stop exception.");
#endif
                return false;
            }
        }

        return true;
    }

    private void _parseToken()
    {
        switch (m_tokens.CurrentElement.Type)
        {
            case TokenType.Number:
                _parseNumber();
                break;

            case TokenType.EOF:
                break;

            // separator should not be encountered here
            default:
                throw new ParserStop();
        }
    }

    private void _parseNumber()
    {
        if (m_tokens.Peek().Type == TokenType.RangeSeparator)
        {
            _parseRange();
            return;
        }

        m_numbers.Add(_takeNumber(m_tokens.CurrentElement));
    }

    private void _parseRange()
    {
        var start = _takeNumber(m_tokens.CurrentElement);
        // separator
        m_tokens.Next();
        var end = _takeNumber(m_tokens.Next());

        if (start < end)
        {
            for (int i = start; i <= end; i++)
            {
                m_numbers.Add(i);
            }
        }
        else throw new ParserStop();
    }

    private int _takeNumber(Token token)
    {
        if (token.Type != TokenType.Number) throw new ParserStop();
        if (Int32.TryParse(token.Value, out int number)) return number;
        else throw new ParserStop();
    }
}
