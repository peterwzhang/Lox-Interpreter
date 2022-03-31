using System;
using System.Collections.Generic;
using LoxInterpreter;
//using LoxInterpreter.Properties;
using Environment = System.Environment;

namespace LoxInterpreter
{
    public class Interpreter<T> : Expr<T>.IVisitor, Stmt<T>.IVisitor
    {
//< Statements and State interpreter
/* Statements and State environment-field < Functions global-environment
  Environment environment = new Environment();
*/
//> Functions global-environment
        static Environment globals = new Environment();

        Environment environment = globals;

//< Functions global-environment
//> Resolving and Binding locals-field
        Dictionary<Expr<T>, int> locals = new Dictionary<Expr<T>, int>();
//< Resolving and Binding locals-field
//> Statements and State environment-field

//< Statements and State environment-field
//> Functions interpreter-constructor
        public Interpreter()
        {
            //TODO: instance of interface?
            globals.Define("clock", new Clock<T>());
        }

//< Functions interpreter-constructor
/* Evaluating Expressions interpret < Statements and State interpret
  void interpret(Expr<Stmt<T>> expression) { // [void]
    try {
      Object value = Evaluate(expression);
      Console.WriteLine(stringify(value));
    } catch (RuntimeError error) {
      Lox.runtimeError(error);
    }
  }
*/
//> Statements and State interpret
        public void Interpret(List<Stmt<T>> statements)
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
            // try
            // {
            //   for (Stmt statement :
            //   statements) {
            //     Execute(statement);
            //   }
            // }
            // catch (RuntimeError error)
            // {
            //   Lox.runtimeError(error);
            // }
        }

//< Statements and State interpret
//> Evaluate
        Object Evaluate(Expr<T> expr)
        {
            return expr.Accept(this);
        }

//< Evaluate
//> Statements and State Execute
        void Execute(Stmt<T> stmt)
        {
            stmt.Accept(this);
        }

//< Statements and State Execute
//> Resolving and Binding resolve
        public void Resolve(Expr<T> expr, int depth)
        {
            locals.Add(expr, depth);
        }

//< Resolving and Binding resolve
//> Statements and State Execute-block
        public void ExecuteBlock(List<Stmt<T>> statements,
            Environment env)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = env;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

//< Statements and State Execute-block
//> Statements and State Visit-block

        public T VisitBlockStmt(Stmt<T>.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return default;
        }

//< Statements and State Visit-block
//> Classes interpreter-Visit-class


//     public void VisitClassStmt(Stmt<T>.Class stmt)
//     {
// //> Inheritance interpret-superclass
//       Object superclass = null;
//       if (stmt.superclass != null)
//       {
//         superclass = Evaluate(stmt.superclass);
//         if (!(superclass is LoxClass)) {
//           throw new RuntimeError(stmt.superclass.name,
//             "Superclass must be a class.");
//         }
//       }
//
// //< Inheritance interpret-superclass
//       environment.Define(stmt.name.lexeme, null);
// //> Inheritance begin-superclass-environment
//
//       if (stmt.superclass != null)
//       {
//         environment = new Environment(environment);
//         environment.Define("super", superclass);
//       }
// //< Inheritance begin-superclass-environment
// //> interpret-methods
//
//       Dictionary<String, LoxFunction<T>> methods = new Dictionary<String, LoxFunction<T>>();
//       foreach (Stmt<T>.Function method in stmt.methods) {
// /* Classes interpret-methods < Classes interpreter-method-initializer
//       LoxFunction<T> function = new LoxFunction<T>(method, environment);
// */
// //> interpreter-method-initializer
//         LoxFunction<T> function = new LoxFunction<T>(method, environment,
//           method.name.lexeme.equals("init"));
// //< interpreter-method-initializer
//         methods.put(method.name.lexeme, function);
//       }
//
// /* Classes interpret-methods < Inheritance interpreter-construct-class
//     LoxClass klass = new LoxClass(stmt.name.lexeme, methods);
// */
// //> Inheritance interpreter-construct-class
//       LoxClass klass = new LoxClass(stmt.name.lexeme,
//         (LoxClass) superclass, methods);
// //> end-superclass-environment
//
//       if (superclass != null)
//       {
//         environment = environment.enclosing;
//       }
// //< end-superclass-environment
//
// //< Inheritance interpreter-construct-class
// //< interpret-methods
// /* Classes interpreter-Visit-class < Classes interpret-methods
//     LoxClass klass = new LoxClass(stmt.name.lexeme);
// */
//       environment.assign(stmt.name, klass);
//       
//     }

//< Classes interpreter-Visit-class
//> Statements and State Visit-expression-stmt


        public T VisitExpressionStmt(Stmt<T>.Expression stmt)
        {
            Evaluate(stmt.expression);

            return default;

        }

//< Statements and State Visit-expression-stmt
//> Functions Visit-function


        public T VisitFunctionStmt(Stmt<T>.Function stmt)
        {
/* Functions Visit-function < Functions Visit-closure
    LoxFunction<T> function = new LoxFunction<T>(stmt);
*/
/* Functions Visit-closure < Classes construct-function
    LoxFunction<T> function = new LoxFunction<T>(stmt, environment);
*/
//> Classes construct-function
            LoxFunction<T> function = new LoxFunction<T>(stmt, environment,
                false);
//< Classes construct-function
            environment.Define(stmt.name.lexeme, function);
            return default;

        }

//< Functions Visit-function
//> Control Flow Visit-if


        public T VisitIfStmt(Stmt<T>.If stmt)
        {
            if (isTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }
            
            return default;

        }

//< Control Flow Visit-if
//> Statements and State Visit-print


        public T VisitPrintStmt(Stmt<T>.Print stmt)
        {
            Object value = Evaluate(stmt.expression);
            Console.WriteLine(stringify(value));
            
            return default;

        }

//< Statements and State Visit-print
//> Functions Visit-return


        public T VisitReturnStmt(Stmt<T>.Return stmt)
        {
            Object value = null;
            if (stmt.value != null) value = Evaluate(stmt.value);

            //throw new Stmt<T>.Return(value); //! maybe important
            return default;
        }

//< Functions Visit-return
//> Statements and State Visit-var


        public T VisitVarStmt(Stmt<T>.Var stmt)
        {
            Object value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);
            return default;
        }

//< Statements and State Visit-var
//> Control Flow Visit-while


        public T VisitWhileStmt(Stmt<T>.While stmt)
        {
            while (isTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }

            return default;
        }

//< Control Flow Visit-while
//> Statements and State Visit-assign


        public T VisitAssignExpr(Expr<T>.Assign expr)
        {
            Object value = Evaluate(expr.value);
/* Statements and State Visit-assign < Resolving and Binding resolved-assign
    environment.assign(expr.name, value);
*/
//> Resolving and Binding resolved-assign

            int distance = locals[expr];
            if (distance != 0)
            {
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                globals.Assign(expr.name, value);
            }

//< Resolving and Binding resolved-assign
            return (T) value; // TODO: fix dis
        }

//< Statements and State Visit-assign
//> Visit-binary


        public T VisitBinaryExpr(Expr<T>.Binary expr)
        {
            Object left = Evaluate(expr.left);
            Object right = Evaluate(expr.right); // [left]

            switch (expr.op.type)
            {
//> binary-equality
                case TokenType.BANG_EQUAL:
                    return (T)(Object)(left != right); // TODO: fix dis
                case TokenType.EQUAL_EQUAL:
                    return (T)(Object)(left == right); // TODO: fix dis & belo
//< binary-equality
//> binary-comparison
                case TokenType.GREATER:
//> check-greater-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-greater-operand
                    return (T)(Object)((double) left > (double) right);
                case TokenType.GREATER_EQUAL:
//> check-greater-equal-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-greater-equal-operand
                    return (T)(Object)((double) left >= (double) right);
                case TokenType.LESS:
//> check-less-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-less-operand
                    return (T)(Object)((double) left < (double) right);
                case TokenType.LESS_EQUAL:
//> check-less-equal-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-less-equal-operand
                    return (T)(Object)((double) left <= (double) right);
//< binary-comparison
                case TokenType.MINUS:
//> check-minus-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-minus-operand
                    return (T)(Object)((double) left - (double) right);
//> binary-plus
                case TokenType.PLUS:
                    if (left is Double && right is Double)
                    {
                        return (T)(Object)((double) left + (double) right);
                    } // [plus]

                    if (left is String && right is String)
                    {
                        return (T)(Object)((String) left + (String) right);
                    }

                    break;

/* Evaluating Expressions binary-plus < Evaluating Expressions String-wrong-type
        break;
*/
//> String-wrong-type
                    //throw new RuntimeError(expr.op, "Operands must be two numbers or two strings.");
//< String-wrong-type
//< binary-plus
                case TokenType.SLASH:
//> check-slash-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-slash-operand
                    return (T)(Object)((double) left / (double) right);
                case TokenType.STAR:
//> check-star-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-star-operand
                    return (T)(Object)((double) left * (double) right);
            }

            return default;

            // Unreachable.

        }

//< Visit-binary
//> Functions Visit-call


        public T VisitCallExpr(Expr<T>.Call expr)
        {
            Object callee = Evaluate(expr.callee);

            List<object> arguments = new List<object>();
            foreach (Expr<T> argument in expr.arguments)
            {
                // [in-order]
                arguments.Add(Evaluate(argument));
            }

//> check-is-callable
            // if (!(callee is ILoxCallable)) {
            //   throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            // }

//< check-is-callable
            ILoxCallable<T> function = (ILoxCallable<T>) callee;
//> check-arity
            // if (arguments.Count != function.arity())
            // {
            //   throw new RuntimeError(expr.paren, "Expected " + function.arity() + " arguments but got " + arguments.size() + ".");
            // }

//< check-arity
            return (T)(Object) function.Call(this, arguments); //TODO: fix dis
        }

//< Functions Visit-call
//> Classes interpreter-Visit-get


        public T VisitGetExpr(Expr<T>.Get expr)
        {
          // Object obj = Evaluate(expr.obj);
          // if (Object is LoxInstance) {
          //   return ((LoxInstance) object).get(expr.name);
          // }
          //
          // throw new RuntimeError(expr.name,
          //   "Only instances have properties.");
          return default;
        }

//< Classes interpreter-Visit-get
//> Visit-grouping


        public T VisitGroupingExpr(Expr<T>.Grouping expr)
        {
            return (T)(Object)Evaluate(expr.expression); // TODO: fix dis
        }

//< Visit-grouping
//> Visit-literal


        public T VisitLiteralExpr(Expr<T>.Literal expr)
        {
            return (T) expr.value;
        }

//< Visit-literal
//> Control Flow Visit-logical


        public T VisitLogicalExpr(Expr<T>.Logical expr)
        {
            Object left = Evaluate(expr.left);

            if (expr.op.type == TokenType.OR)
            {
                if (isTruthy(left)) return (T) left;
            }
            else
            {
                if (!isTruthy(left)) return (T)  left;
            }

            return (T) Evaluate(expr.right);
        }

//< Control Flow Visit-logical
//> Classes interpreter-Visit-set

        
        public T VisitSetExpr(Expr<T>.Set expr)
        {
          // Object o = Evaluate(expr.obj);
          //
          // if (!(Object is LoxInstance)) {
          //   // [order]
          //   throw new RuntimeError(expr.name,
          //     "Only instances have fields.");
          // }
          //
           // Object value = Evaluate(expr.value);
           // ((LoxInstance) o).set(expr.name, value);
          // return value;
          return default;
        }

//< Classes interpreter-Visit-set
//> Inheritance interpreter-Visit-super


//     public Object VisitSuperExpr(Expr<Stmt<T>>.Super expr)
//     {
//       int distance = locals.get(expr);
//       LoxClass superclass = (LoxClass) environment.getAt(
//         distance, "super");
// //> super-find-this
//
//       LoxInstance Object = (LoxInstance) environment.getAt(
//         distance - 1, "this");
// //< super-find-this
// //> super-find-method
//
//       LoxFunction<T> method = superclass.findMethod(expr.method.lexeme);
// //> super-no-method
//
//       if (method == null)
//       {
//         throw new RuntimeError(expr.method,
//           "Undefined property '" + expr.method.lexeme + "'.");
//       }

//< super-no-method
//      return method.bind(object);
//< super-find-method
//    }

//< Inheritance interpreter-Visit-super
//> Classes interpreter-Visit-this

        //
        // public Object VisitThisExpr(Expr<Stmt<T>>.This expr)
        // {
        //   return lookUpVariable(expr.keyword, expr);
        // }

//< Classes interpreter-Visit-this
//> Visit-unary
//     
//
    public T VisitUnaryExpr(Expr<T>.Unary expr)
    {
      Object right = Evaluate(expr.right);

      switch (expr.op.type) {
//> unary-bang
        case TokenType.BANG:
        return (T) (Object)!isTruthy(right);
//< unary-bang
        case TokenType.MINUS:
//> check-unary-operand
        CheckNumberOperand(expr.op, right);
//< check-unary-operand
        return (T)(Object) (-(double) right);
      }

      // Unreachable.
      return default;
    }

//< Visit-unary
//> Statements and State Visit-variable


        public T VisitVariableExpr(Expr<T>.Variable expr)
        {
/* Statements and State Visit-variable < Resolving and Binding call-look-up-variable
    return environment.get(expr.name);
*/
//> Resolving and Binding call-look-up-variable
            return (T) LookUpVariable(expr.name, expr);
//< Resolving and Binding call-look-up-variable
        }

//> Resolving and Binding look-up-variable
        Object LookUpVariable(Token name, Expr<T> expr)
        {
            int distance = locals[expr];
            if (distance != 0)
            {
                return environment.GetAt(distance, name.lexeme);
            }
            else
            {
                return globals.Get(name);
            }
        }

//< Resolving and Binding look-up-variable
//< Statements and State Visit-variable
//> check-operand
        void CheckNumberOperand(Token op, Object operand)
        {
            if (operand is Double) return;
            //throw new RuntimeError(op, "Operand must be a number.");
        }

//< check-operand
//> check-operands
        void CheckNumberOperands(Token op,
            Object left, Object right)
        {
            if (left is Double && right is Double) return;
            // [operand]
            //throw new RuntimeError(op, "Operands must be numbers.");
        }

//< check-operands
//> is-truthy
        private bool isTruthy(Object obj)
        {
            if (obj == null) return false;
            if (obj is Boolean) return (bool) obj;
            return true;
        }

//< is-truthy
//> is-equal
        private bool isEqual(Object a, Object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a == b;
        }

//< is-equal
//> stringify
        private String stringify(Object obj)
        {
            if (obj == null) return "nil";

            if (obj is Double)
            {
                String text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return obj.ToString();
        }
//< stringify
    }
}
  