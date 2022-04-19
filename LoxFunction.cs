using System.Collections.Generic;

// group members: Peter Zhang, Madeline Moore, Cara Cannarozzi
// Crafting Interpreters book by Robert Nystrom used as a reference
// https://craftinginterpreters.com/contents.html

namespace LoxInterpreter
{
    /// <summary>
    /// implements ILoxCallable so it can be implemented
    /// </summary>
    public class LoxFunction : ILoxCallable
    {
        private readonly Environment closure;
        private readonly Stmt.Function declaration;

        private bool isInitializer;

        /// <summary>
        /// initializes environment from closure (constructor)
        /// </summary>
        /// <param name="declaration"></param>
        /// <param name="closure"></param>
        /// <param name="isInitializer"></param>
        public LoxFunction(Stmt.Function declaration, Environment closure,
            bool isInitializer)
        {
            this.isInitializer = isInitializer;
            this.closure = closure;
            this.declaration = declaration;
        }

        // returns arity of function
        public int Arity()
        {
            return declaration.parms.Count;
        }

        // implements call() of ILoxCallable
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            // creates environment from args
            var environment = new Environment(closure);
            for (var i = 0; i < declaration.parms.Count; i++)
                environment.Define(declaration.parms[i].lexeme, arguments[i]);

            // executes function call
            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.value;
            }

            return null;
        }

        // turns function into a string
        public override string ToString()
        {
            return "<fn " + declaration.name.lexeme + ">";
        }
    }
}