using System;

namespace LoxInterpreter
{
    /// <summary>
    /// wraps return value with exception specific stuff
    /// since returns are similar to exceptions, this is the
    /// easiest way to handle returns
    /// </summary>
    public class Return : Exception
    {
        public readonly object value;

        public Return(object val)
        {
            value = val;
        }
    }
}