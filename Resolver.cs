using System.Collections.Generic;

// group members: Peter Zhang, Madeline Moore, Cara Cannarozzi
// Crafting Interpreters book by Robert Nystrom used as a reference
// https://craftinginterpreters.com/contents.html

namespace LoxInterpreter
{
    /// <summary>
    /// resolves statements
    /// </summary>
    public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private readonly Interpreter interpreter;

        private readonly List<Dictionary<string, bool>> scopes = new List<Dictionary<string, bool>>();

        private FunctionType currentFunction = FunctionType.NONE;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        // begins scope, traverses into statements, discards scope
        public object VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();
            return null;
        }

        // resolves expression statement
        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        // resolves functions
        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        // resolves if statement
        public object VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) Resolve(stmt.elseBranch);
            return null;
        }

        // resolves print statement
        public object VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.expression);
            return null;
        }

        // resolves return statement
        public object VisitReturnStmt(Stmt.Return stmt)
        {
            Resolve(stmt.value);

            return null;
        }

        // resolves variable declarations
        public object VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer != null) Resolve(stmt.initializer);

            Define(stmt.name);
            return null;
        }

        // resolves while loop statement
        public object VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return null;
        }

        // resolves variable assignment
        public object VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);
            return null;
        }

        // resolves binary expressions
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            // resolves left and right sides of expression
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        // resolves function calls
        public object VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.callee);

            foreach (var argument in expr.arguments) Resolve(argument);

            return null;
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.obj);
            return null;
        }

        // resolves parentheses
        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.expression);
            return null;
        }

        // resolves literals
        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        // resolves logical expressions
        public object VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;
        }

        // resolves set expressions (setters)
        public object VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.obj);
            return null;
        }

        // resolves unary expressions
        public object VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.right);
            return null;
        }

        // resolves variable expressions
        public object VisitVariableExpr(Expr.Variable expr)
        {
            ResolveLocal(expr, expr.name);
            return null;
        }
        
        // resolves each statement in a list of statements
        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements) Resolve(statement);
        }

        // resolves statements
        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        // resolves expressions
        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        // resolves function body (helper to VisitFunctionStmt)
        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;

            BeginScope();
            // declares and defines function parameters
            foreach (var param in function.parms)
            {
                Declare(param);
                Define(param);
            }

            // resolves function body
            Resolve(function.body);
            EndScope();
            currentFunction = enclosingFunction;
        }

        // creates new block scope
        private void BeginScope()
        {
            scopes.Add(new Dictionary<string, bool>());
        }

        // removes scope from the dictionary of scopes (popping if this were a stack)
        private void EndScope()
        {
            scopes.RemoveAt(scopes.Count - 1);
        }

        // adds variable to inner scope
        private void Declare(Token name)
        {
            if (scopes.Count < 1) return;

            var scope = scopes[scopes.Count - 1];

            scope.Add(name.lexeme, false);
        }

        // defines a variable
        private void Define(Token name)
        {
            if (scopes.Count < 1)
                return;
            scopes[scopes.Count - 1][name.lexeme] = true;
        }

        // resolves variable (helper function for VisitVariableExpr)
        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = scopes.Count - 1; i >= 0; i--)
                // Csharp doesn't access Stack<> by index (without resorting to Linq), hence used List<> instead
                if (scopes[i].ContainsKey(name .lexeme))
                {
                    interpreter.Resolve(expr, scopes.Count - 1 - i);
                    return;
                }
        }

        // function types (only none and function since not implementing classes)
        private enum FunctionType
        {
            NONE,
            FUNCTION,
        }
    }
}