namespace LoxInterpreter

// group members: Peter Zhang, Madeline Moore, Cara Cannarozzi
// Crafting Interpreters book by Robert Nystrom used as a reference
// https://craftinginterpreters.com/contents.html

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

        /// <summary>
        /// constructs token from provided arguments
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lexeme"></param>
        /// <param name="literal"></param>
        /// <param name="line"></param>
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        // converts token to string
        public override string ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}