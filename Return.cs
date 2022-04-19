using System;

namespace LoxInterpreter
{
    public class Return : Exception
    {
        public readonly object value;

        public Return(object val)
        {
            value = val;
        }
    }
}