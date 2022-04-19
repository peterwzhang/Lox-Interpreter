using System;
using System.Collections.Generic;

// group members: Peter Zhang, Madeline Moore, Cara Cannarozzi
// Crafting Interpreters book by Robert Nystrom used as a reference
// https://craftinginterpreters.com/contents.html

namespace LoxInterpreter
{
    /// <summary>
    /// implements Clock, a native function of lox
    /// </summary>
    public class Clock : ILoxCallable
    {
        public int Arity()
        {
            return 0;
        }

        /// <summary>
        /// gets current time
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="arguments"></param>
        /// <returns>time in milliseconds</returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var millisec = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return millisec / 1000.0;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}