using System;
using System.Collections.Generic;
using LoxInterpreter.Properties;

namespace LoxInterpreter
{
  public abstract class Stmt
  {
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
      public Expression(Expr expression)
      {
        this.expression = expression;
      }

      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitExpressionStmt(this);
      }

      public Expr expression;
    }

//< stmt-expression
//> stmt-function
    public class Function : Stmt
    {
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

      public Token name;
      public List<Token> parms;
      public List<Stmt> body;
    }

//< stmt-function
//> stmt-if
    public class If : Stmt
    {
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

      public Expr condition;
      public Stmt thenBranch;
      public Stmt elseBranch;
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
      public Return(Token keyword, Expr value)
      {
        this.keyword = keyword;
        this.value = value;
      }

      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitReturnStmt(this);
      }

      public Token keyword;
      public Expr value;
    }

//< stmt-return
//> stmt-var
    public class Var : Stmt
    {
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

      public Token name;
      public Expr initializer;
    }

//< stmt-var
//> stmt-while
    public class While : Stmt
    {
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

      public Expr condition;
      public Stmt body;
    }
//< stmt-while
    public abstract T Accept<T>(IVisitor<T> visitor);
  }
}
//< Appendix II stmt