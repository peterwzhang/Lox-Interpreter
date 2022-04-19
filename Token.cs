namespace LoxInterpreter
{
    /// <summary>
    /// class that tracks location of tokens
    /// </summary>
    public class Token
    {
        public TokenType type;
        public string lexeme;
        public object literal;
        public int line; // [location]

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}