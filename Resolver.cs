using System;
using System.Collections.Generic;
using System.Linq;
using LoxInterpreter;

namespace LoxInterpreter
{
    public class Resolver : Expr.IVisitor<Object>, Stmt.IVisitor<Object>
    {
        private Interpreter interpreter;

//> scopes-field
        private Stack<Dictionary<String, bool>> scopes = new Stack<Dictionary<String, bool>>();

//< scopes-field
//> function-type-field
        private FunctionType currentFunction = FunctionType.NONE;
//< function-type-field

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
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
            INITIALIZER,
//< functio   n-type-initializer
            //METHOD
//< Classes function-type-method
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
        public void resolve(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                resolve(statement);
            }
        }
//< resolve-statements
//> Visit-block-stmt

        public Object VisitBlockStmt(Stmt.Block stmt)
        {
            beginScope();
            resolve(stmt.statements);
            endScope();
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

        public Object VisitExpressionStmt(Stmt.Expression stmt)
        {
            resolve(stmt.expression);
            return null;
        }
//< Visit-expression-stmt
//> Visit-function-stmt

        public Object VisitFunctionStmt(Stmt.Function stmt)
        {
            declare(stmt.name);
            define(stmt.name);

/* Resolving and Binding Visit-function-stmt < Resolving and Binding pass-function-type
    resolveFunction(stmt);
*/
//> pass-function-type
            resolveFunction(stmt, FunctionType.FUNCTION);
//< pass-function-type
            return null;
        }
//< Visit-function-stmt
//> Visit-if-stmt

        public Object VisitIfStmt(Stmt.If stmt)
        {
            resolve(stmt.condition);
            resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) resolve(stmt.elseBranch);
            return null;
        }
//< Visit-if-stmt
//> Visit-print-stmt

        public Object VisitPrintStmt(Stmt.Print stmt)
        {
            resolve(stmt.expression);
            return null;
        }
//< Visit-print-stmt
//> Visit-return-stmt

        public Object VisitReturnStmt(Stmt.Return stmt)
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
                resolve(stmt.value);
            }

            return null;
        }
//< Visit-return-stmt
//> Visit-var-stmt

        public Object VisitVarStmt(Stmt.Var stmt)
        {
            declare(stmt.name);
            if (stmt.initializer != null)
            {
                resolve(stmt.initializer);
            }

            define(stmt.name);
            return null;
        }
//< Visit-var-stmt
//> Visit-while-stmt

        public Object VisitWhileStmt(Stmt.While stmt)
        {
            resolve(stmt.condition);
            resolve(stmt.body);
            return null;
        }
//< Visit-while-stmt
//> Visit-assign-expr

        public Object VisitAssignExpr(Expr.Assign expr)
        {
            resolve(expr.value);
            resolveLocal(expr, expr.name);
            return null;
        }
//< Visit-assign-expr
//> Visit-binary-expr

        public Object VisitBinaryExpr(Expr.Binary expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }
//< Visit-binary-expr
//> Visit-call-expr

        public Object VisitCallExpr(Expr.Call expr)
        {
            resolve(expr.callee);

            foreach (Expr argument in expr.arguments)
            {
                resolve(argument);
            }

            return null;
        }
//< Visit-call-expr
//> Classes resolver-Visit-get

        public Object VisitGetExpr(Expr.Get expr)
        {
            resolve(expr.obj);
            return null;
        }
//< Classes resolver-Visit-get
//> Visit-grouping-expr

        public Object VisitGroupingExpr(Expr.Grouping expr)
        {
            resolve(expr.expression);
            return null;
        }
//< Visit-grouping-expr
//> Visit-literal-expr

        public Object VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }
//< Visit-literal-expr
//> Visit-logical-expr

        public Object VisitLogicalExpr(Expr.Logical expr)
        {
            resolve(expr.left);
            resolve(expr.right);
            return null;
        }
//< Visit-logical-expr
//> Classes resolver-Visit-set

        public Object VisitSetExpr(Expr.Set expr)
        {
            resolve(expr.value);
            resolve(expr.obj);
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

        public Object VisitUnaryExpr(Expr.Unary expr)
        {
            resolve(expr.right);
            return null;
        }
//< Visit-unary-expr
//> Visit-variable-expr

        public Object VisitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count != 0 &&
                !scopes.Peek().ContainsKey(expr.name.lexeme))
            {
                // Lox.error(expr.name,
                //     "Can't read local variable in its own initializer.");
            }

            resolveLocal(expr, expr.name);
            return null;
        }

//< Visit-variable-expr
//> resolve-stmt
        private void resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

//< resolve-stmt
//> resolve-expr
        private void resolve(Expr expr)
        {
            expr.Accept(this);
        }

//< resolve-expr
//> resolve-function
/* Resolving and Binding resolve-function < Resolving and Binding set-current-function
  private void resolveFunction(Stmt.Function function) {
*/
//> set-current-function
        private void resolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;

//< set-current-function
            beginScope();
            foreach (Token param in function.parms)
            {
                declare(param);
                define(param);
            }

            resolve(function.body);
            endScope();
//> restore-current-function
            currentFunction = enclosingFunction;
//< restore-current-function
        }

//< resolve-function
//> begin-scope
        private void beginScope()
        {
            scopes.Push(new Dictionary<String, bool>());
        }

//< begin-scope
//> end-scope
        private void endScope()
        {
            scopes.Pop();
        }

//< end-scope
//> declare
        private void declare(Token name)
        {
            if (scopes.Count == 0) return;

            Dictionary<String, bool> scope = scopes.Peek();
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
        private void define(Token name)
        {
            if (scopes.Count == 0) return;
            if (scopes.Peek().ContainsKey(name.lexeme)) scopes.Peek()[name.lexeme] = true;
            else scopes.Peek().Add(name.lexeme, true);
        }

//< define
//> resolve-local
        private void resolveLocal(Expr expr, Token name)
        {
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ToArray()[i].ContainsKey(name.lexeme))
                {
                    interpreter.Resolve(expr, scopes.Count - 1 - i);
                    return;
                }
            }
        }
    }
}
//< resolve-local