using System.Collections.Generic;

namespace LoxInterpreter
{
    public abstract class Stmt
    {
//< stmt-while
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T VisitBlockStmt(Block stmt);

            //T VisitClassStmt(Class stmt);
            T VisitExpressionStmt(Expression stmt);
            T VisitFunctionStmt(Function stmt);
            T VisitIfStmt(If stmt);
            T VisitPrintStmt(Print stmt);
            T VisitReturnStmt(Return stmt);
            T VisitVarStmt(Var stmt);
            T VisitWhileStmt(While stmt);
        }

        // Nested Stmt classes here...
//> stmt-block
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

//< stmt-block
//> stmt-class
        // public class Class : Stmt
        // {
        //
        //   public Token name;
        //   public Expr.Variable superclass;
        //   public List<Stmt.Function> methods;
        //
        //   public Class(Token name,
        //     Expr.Variable superclass,
        //     List<Stmt.Function> methods)
        //   {
        //     this.name = name;
        //     this.superclass = superclass;
        //     this.methods = methods;
        //   }
        //
        //   public override T Accept<T>(IVisitor visitor)
        //   {
        //     return visitor.VisitClassStmt(this);
        //   }
        // }

//< stmt-class
//> stmt-expression
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

//< stmt-expression
//> stmt-function
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

//< stmt-function
//> stmt-if
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

//< stmt-if
//> stmt-print
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

//< stmt-print
//> stmt-return
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

//< stmt-return
//> stmt-var
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

//< stmt-var
//> stmt-while
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
//< Appendix II stmt