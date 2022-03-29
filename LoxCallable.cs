using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
//> Functions callable
    public interface ILoxCallable<T>
    {
//> callable-arity
        int Arity();

//< callable-arity
        object Call(Interpreter<T> interpreter, List<object> arguments);
    }
}