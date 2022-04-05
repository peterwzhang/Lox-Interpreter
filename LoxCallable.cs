using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
//> Functions callable
    public interface ILoxCallable
    {
//> callable-arity
        int Arity();

//< callable-arity
        Object Call(Interpreter interpreter, List<Object> arguments);
    }
}