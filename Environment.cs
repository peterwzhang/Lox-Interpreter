using System;
using System.Text;
using System.Collections.Generic;
using LoxInterpreter;

namespace LoxInterpreter
{
  public class Environment
  {
    //> enclosing-field
    private Environment enclosing;

    //< enclosing-field
    private Dictionary<string, object> values = new Dictionary<string, object>();

    //> environment-constructors
    public Environment()
    {
      enclosing = null;
    }

    public Environment(Environment enclosing)
    {
      this.enclosing = enclosing;
    }
    //< environment-constructors
    //> environment-get

    public object Get(Token name)
    {
      if (values.ContainsKey(name.lexeme))
      {
        return values[name.lexeme];
      }
      //> environment-get-enclosing

      if (enclosing != null) return enclosing.Get(name);
      //< environment-get-enclosing

      // throw new RuntimeError(name,
      //   "Undefined variable '" + name.lexeme + "'.");
      return enclosing.Get(name);
      //return null;
    }

    //< environment-get
    //> environment-assign
    public void Assign(Token name, object value)
    {
      if (values.ContainsKey(name.lexeme))
      {
        values.Add(name.lexeme, value);
        return;
      }

      //> environment-assign-enclosing
      if (enclosing != null)
      {
        enclosing.Assign(name, value);
        return;
      }

      //< environment-assign-enclosing
      // throw new RuntimeError(name,
      //   "Undefined variable '" + name.lexeme + "'.");
    }

    //< environment-assign
    //> environment-define
    public void Define(string name, object value)
    {
      values.Add(name, value);
    }

    //< environment-define
    //> Resolving and Binding ancestor
    public Environment Ancestor(int distance)
    {
      Environment environment = this;
        for (int i = 0; i < distance; i++)
        {
          environment = environment.enclosing; // [coupled]
        }

        return environment;
    }

    //< Resolving and Binding ancestor
    //> Resolving and Binding get-at
    public object GetAt(int distance, string name)
    {
      return Ancestor(distance).values[name];
    }

    //< Resolving and Binding get-at
    //> Resolving and Binding assign-at
    public void AssignAt(int distance, Token name, object value)
    {
      Ancestor(distance).values.Add(name.lexeme, value);
    }

    //< Resolving and Binding assign-at
    //> omit
    public override string ToString()
    {
      string result = values.ToString();
      if (enclosing != null)
      {
        result += " -> " + enclosing.ToString();
      }

      return result;
    }
    //< omit
  }
}