using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace PSUserInput.Parsers.MultipleChoice
{
    using Choices = List<int>;
    
    enum TokenType
    {
        EOF = 0,
        // unsigned integer
        Number,
        // - :
        RangeSeparator
    }

    class Token
    {
        public TokenType Type;
        public string    Value;
    }

    class Scanner
    {
        private List<Token> m_tokens      { get; set; }
        private string      m_input       { get; set; }
        private char        m_currentChar { get; set; }
        private int         m_position    { get; set; }
        private bool        m_continue
        {
            get { return m_position + 1 < m_input.Length; }
        }

        public (bool, List<Token>) Tokenise(string input)
        {
            _resetValues(input);
            return (_tokenise(), m_tokens);
        }

        private void _resetValues(string input)
        {
            m_input       = input;
            m_currentChar = 'E';
            m_position    = -1;
            m_tokens      = new List<Token>();
        }

        private void _nextChar()
        {
            #if DEBUG
                Console.WriteLine("NextChar: (Pos:{0} Next:{1} Len:{2})", m_position, m_position + 1, m_input.Length);
            #endif
            if (m_continue)
            {
                m_position++;
                m_currentChar = m_input[m_position];
            }
            else
            {
                m_currentChar = 'E';
            }
        }

        private char _peekChar()
        {
            return m_continue
                ? m_input[m_position + 1]
                : 'E';
        }

        private bool _tokenise()
        {
            while (m_continue)
            {
                _nextChar();

                // ignore whitespace                
                if (Char.IsWhiteSpace(m_currentChar)) {}
                else if (_isNumeric(m_currentChar)) _tokeniseNumber();
                else if ("-:".Contains(m_currentChar))
                    m_tokens.Add(new Token { Type=TokenType.RangeSeparator, Value=m_currentChar.ToString() });
                else return false;
            }

            m_tokens.Add(new Token { Type=TokenType.EOF, Value="" });
            return true;
        }

        private bool _isNumeric(char ch)
        {
            return "0123456789".Contains(ch);
        }

        private void _tokeniseNumber()
        {
            var number = new StringBuilder();

            number.Append(m_currentChar);
            // construct number
            while (_isNumeric(_peekChar()))
            {
                _nextChar();
                number.Append(m_currentChar);
            }

            var str = number.ToString();
            m_tokens.Add(
                new Token
                {
                    Type  = TokenType.Number,
                    Value = str
                }
            );
        }
    }

    class ParserStop : Exception {}

    public class Parser
    {
        private Scanner        m_scanner      { get; } = new Scanner();
        private DecisionEngine m_engine       { get; set; }
        private Choices        m_numbers      { get; set; }
        private List<Token>    m_tokens       { get; set; }
        private Token          m_currentToken { get; set; }
        private int            m_position     { get; set; }
        private bool           m_continue
        {
            get { return m_position + 1 < m_tokens.Count; }
        }

        public Parser(string[] answers, string list, string duplicates)
        {
            m_engine = new DecisionEngine(answers, list, duplicates);
        }

        public (bool, Choices) Parse(string input)
        {
            var invalidChoices = (false, new Choices());

            // tokenising
            var (scanSuccess, tokens) = m_scanner.Tokenise(input);
            if (!scanSuccess) return invalidChoices;

            // parsing
            _resetValues(tokens);
            var parseSuccess = _parse();
            if (!parseSuccess) return invalidChoices;

            // validation
            return m_engine.ValidateChoices(m_numbers);
        }

        private void _resetValues(List<Token> tokens)
        {
            m_tokens       = tokens;
            m_position     = -1;
            m_currentToken = new Token { Type=TokenType.EOF, Value="" }; 
            m_numbers      = new List<int>();
        }

        private Token _nextToken()
        {
            #if DEBUG
                Console.WriteLine("NextToken: (Pos:{0} Next:{1} Len:{2})", m_position, m_position + 1, m_tokens.Count);
            #endif
            if (m_continue)
            {
                m_position++;
                m_currentToken = m_tokens[m_position];
            }
            else
            {
                m_currentToken = new Token { Type=TokenType.EOF, Value="" };
            }

            return m_currentToken;
        }

        private Token _peekToken()
        {
            return m_continue
                ? m_tokens[m_position + 1]
                : new Token { Type=TokenType.EOF, Value="" };
        }

        private bool _parse()
        {
            while (m_continue)
            {
                _nextToken();

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
            switch (m_currentToken.Type)
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
            if (_peekToken().Type == TokenType.RangeSeparator)
            {
                _parseRange();
                return;
            }

            m_numbers.Add(_takeNumber(m_currentToken));
        }

        private void _parseRange()
        {
            var start = _takeNumber(m_currentToken);
            // separator
            _nextToken();
            var end = _takeNumber(_nextToken());
            
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

            int number;
            if (Int32.TryParse(token.Value, out number)) return number;
            else throw new ParserStop();
        }
    }
}