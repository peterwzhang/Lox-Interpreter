using System.Collections.Generic;

namespace LoxInterpreter
{
    public class Scanner
    {
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

//< keyword-map
        private readonly string source;

        private readonly List<Token> tokens = new List<Token>();
        private int current;

        private int line = 1;

//> scan-state
        private int start;
//< scan-state

        public Scanner(string source)
        {
            this.source = source;
        }

//> scan-tokens
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

//< scan-tokens
//> scan-token
        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
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
                    break; // [slash]
//> two-char-tokens
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
                //< two-char-tokens
                //> slash
                case '/':
                    if (Match('/'))
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    else
                        AddToken(TokenType.SLASH);

                    break;
                //< slash
                //> whitespace

                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '\n':
                    line++;
                    break;
//< whitespace
//> string-start

                case '"':
                    String();
                    break;
//< string-start
//> char-error

                default:
/* Scanning char-error < Scanning digit-start
        Lox.error(line, "Unexpected character.");
*/
//> digit-start
                    if (IsDigit(c))
                        Number();
//> identifier-start
                    else if (IsAlpha(c))
                        Identifier();
//< identifier-start

//< digit-start
                    break;
//< char-error
            }
        }

//< scan-token
//> identifier
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

/* Scanning identifier < Scanning keyword-type
    addToken(IDENTIFIER);
*/
//> keyword-type
            var text = source.Substring(start, current - start);
            var keyExists = keywords.ContainsKey(text);
            TokenType type;
            if (keyExists)
                type = keywords[text];
            else //keywords.Add(text, TokenType.IDENTIFIER);
                type = TokenType.IDENTIFIER;
            AddToken(type);
//< keyword-type
        }

//< identifier
//> number
        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER,
                double.Parse(source.Substring(start, current - start)));
        }

//< number
//> string
        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
                //Lox.error(line, "Unterminated string.");
                return;

            // The closing ".
            Advance();

            // Trim the surrounding quotes.
            var value = source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

//< string
//> match
        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

//< match
//> peek
        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

//< peek
//> peek-next
        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        } // [peek-next]

//< peek-next
//> is-alpha
        private bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

//< is-alpha
//> is-digit
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        } // [is-digit]

//< is-digit
//> is-at-end
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

//< is-at-end
//> advance-and-add-token
        private char Advance()
        {
            return source[current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
//< advance-and-add-token
    }
}