using System;

namespace LoxInterpreter
{
    public class Token
    {
        TokenType type;
        string lexeme;
        Object literal;
        int line; // [location]

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