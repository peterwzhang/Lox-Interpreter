using System.Collections.Generic;

namespace LoxInterpreter
{
    public class LoxFunction : ILoxCallable
    {
        //> closure-field
        private readonly Environment closure;
        private readonly Stmt.Function declaration;

        //< closure-field
        /* Functions lox-function < Functions closure-constructor
          LoxFunction(Stmt.Function declaration) {
        */
        /* Functions closure-constructor < Classes is-initializer-field
          LoxFunction(Stmt.Function declaration, Environment closure) {
        */
        //> Classes is-initializer-field
        private bool isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure,
            bool isInitializer)
        {
            this.isInitializer = isInitializer;
            //< Classes is-initializer-field
            //> closure-constructor
            this.closure = closure;
            //< closure-constructor
            this.declaration = declaration;
        }

        //< function-to-string
        //> function-arity

        //deleted override below
        public int Arity()
        {
            return declaration.parms.Count;
        }

        //< function-arity
        //> function-call

        //deleted override below
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            /* Functions function-call < Functions call-closure
                Environment environment = new Environment(interpreter.globals);
            */
            //> call-closure
            var environment = new Environment(closure);
            //< call-closure
            for (var i = 0; i < declaration.parms.Count; i++)
                environment.Define(declaration.parms[i].lexeme, arguments[i]);

            /* Functions function-call < Functions catch-return
                interpreter.executeBlock(declaration.body, environment);
            */
            //> catch-return
            //interpreter.ExecuteBlock(declaration.body, environment);
            try
            {
                interpreter.ExecuteBlock(declaration.body, environment);
            }
            catch (Return returnValue)
            {
                //> Classes early-return-this
                //if (isInitializer) return closure.GetAt(0, "this");

                //< Classes early-return-this
                return returnValue.value;
            }

            // < catch-return
            // > Classes return-this

            //if (isInitializer) return closure.GetAt(0, "this");
            //< Classes return-this
            return null;
        }
        //
        // //> Classes bind-instance
        // public LoxFunction<T> Bind(LoxInstance<T> instance)
        // {
        //     Environment environment = new Environment(closure);
        //     environment.Define("this", instance);
        //     /* Classes bind-instance < Classes lox-function-bind-with-initializer
        //         return new LoxFunction(declaration, environment);
        //     */
        //     //> lox-function-bind-with-initializer
        //     return new LoxFunction<T>(declaration, environment,
        //         isInitializer);
        //     //< lox-function-bind-with-initializer
        // }

        //< Classes bind-instance
        //> function-to-string

        public override string ToString()
        {
            return "<fn " + declaration.name.lexeme + ">";
        }
        //< function-call
    }
}