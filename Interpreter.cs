using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    // interprets expressions
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private static readonly Environment Globals = new Environment();

        private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        private Environment environment = Globals;
        
        // defines native clock function (constructor)
        public Interpreter()
        {
            Globals.Define("clock", new Clock());
        }
        
        // executes block statement
        public object VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        // evaluates inner expression
        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);

            return null;
        }

        // evaluates function statements
        public object VisitFunctionStmt(Stmt.Function stmt)
        {
            var function = new LoxFunction(stmt, environment, false);
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

        // evaluates if statements
        public object VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
                Execute(stmt.thenBranch);
            else if (stmt.elseBranch != null) Execute(stmt.elseBranch);

            return null;
        }

        // evaluates inner expression, prints to console
        public object VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));

            return null;
        }

        // evaluates return statement of function
        public object VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.value != null)
                value = Evaluate(stmt.value);

            throw new Return(value);
        }

        // evaluates variable or sets variable to null
        // (if the variable already exists or not, respectively)
        public object VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.initializer != null) value = Evaluate(stmt.initializer);

            environment.Define(stmt.name.lexeme, value);
            return null;
        }

        // evaluates while statement
        public object VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition))) Execute(stmt.body);

            return null;
        }

        // evaluates assignment expressions
        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.value);

            if (locals.ContainsKey(expr))
            {
                var distance = locals[expr];
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                Globals.Assign(expr.name, value);
            }

            return value;
        }

        // evaluates binary operators
        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.left);
            var right = Evaluate(expr.right);

            switch (expr.op.type)
            {
                // equality operators
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                // comparison operators
                case TokenType.GREATER:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left > (double) right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left >= (double) right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left < (double) right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left <= (double) right;
                // arithmetic binary operators
                case TokenType.MINUS:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left - (double) right;
                case TokenType.PLUS:
                    // handles that + can be used to concatenate strings or add numbers
                    if (left is double && right is double) return (double) left + (double) right;
                    if (left is string && right is string) return (string) left + (string) right;
                    break;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left / (double) right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.op, left, right);
                    return (double) left * (double) right;
            }

            return null;
        }
        
        // evaluates function calls
        public object VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.callee);

            var arguments = new List<object>();
            foreach (var argument in expr.arguments)
                arguments.Add(Evaluate(argument));

            var function = (ILoxCallable) callee;

            return function.Call(this, arguments);
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            return null;
        }

        // evaluates when there are explicit parentheses in an expression
        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        // convert tree nodes into runtime values
        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        // evaluates or statements from left to right
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

        public object VisitSetExpr(Expr.Set expr)
        {
            return null;
        }

        // evaluates unary expressions
        public object VisitUnaryExpr(Expr.Unary expr)
        {
            // evaluate operand expression
            var right = Evaluate(expr.right);

            // apply unary operator to result of evaluated expression
            switch (expr.op.type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.op, right);
                    return -(double) right;
            }

            return null;
        }

        // sends variable to environment with its definition
        public object VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.name, expr);
        }

        // evaluates each statement in given list of statements
        public void Interpret(List<Stmt> statements)
        {
            foreach (var statement in statements) Execute(statement);
        }

        // evaluates expression contained inside of parentheses
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        // similar to Evaluate() but for statements instead of expressions
        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        /// <summary>
        /// resolves expressions at depth within the environment
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="depth"></param>
        public void Resolve(Expr expr, int depth)
        {
            locals.Add(expr, depth);
        }

        /// <summary>
        /// executes list of statements in context of env
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="env"></param>
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

        // looks up distance in environment, gets variable (if it exists)
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

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double)
                return;
        }

        private void CheckNumberOperands(Token op,
            object left, object right)
        {
            if (left is double && right is double) return;
        }

        /// <summary>
        /// determines truthiness of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>boolean truthy value of an object</returns>
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool) obj;
            return true;
        }

        /// <summary>
        /// checks if two objects are equal
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>boolean equality value of a and b</returns>
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        // converts lox object to string
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
    }
}