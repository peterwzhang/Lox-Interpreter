using System;
using System.Collections.Generic;

//using LoxInterpreter.Properties;

namespace LoxInterpreter
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
//< Statements and State interpreter
/* Statements and State environment-field < Functions global-environment
  Environment environment = new Environment();
*/
//> Functions global-environment
        private static readonly Environment Globals = new Environment();

//< Functions global-environment
//> Resolving and Binding locals-field
        private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        private Environment environment = Globals;
//< Resolving and Binding locals-field
//> Statements and State environment-field

//< Statements and State environment-field
//> Functions interpreter-constructor
        public Interpreter()
        {
            Globals.Define("clock", new Clock());
        }

//< Statements and State Execute-block
//> Statements and State Visit-block

        public object VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

//< Statements and State Visit-block
//> Classes interpreter-Visit-class


//     public void VisitClassStmt(Stmt.Class stmt)
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
//       Dictionary<String, LoxFunction> methods = new Dictionary<String, LoxFunction>();
//       foreach (Stmt.Function method in stmt.methods) {
// /* Classes interpret-methods < Classes interpreter-method-initializer
//       LoxFunction function = new LoxFunction(method, environment);
// */
// //> interpreter-method-initializer
//         LoxFunction function = new LoxFunction(method, environment,
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


        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);

            return null;
        }

//< Statements and State Visit-expression-stmt
//> Functions Visit-function


        public object VisitFunctionStmt(Stmt.Function stmt)
        {
/* Functions Visit-function < Functions Visit-closure
    LoxFunction function = new LoxFunction(stmt);
*/
/* Functions Visit-closure < Classes construct-function
    LoxFunction function = new LoxFunction(stmt, environment);
*/
//> Classes construct-function
            var function = new LoxFunction(stmt, environment, false);
//< Classes construct-function
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

//< Functions Visit-function
//> Control Flow Visit-if


        public object VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
                Execute(stmt.thenBranch);
            else if (stmt.elseBranch != null) Execute(stmt.elseBranch);

            return null;
        }

//< Control Flow Visit-if
//> Statements and State Visit-print


        public object VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));

            return null;
        }

//< Statements and State Visit-print
//> Functions Visit-return


        public object VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.value != null)
                value = Evaluate(stmt.value);
            //Console.WriteLine(value);

            throw new Return(value); //! maybe important
        }

//< Functions Visit-return
//> Statements and State Visit-var


        public object VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.initializer != null) value = Evaluate(stmt.initializer);

            environment.Define(stmt.name.lexeme, value);
            return null;
        }

//< Statements and State Visit-var
//> Control Flow Visit-while


        public object VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition))) Execute(stmt.body);

            return null;
        }

//< Control Flow Visit-while
//> Statements and State Visit-assign


        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.value);
/* Statements and State Visit-assign < Resolving and Binding resolved-assign
    environment.assign(expr.name, value);
*/
//> Resolving and Binding resolved-assign


            if (locals.ContainsKey(expr))
            {
                var distance = locals[expr];
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                Globals.Assign(expr.name, value);
            }

//< Resolving and Binding resolved-assign
            return value;
        }

//< Statements and State Visit-assign
//> Visit-binary


        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right); // [left]

            switch (expr.op.type)
            {
//> binary-equality
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
//< binary-equality
//> binary-comparison
                case TokenType.GREATER:
//> check-greater-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-greater-operand
                    return (double) left > (double) right;
                case TokenType.GREATER_EQUAL:
//> check-greater-equal-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-greater-equal-operand
                    return (double) left >= (double) right;
                case TokenType.LESS:
//> check-less-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-less-operand
                    return (double) left < (double) right;
                case TokenType.LESS_EQUAL:
//> check-less-equal-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-less-equal-operand
                    return (double) left <= (double) right;
//< binary-comparison
                case TokenType.MINUS:
//> check-minus-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-minus-operand
                    return (double) left - (double) right;
//> binary-plus
                case TokenType.PLUS:
                    if (left is double && right is double) return (double) left + (double) right;

                    if (left is string && right is string) return (string) left + (string) right;

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
                    return (double) left / (double) right;
                case TokenType.STAR:
//> check-star-operand
                    CheckNumberOperands(expr.op, left, right);
//< check-star-operand
                    return (double) left * (double) right;
            }

            return null;

            // Unreachable.
        }

//< Visit-binary
//> Functions Visit-call


        public object VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.callee);

            var arguments = new List<object>();
            foreach (var argument in expr.arguments)
                // [in-order]
                arguments.Add(Evaluate(argument));

//> check-is-callable
            // if (!(callee is ILoxCallable)) {
            //   throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            // }

//< check-is-callable
            var function = (ILoxCallable) callee;
//> check-arity
            // if (arguments.Count != function.arity())
            // {
            //   throw new RuntimeError(expr.paren, "Expected " + function.arity() + " arguments but got " + arguments.size() + ".");
            // }

//< check-arity
            return function.Call(this, arguments);
        }

//< Functions Visit-call
//> Classes interpreter-Visit-get


        public object VisitGetExpr(Expr.Get expr)
        {
            // Object obj = Evaluate(expr.obj);
            // if (Object is LoxInstance) {
            //   return ((LoxInstance) object).get(expr.name);
            // }
            //
            // throw new RuntimeError(expr.name,
            //   "Only instances have properties.");
            return null;
        }

//< Classes interpreter-Visit-get
//> Visit-grouping


        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

//< Visit-grouping
//> Visit-literal


        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

//< Visit-literal
//> Control Flow Visit-logical


        public object VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.left);

            if (expr.op.type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
        }

//< Control Flow Visit-logical
//> Classes interpreter-Visit-set


        public object VisitSetExpr(Expr.Set expr)
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
            return null;
        }

//< Classes interpreter-Visit-set
//> Inheritance interpreter-Visit-super


//     public Object VisitSuperExpr(Expr<Stmt>.Super expr)
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
//       LoxFunction method = superclass.findMethod(expr.method.lexeme);
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
        // public Object VisitThisExpr(Expr<Stmt>.This expr)
        // {
        //   return lookUpVariable(expr.keyword, expr);
        // }

//< Classes interpreter-Visit-this
//> Visit-unary
//     
//
        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.right);

            switch (expr.op.type)
            {
//> unary-bang
                case TokenType.BANG:
                    return !IsTruthy(right);
//< unary-bang
                case TokenType.MINUS:
//> check-unary-operand
                    CheckNumberOperand(expr.op, right);
//< check-unary-operand
                    return -(double) right;
            }

            // Unreachable.
            return null;
        }

//< Visit-unary
//> Statements and State Visit-variable


        public object VisitVariableExpr(Expr.Variable expr)
        {
/* Statements and State Visit-variable < Resolving and Binding call-look-up-variable
    return environment.get(expr.name);
*/
//> Resolving and Binding call-look-up-variable
            return LookUpVariable(expr.name, expr);
//< Resolving and Binding call-look-up-variable
        }

//< Functions interpreter-constructor
/* Evaluating Expressions interpret < Statements and State interpret
  void interpret(Expr<Stmt> expression) { // [void]
    try {
      Object value = Evaluate(expression);
      Console.WriteLine(stringify(value));
    } catch (RuntimeError error) {
      Lox.runtimeError(error);
    }
  }
*/
//> Statements and State interpret
        public void Interpret(List<Stmt> statements)
        {
            foreach (var statement in statements) Execute(statement);
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
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

//< Evaluate
//> Statements and State Execute
        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

//< Statements and State Execute
//> Resolving and Binding resolve
        public void Resolve(Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }

//< Resolving and Binding resolve
//> Statements and State Execute-block
        public void ExecuteBlock(List<Stmt> statements, Environment env)
        {
            var previous = environment;
            try
            {
                environment = env;

                foreach (var statement in statements) Execute(statement);
            }
            //TODO: finally?
            finally
            {
                environment = previous;
            }
        }

//> Resolving and Binding look-up-variable
        private object LookUpVariable(Token name, Expr expr)
        {
            int distance;
            if (locals.ContainsKey(expr))
            {
                distance = locals[expr];
                return environment.GetAt(distance, name.lexeme);
            }

            return Globals.Get(name);
        }

//< Resolving and Binding look-up-variable
//< Statements and State Visit-variable
//> check-operand
        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double)
                return;
            //throw new RuntimeError(op, "Operand must be a number.");
        }

//< check-operand
//> check-operands
        private void CheckNumberOperands(Token op,
            object left, object right)
        {
            if (left is double && right is double) return;
            // [operand]
            //throw new RuntimeError(op, "Operands must be numbers.");
        }

//< check-operands
//> is-truthy
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool) obj;
            return true;
        }

//< is-truthy
//> is-equal
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

//< is-equal
//> stringify
        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                var text = obj.ToString();
                if (text.EndsWith(".0")) text = text.Substring(0, text.Length - 2);

                return text;
            }

            if (obj is bool)
            {
                if ((bool) obj)
                    return "true";
                return "false";
            }

            return obj.ToString();
        }
//< stringify
    }
}