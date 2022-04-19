using System.Collections.Generic;

// group members: Peter Zhang, Madeline Moore, Cara Cannarozzi
// Crafting Interpreters book by Robert Nystrom used as a reference
// https://craftinginterpreters.com/contents.html

namespace LoxInterpreter
{
    /// <summary>
    /// class for the scanner
    /// input is stored as a string, turn into list of tokens
    /// </summary>
    public class Scanner
    {
        // handles keywords of the lox language
        private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            {
                "and", TokenType.AND
            },
            {
                "class", TokenType.CLASS
            },
            {
                "else", TokenType.ELSE
            },
            {
                "false", TokenType.FALSE
            },
            {
                "for", TokenType.FOR
            },
            {
                "fun", TokenType.FUN
            },
            {
                "if", TokenType.IF
            },
            {
                "nil", TokenType.NIL
            },
            {
                "or", TokenType.OR
            },
            {
                "print", TokenType.PRINT
            },
            {
                "return", TokenType.RETURN
            },
            {
                "super", TokenType.SUPER
            },
            {
                "this", TokenType.THIS
            },
            {
                "true", TokenType.TRUE
            },
            {
                "var", TokenType.VAR
            },
            {
                "while", TokenType.WHILE
            }
        };

        // source string
        private readonly string source;

        // list of tokens (empty for now)
        private readonly List<Token> tokens = new List<Token>();
        
        // keeps track of where scanner is in source
        private int start;
        private int current;
        private int line = 1;


        // constructor for Scanner
        public Scanner(string source)
        {
            this.source = source;
        }

        // fills token list with tokens
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        // scans a single token (called in for loop in ScanTokens)
        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                // tokens with only one character
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                // tokens for operators with two characters
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                // handles the fact that a slash is used for division and comments
                case '/':
                    if (Match('/'))
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    else
                        AddToken(TokenType.SLASH);

                    break;

                // ignores whitespace
                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    line++;
                    break;

                case '"':
                    String();
                    break;

                // default case if token is a number or character
                default:
                    if (IsDigit(c))
                        Number();
                    else if (IsAlpha(c))
                        Identifier();

                    break;
            }
        }

        // checks if an identifier matches anything in keywords dictionary
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            var text = source.Substring(start, current - start);
            var keyExists = keywords.ContainsKey(text);
            TokenType type;
            if (keyExists)
                type = keywords[text];
            else
                type = TokenType.IDENTIFIER;
            AddToken(type);
        }

        // consumes as many digits of an integer as possible
        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // looks (and handles) if there's a decimal in the number
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER,
                double.Parse(source.Substring(start, current - start)));
        }

        // consumes string until reaching the closing quote
        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
                return;

            Advance();

            var value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        // similar to Advance(), but employs maximal munch
        // checks if current matches expected
        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        // similar to Advance() but doesn't consume the character (just looks ahead)
        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        // used for checking after the decimal in a number (you need to peek twice)
        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        // checks if input is a character
        private bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        // checks if input is alphanumeric
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        // checks if input is digit
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        // checks if is at end of source
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        // advances through source (consumes next character and returns it)
        private char Advance()
        {
            return source[current++];
        }

        // creates a token for current text (token only)
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        // creates a token for current text (handles tokens with literal values)
        private void AddToken(TokenType type, object literal)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}