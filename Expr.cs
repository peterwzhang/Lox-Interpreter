using System.Collections.Generic;

namespace LoxInterpreter
{
    public abstract class Expr
    {
        //< expr-variable

        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitAssignExpr(Assign expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGetExpr(Get expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitLogicalExpr(Logical expr);

            T VisitSetExpr(Set expr);

            T VisitUnaryExpr(Unary expr);
            T VisitVariableExpr(Variable expr);
        }

        // Nested Expr classes here...
        //> expr-assign
        public class Assign : Expr
        {
            public Token name;
            public Expr value;

            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }

        //< expr-assign
        //> expr-binary
        public class Binary : Expr
        {
            public Expr left;
            public Token op;
            public Expr right;

            public Binary(Expr left, Token op, Expr right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        //< expr-binary
        //> expr-call
        public class Call : Expr
        {
            public List<Expr> arguments;

            public Expr callee;
            public Token paren;

            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        //< expr-call
        //> expr-get
        public class Get : Expr
        {
            public Token name;

            public Expr obj;

            public Get(Expr obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }

        //< expr-get
        //> expr-grouping
        public class Grouping : Expr
        {
            public Expr expression;

            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        //< expr-grouping
        //> expr-literal
        public class Literal : Expr
        {
            public object value;

            public Literal(object value)
            {
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        //< expr-literal
        //> expr-logical
        public class Logical : Expr
        {
            public Expr left;
            public Token op;
            public Expr right;

            public Logical(Expr left, Token op, Expr right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        //< expr-logical
        //> expr-set
        public class Set : Expr
        {
            public Token name;

            public Expr obj;
            public Expr value;

            public Set(Expr obj, Token name, Expr value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
        }

        //< expr-this
        //> expr-unary
        public class Unary : Expr
        {
            public Token op;
            public Expr right;

            public Unary(Token op, Expr right)
            {
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        //< expr-unary
        //> expr-variable
        public class Variable : Expr
        {
            public Token name;

            public Variable(Token name)
            {
                this.name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }
    }
}