using System.Collections.Generic;

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