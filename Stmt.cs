using System.Collections.Generic;

namespace LoxInterpreter
{
    /// <summary>
    /// statement class that holds all types of statements
    /// </summary>
    public abstract class Stmt
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        // IVisitor interface with all relevant statement types
        public interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
        }

        // calls IVisit interface on given block statements
        public class Block : Stmt
        {
            public List<Stmt> statements;

            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

        // calls IVisit interface on given expressions
        public class Expression : Stmt
        {
            public Expr expression;

            public Expression(Expr expression)
            {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        // calls IVisit interface on given functions
        public class Function : Stmt
        {
            public List<Stmt> body;

            public Token name;
            public List<Token> parms;

            public Function(Token name, List<Token> parms, List<Stmt> body)
            {
                this.name = name;
                this.parms = parms;
                this.body = body;
            }

            public override
                T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
        }

        // calls IVisit interface on given if statements
        public class If : Stmt
        {
            public Expr condition;
            public Stmt elseBranch;
            public Stmt thenBranch;

            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitIfStmt(this);
            }
        }

        // calls IVisit interface on given print statements
        public class Print : Stmt
        {
            public Expr expression;

            public Print(Expr expression)
            {
                this.expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        // calls IVisit interface on given return statements
        public class Return : Stmt
        {
            public readonly Token keyword;
            public readonly Expr value;

            public Return(Token keyword, Expr value)
            {
                this.keyword = keyword;
                this.value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }
        }

        // calls IVisit interface on given variables
        public class Var : Stmt
        {
            public Expr initializer;

            public Token name;

            public Var(Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }

            public override
                T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

        // calls IVisit interface on given while statements
        public class While : Stmt
        {
            public Stmt body;

            public Expr condition;

            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            public override
                T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }
    }
}