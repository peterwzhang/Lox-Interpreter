using System;

namespace LoxInterpreter
{
    public class Token
    {
        public TokenType type;
        public string lexeme;
        public Object literal;
        public int line; // [location]

        public Token(TokenType type, String lexeme, Object literal, int line)
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