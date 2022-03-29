using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
  public class LoxClass<T> : ILoxCallable
  {
//< lox-class-callable
      string name;
//> Inheritance lox-class-superclass-field
      public LoxClass<T> superclass;
//< Inheritance lox-class-superclass-field
/* Classes lox-class < Classes lox-class-methods

  LoxClass(string name) {
    this.name = name;
  }
*/
//> lox-class-methods
      private Dictionary< string, LoxFunction<T>> methods;

/* Classes lox-class-methods < Inheritance lox-class-constructor
  LoxClass(string name, Dictionary<string, LoxFunction> methods) {
*/
//> Inheritance lox-class-constructor
      public LoxClass(string name, LoxClass<T> superclass, Dictionary<string, LoxFunction<T>> methods) 
      {
        this.superclass = superclass;
//< Inheritance lox-class-constructor
      this.name = name;
      this.methods = methods;
    }

//< lox-class-methods
//> lox-class-find-method
      public LoxFunction<T> FindMethod(string name)
    {
      if (methods.ContainsKey(name))
      {
        return methods[name];
      }

//> Inheritance find-method-recurse-superclass
      if (superclass != null)
      {
        return superclass.FindMethod(name);
      }

//< Inheritance find-method-recurse-superclass
      return null;
    }
//< lox-class-find-method
    public override string ToString()
    {
      return name;
    }

//> lox-class-call-arity
//override--deleted
    public object Call(Interpreter interpreter, List<object> arguments)
    {
        //was: LoxInstance<T> instance = new LoxInstance<T>(this);
      LoxInstance<T> instance = new LoxInstance<T>();
//> lox-class-call-initializer
      LoxFunction<T> initializer = FindMethod("init");
      if (initializer != null)
      {
        initializer.Bind(instance).Call(interpreter, arguments);
      }

//< lox-class-call-initializer
      return instance;
    }

      //deleted override below
    public int Arity()
    {
/* Classes lox-class-call-arity < Classes lox-initializer-arity
    return 0;
*/
//> lox-initializer-arity
      LoxFunction<T> initializer = FindMethod("init");
      if (initializer == null) return 0;
      return initializer.Arity();
//< lox-initializer-arity
    }
//< lox-class-call-arity
  }
}