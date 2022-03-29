using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    public class LoxInstance<T>
    {
        private LoxClass<T> klass;

//> lox-instance-fields
        private Dictionary<string, object> fields = new Dictionary<string, object>();
//< lox-instance-fields

        LoxInstance<T>(LoxClass<T> klass)
        {
            this.klass = klass;
        }

//> lox-instance-get-property
        object get(Token name)
        {
            if (fields.ContainsKey(name.lexeme))
            {
                return fields[name.lexeme];
            }

//> lox-instance-get-method
            LoxFunction method = klass.FindMethod(name.lexeme);
/* Classes lox-instance-get-method < Classes lox-instance-bind-method
if (method != null) return method;
*/
//> lox-instance-bind-method
            if (method != null) return method.bind(this);
//< lox-instance-bind-method

//< lox-instance-get-method
            throw new RuntimeError(name, // [hidden]
                "Undefined property '" + name.lexeme + "'.");
        }

//< lox-instance-get-property
//> lox-instance-set-property
        void set(Token name, object value)
        {
            fields.put(name.lexeme, value);
        }

//< lox-instance-set-property
        @Override

        public string toString()
        {
            return klass.name + " instance";
        }
    }
}