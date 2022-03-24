using System;
using System.Collections.Generic;
using LoxInterpreter.Properties;

namespace LoxInterpreter
{
  abstract public class Stmt<T>
  {
    public interface IVisitor
    {
      T VisitBlockStmt(Block stmt);
      T VisitClassStmt(Class stmt);
      T VisitExpressionStmt(Expression stmt);
      T VisitFunctionStmt(Function stmt);
      T VisitIfStmt(If stmt);
      T VisitPrintStmt(Print stmt);
      T VisitReturnStmt(Return stmt);
      T VisitVarStmt(Var stmt);
      T VisitWhileStmt(While stmt);
    }

    // Nested Stmt<T> classes here...
//> stmt-block
    public class Block : Stmt<T>
    {

      List<Stmt<T>> statements;

      Block(List<Stmt<T>> statements)
      {
        this.statements = statements;
      }

      public override T accept(IVisitor visitor)
      {
        return visitor.VisitBlockStmt(this);
      }
    }

//< stmt-block
//> stmt-class
    public class Class : Stmt<T>
    {

      Token name;
      Expr<T>.Variable superclass;
      List<Stmt<T>.Function> methods;

      public Class(Token name,
        Expr<T>.Variable superclass,
        List<Stmt<T>.Function> methods)
      {
        this.name = name;
        this.superclass = superclass;
        this.methods = methods;
      }

      public override T accept(IVisitor visitor)
      {
        return visitor.VisitClassStmt(this);
      }
    }

//< stmt-class
//> stmt-expression
    public class Expression : Stmt<T>
    {
      Expression(Expr<T> expression)
      {
        this.expression = expression;
      }

      public override T accept(IVisitor visitor)
      {
        return visitor.VisitExpressionStmt(this);
      }

      Expr<T> expression;
    }

//< stmt-expression
//> stmt-function
    public class Function : Stmt<T>
    {
      Function(Token name, List<Token> parms, List<Stmt<T>> body)
      {
        this.name = name;
        this.parms = parms;
        this.body = body;
      }

      public override
        T accept(IVisitor visitor)
      {
        return visitor.VisitFunctionStmt(this);
      }

      Token name;
      List<Token> parms;
      List<Stmt<T>> body;
    }

//< stmt-function
//> stmt-if
    public class If : Stmt<T>
    {
      If(Expr<T> condition, Stmt<T> thenBranch, Stmt<T> elseBranch)
      {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
      }

      public override
        T accept(IVisitor visitor)
      {
        return visitor.VisitIfStmt(this);
      }

      Expr<T> condition;
      Stmt<T> thenBranch;
      Stmt<T> elseBranch;
    }

//< stmt-if
//> stmt-print
    public class Print : Stmt<T>
    {
      Print(Expr<T> expression)
      {
        this.expression = expression;
      }

      public override
        T accept(IVisitor visitor)
      {
        return visitor.VisitPrintStmt(this);
      }

      Expr<T> expression;
    }

//< stmt-print
//> stmt-return
    public class Return : Stmt<T>
    {
      Return(Token keyword, Expr<T> value)
      {
        this.keyword = keyword;
        this.value = value;
      }

      public override
        T accept(IVisitor visitor)
      {
        return visitor.VisitReturnStmt(this);
      }

      Token keyword;
      Expr<T> value;
    }

//< stmt-return
//> stmt-var
    public class Var : Stmt<T>
    {
      Var(Token name, Expr<T> initializer)
      {
        this.name = name;
        this.initializer = initializer;
      }

      public override
        T accept(IVisitor visitor)
      {
        return visitor.VisitVarStmt(this);
      }

      Token name;
      Expr<T> initializer;
    }

//< stmt-var
//> stmt-while
    public class While : Stmt<T>
    {
      While(Expr<T> condition, Stmt<T> body)
      {
        this.condition = condition;
        this.body = body;
      }

      public override
        T accept(IVisitor visitor)
      {
        return visitor.VisitWhileStmt(this);
      }

      Expr<T> condition;
      Stmt<T> body;
    }
//< stmt-while

    public abstract T accept(IVisitor visitor);
  }
}
//< Appendix II stmt