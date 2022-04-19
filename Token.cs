namespace LoxInterpreter
{
    public class Token
    {
        public string lexeme;
        public int line; // [location]
        public object literal;
        public TokenType type;

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