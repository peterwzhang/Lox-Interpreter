using System.Collections.Generic;

namespace LoxInterpreter
{
//> Functions callable
    public interface ILoxCallable
    {
//> callable-arity
        int Arity();

//< callable-arity
        object Call(Interpreter interpreter, List<object> arguments);
    }
}