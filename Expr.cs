using System;
using System.Collections.Generic;

namespace LoxInterpreter.Properties
{
  public abstract class Expr<T>
  {
    public interface IVisitor
    {
      T VisitAssignExpr(Assign expr);
      T VisitBinaryExpr(Binary expr);
      T VisitCallExpr(Call expr);
      T VisitGetExpr(Get expr);
      T VisitGroupingExpr(Grouping expr);
      T VisitLiteralExpr(Literal expr);
      T VisitLogicalExpr(Logical expr);
      T VisitSetExpr(Set expr);
      T VisitSuperExpr(Super expr);
      T VisitThisExpr(This expr);
      T VisitUnaryExpr(Unary expr);
      T VisitVariableExpr(Variable expr);
    }

    // Nested Expr<T> classes here...
    //> expr-assign
    public class Assign : Expr<T>
    {

      public Token name;
      public Expr<T> value;
      public Assign(Token name, Expr<T> value)
      {
        this.name = name;
        this.value = value;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitAssignExpr(this);
      }
    }

    //< expr-assign
    //> expr-binary
    public class Binary : Expr<T>
    {
      public Binary(Expr<T> left, Token op, Expr<T> right)
      {
        this.left = left;
        this.op = op;
        this.right = right;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitBinaryExpr(this);
      }

      public Expr<T> left;
      public Token op;
      public Expr<T> right;
    }

    //< expr-binary
    //> expr-call
    public class Call : Expr<T>
    {
      public Call(Expr<T> callee, Token paren, List<Expr<T>> arguments)
      {
        this.callee = callee;
        this.paren = paren;
        this.arguments = arguments;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitCallExpr(this);
      }

      public Expr<T> callee;
      public Token paren;
      public List<Expr<T>> arguments;
    }

    //< expr-call
    //> expr-get
    public class Get : Expr<T>
    {
      public Get(Expr<T> obj, Token name) {
        this.obj = obj;
        this.name = name;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitGetExpr(this);
      }

      public Expr<T> obj;
      public Token name;
    }

    //< expr-get
    //> expr-grouping
    public class Grouping : Expr<T>
    {
      public Grouping(Expr<T> expression)
      {
        this.expression = expression;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitGroupingExpr(this);
      }

      public Expr<T> expression;
    }

    //< expr-grouping
    //> expr-literal
    public class Literal : Expr<T>
    {
      public Literal(Object value)
      {
        this.value = value;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitLiteralExpr(this);
      }

      public Object value;
    }

    //< expr-literal
    //> expr-logical
    public class Logical : Expr<T>
    {
      public Logical(Expr<T> left, Token op, Expr<T> right)
      {
        this.left = left;
        this.op = op;
        this.right = right;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitLogicalExpr(this);
      }

      public Expr<T> left;
      public Token op;
      public Expr<T> right;
    }

    //< expr-logical
    //> expr-set
    public class Set : Expr<T>
    {
      public Set(Expr<T> obj, Token name, Expr<T> value) {
        this.obj = obj;
        this.name = name;
        this.value = value;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitSetExpr(this);
      }

      public Expr<T> obj;
      public Token name;
      public Expr<T> value;
    }

    //< expr-set
    //> expr-super
    public class Super : Expr<T>
    {
      public Super(Token keyword, Token method)
      {
        this.keyword = keyword;
        this.method = method;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitSuperExpr(this);
      }

      public Token keyword;
      public Token method;
    }

    //< expr-super
    //> expr-this
    public class This : Expr<T>
    {
      public This(Token keyword)
      {
        this.keyword = keyword;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitThisExpr(this);
      }

      public Token keyword;
    }

    //< expr-this
    //> expr-unary
    public class Unary : Expr<T>
    {
      public Unary(Token op, Expr<T> right)
      {
        this.op = op;
        this.right = right;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitUnaryExpr(this);
      }

      public Token op;
      public Expr<T> right;
    }

    //< expr-unary
    //> expr-variable
    public class Variable : Expr<T>
    {
      public Variable(Token name)
      {
        this.name = name;
      }

      public override T Accept(IVisitor visitor)
      {
        return visitor.VisitVariableExpr(this);
      }

      public Token name;
    }
    //< expr-variable

    public abstract T Accept(IVisitor visitor);
  }
}