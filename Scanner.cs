using System.Collections.Generic;

namespace LoxInterpreter
{
    public class Scanner
    {
        private int current;

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

        private int line = 1;

//< keyword-map
        private readonly string source;

//> scan-state
        private int start;

        private readonly List<Token> tokens = new List<Token>();
//< scan-state

        public Scanner(string source)
        {
            this.source = source;
        }

//> scan-tokens
        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

//< scan-tokens
//> scan-token
        private void scanToken()
        {
            var c = advance();
            switch (c)
            {
                case '(':
                    addToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    addToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    addToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    addToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    addToken(TokenType.COMMA);
                    break;
                case '.':
                    addToken(TokenType.DOT);
                    break;
                case '-':
                    addToken(TokenType.MINUS);
                    break;
                case '+':
                    addToken(TokenType.PLUS);
                    break;
                case ';':
                    addToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    addToken(TokenType.STAR);
                    break; // [slash]
//> two-char-tokens
                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                //< two-char-tokens
                //> slash
                case '/':
                    if (match('/'))
                        // A comment goes until the end of the line.
                        while (peek() != '\n' && !isAtEnd())
                            advance();
                    else
                        addToken(TokenType.SLASH);

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
                    if (isDigit(c))
                    {
                        number();
//> identifier-start
                    }
                    else if (isAlpha(c))
                    {
                        identifier();
//< identifier-start
                    }

//< digit-start
                    break;
//< char-error
            }
        }

//< scan-token
//> identifier
        private void identifier()
        {
            while (isAlphaNumeric(peek())) advance();

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
            addToken(type);
//< keyword-type
        }

//< identifier
//> number
        private void number()
        {
            while (isDigit(peek())) advance();

            // Look for a fractional part.
            if (peek() == '.' && isDigit(peekNext()))
            {
                // Consume the "."
                advance();

                while (isDigit(peek())) advance();
            }

            addToken(TokenType.NUMBER,
                double.Parse(source.Substring(start, current - start)));
        }

//< number
//> string
        private void String()
        {
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd())
                //Lox.error(line, "Unterminated string.");
                return;

            // The closing ".
            advance();

            // Trim the surrounding quotes.
            var value = source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, value);
        }

//< string
//> match
        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

//< match
//> peek
        private char peek()
        {
            if (isAtEnd()) return '\0';
            return source[current];
        }

//< peek
//> peek-next
        private char peekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        } // [peek-next]

//< peek-next
//> is-alpha
        private bool isAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

//< is-alpha
//> is-digit
        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        } // [is-digit]

//< is-digit
//> is-at-end
        private bool isAtEnd()
        {
            return current >= source.Length;
        }

//< is-at-end
//> advance-and-add-token
        private char advance()
        {
            return source[current++];
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, object literal)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }
//< advance-and-add-token
    }
}