using System.Collections.Generic;

namespace LoxInterpreter
{
    /// <summary>
    /// class containing dictionary of variables
    /// </summary>
    public class Environment
    {
        public readonly Environment enclosing;

        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        // sets outer environment to null (constructor)
        public Environment()
        {
            enclosing = null;
        }

        // sets outer environment to "enclosing" (constructor with arg)
        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        /// <summary>
        /// looks up variables once they've been put in the dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <returns>value associated with name</returns>
        public object Get(Token name)
        {
            if (values.ContainsKey(name.lexeme)) return values[name.lexeme];

            if (enclosing != null)
                return enclosing.Get(name);

            return enclosing.Get(name);
        }

        /// <summary>
        /// assigns a value to a pre-existing variable
        /// CANNOT create a new variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            if (enclosing != null) enclosing.Assign(name, value);
        }

        /// <summary>
        /// binds a name to a value
        /// adds name and value to dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Define(string name, object value)
        {
            if (values.ContainsKey(name))
                values[name] = value;
            else
                values.Add(name, value);
        }

        // returns parent environment from "distance" in environment
        public Environment Ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i++) environment = environment.enclosing; // [coupled]

            return environment;
        }

        // gets a variable at a certain distance in the environment
        public object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        // assigns a variable at a specific depth of the environment
        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.lexeme] = value;
        }

        public override string ToString()
        {
            var result = values.ToString();
            if (enclosing != null) result += " -> " + enclosing;

            return result;
        }
    }
}