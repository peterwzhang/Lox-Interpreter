using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    public class Clock : ILoxCallable
    {
        //deleted override below
        public int Arity()
        {
            return 0;
        }

        //deleted override below
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var millisec = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return millisec / 1000.0;
        }

        //deleted override below
        public string ToString()
        {
            return "<native fn>";
        }
    }
}