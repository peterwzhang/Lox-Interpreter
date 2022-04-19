using System.Collections.Generic;

// group members: Peter Zhang, Madeline Moore, Cara Cannarozzi
// Crafting Interpreters book by Robert Nystrom used as a reference
// https://craftinginterpreters.com/contents.html

namespace LoxInterpreter
{
    /// <summary>
    /// interface of callable functions
    /// </summary>
    public interface ILoxCallable
    {
        int Arity();

        object Call(Interpreter interpreter, List<object> arguments);
    }
}