using System.Collections.Generic;

namespace LoxInterpreter
{
    public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private readonly Interpreter interpreter;

//> scopes-field
        private readonly List<Dictionary<string, bool>> scopes = new List<Dictionary<string, bool>>();

//< scopes-field
//> function-type-field
        private FunctionType currentFunction = FunctionType.NONE;
//< function-type-field

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }
//< resolve-statements
//> Visit-block-stmt

        public object VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();
            return null;
        }
//< Visit-block-stmt
//> Classes resolver-Visit-class

//   public  Object VisitClassStmt(Stmt.Class stmt) {
// //> set-current-class
//     ClassType enclosingClass = currentClass;
//     currentClass = ClassType.CLASS;
//
// //< set-current-class
//     declare(stmt.name);
//     define(stmt.name);
// //> Inheritance resolve-superclass
//
// //> inherit-self
//     if (stmt.superclass != null &&
//         stmt.name.lexeme.equals(stmt.superclass.name.lexeme)) {
//       Lox.error(stmt.superclass.name,
//           "A class can't inherit from itself.");
//     }
//
// //< inherit-self
//     if (stmt.superclass != null) {
// //> set-current-subclass
//       currentClass = ClassType.SUBCLASS;
// //< set-current-subclass
//       resolve(stmt.superclass);
//     }
// //< Inheritance resolve-superclass
// //> Inheritance begin-super-scope
//
//     if (stmt.superclass != null) {
//       beginScope();
//       scopes.Peek().put("super", true);
//     }
// //< Inheritance begin-super-scope
// //> resolve-methods
//
// //> resolver-begin-this-scope
//     beginScope();
//     scopes.Peek().put("this", true);
//
// //< resolver-begin-this-scope
//     for (Stmt.Function method : stmt.methods) {
//       FunctionType declaration = FunctionType.METHOD;
// //> resolver-initializer-type
//       if (method.name.lexeme.equals("init")) {
//         declaration = FunctionType.INITIALIZER;
//       }
//
// //< resolver-initializer-type
//       resolveFunction(method, declaration); // [local]
//     }
//
// //> resolver-end-this-scope
//     endScope();
//
// //< resolver-end-this-scope
// //< resolve-methods
// //> Inheritance end-super-scope
//     if (stmt.superclass != null) endScope();
//
// //< Inheritance end-super-scope
// //> restore-current-class
//     currentClass = enclosingClass;
// //< restore-current-class
//     return null;
//   }
// //< Classes resolver-Visit-class
// //> Visit-expression-stmt

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return null;
        }
//< Visit-expression-stmt
//> Visit-function-stmt

        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);

/* Resolving and Binding Visit-function-stmt < Resolving and Binding pass-function-type
    resolveFunction(stmt);
*/
//> pass-function-type
            ResolveFunction(stmt, FunctionType.FUNCTION);
//< pass-function-type
            return null;
        }
//< Visit-function-stmt
//> Visit-if-stmt

        public object VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) Resolve(stmt.elseBranch);
            return null;
        }
//< Visit-if-stmt
//> Visit-print-stmt

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.expression);
            return null;
        }
//< Visit-print-stmt
//> Visit-return-stmt

        public object VisitReturnStmt(Stmt.Return stmt)
        {
//> return-from-top
            if (currentFunction == FunctionType.NONE)
            {
                //Lox.error(stmt.keyword, "Can't return from top-level code.");
            }

//< return-from-top
            if (stmt.value != null)
            {
//> Classes return-in-initializer
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    // Lox.error(stmt.keyword,
                    //     "Can't return a value from an initializer.");
                }

//< Classes return-in-initializer
                Resolve(stmt.value);
            }

            return null;
        }
//< Visit-return-stmt
//> Visit-var-stmt

        public object VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer != null) Resolve(stmt.initializer);

            Define(stmt.name);
            return null;
        }
//< Visit-var-stmt
//> Visit-while-stmt

        public object VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return null;
        }
//< Visit-while-stmt
//> Visit-assign-expr

        public object VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);
            return null;
        }
//< Visit-assign-expr
//> Visit-binary-expr

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }
//< Visit-binary-expr
//> Visit-call-expr

        public object VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.callee);

            foreach (var argument in expr.arguments) Resolve(argument);

            return null;
        }
//< Visit-call-expr
//> Classes resolver-Visit-get

        public object VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.obj);
            return null;
        }
//< Classes resolver-Visit-get
//> Visit-grouping-expr

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.expression);
            return null;
        }
//< Visit-grouping-expr
//> Visit-literal-expr

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }
//< Visit-literal-expr
//> Visit-logical-expr

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }
//< Visit-logical-expr
//> Classes resolver-Visit-set

        public object VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.obj);
            return null;
        }
//< Classes resolver-Visit-set
//> Inheritance resolve-super-expr

//   public Object VisitSuperExpr(Expr.Super expr) {
// //> invalid-super
//     if (currentClass == ClassType.NONE) {
//       Lox.error(expr.keyword,
//           "Can't use 'super' outside of a class.");
//     } else if (currentClass != ClassType.SUBCLASS) {
//       Lox.error(expr.keyword,
//           "Can't use 'super' in a class with no superclass.");
//     }
//
// //< invalid-super
//     resolveLocal(expr, expr.keyword);
//     return null;
//   }
// //< Inheritance resolve-super-expr
// //> Classes resolver-Visit-this

//   public Object VisitThisExpr(Expr.This expr) {
// //> this-outside-of-class
//     if (currentClass == ClassType.NONE) {
//       Lox.error(expr.keyword,
//           "Can't use 'this' outside of a class.");
//       return null;
//     }
//
// //< this-outside-of-class
//     resolveLocal(expr, expr.keyword);
//     return null;
//   }
//
// //< Classes resolver-Visit-this
// //> Visit-unary-expr

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.right);
            return null;
        }
//< Visit-unary-expr
//> Visit-variable-expr

        public object VisitVariableExpr(Expr.Variable expr)
        {
            // if (scopes.Count > 0 && scopes[scopes.Count - 1].ContainsKey(expr.name.lexeme))
            // {
            //     //if (scopes[scopes.Count - 1][expr.name.lexeme] == false) // declared but not yet defined
            //         //Lox.error(expr.name, "Cannot read local variable in its own initializer.");
            // }

            ResolveLocal(expr, expr.name);
            return null;
        }
//< function-type
//> Classes class-type

//   private enum ClassType {
//     NONE,
// /* Classes class-type < Inheritance class-type-subclass
//     CLASS
//  */
// //> Inheritance class-type-subclass
//     CLASS,
//     SUBCLASS
// //< Inheritance class-type-subclass
//   }
//
//   private ClassType currentClass = ClassType.NONE;

//< Classes class-type
//> resolve-statements
        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements) Resolve(statement);
        }

//< Visit-variable-expr
//> resolve-stmt
        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

//< resolve-stmt
//> resolve-expr
        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

//< resolve-expr
//> resolve-function
/* Resolving and Binding resolve-function < Resolving and Binding set-current-function
  private void resolveFunction(Stmt.Function function) {
*/
//> set-current-function
        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;

//< set-current-function
            BeginScope();
            foreach (var param in function.parms)
            {
                Declare(param);
                Define(param);
            }

            Resolve(function.body);
            EndScope();
//> restore-current-function
            currentFunction = enclosingFunction;
//< restore-current-function
        }

//< resolve-function
//> begin-scope
        private void BeginScope()
        {
            scopes.Add(new Dictionary<string, bool>());
        }

//< begin-scope
//> end-scope
        private void EndScope()
        {
            scopes.RemoveAt(scopes.Count - 1);
        }

//< end-scope
//> declare
        private void Declare(Token name)
        {
            if (scopes.Count < 1) return;

            var scope = scopes[scopes.Count - 1];
//> duplicate-variable
            if (scope.ContainsKey(name.lexeme))
            {
                // Lox.error(name,
                //     "Already a variable with this name in this scope.");
            }

//< duplicate-variable
            scope.Add(name.lexeme, false);
        }

//< declare
//> define
        private void Define(Token name)
        {
            if (scopes.Count < 1)
                return;
            scopes[scopes.Count - 1][name.lexeme] = true;
        }

//< define
//> resolve-local
        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = scopes.Count - 1; i >= 0; i--)
                if (scopes[i].ContainsKey(name
                        .lexeme)) // Csharp doesn't access Stack<> by index (without resorting to Linq), hence used List<> instead
                {
                    interpreter.Resolve(expr, scopes.Count - 1 - i);
                    return;
                }
        }

//> function-type
        private enum FunctionType
        {
            NONE,

/* Resolving and Binding function-type < Classes function-type-method
    FUNCTION
*/
//> Classes function-type-method
            FUNCTION,

//> function-type-initializer
            INITIALIZER
            //< functio   n-type-initializer
            //METHOD
//< Classes function-type-method
        }
    }
}
//< resolve-local