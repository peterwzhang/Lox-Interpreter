using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
  public abstract class Expr
  {
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
      // T VisitSuperExpr(Super expr);
      // T VisitThisExpr(This expr);
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

      public Expr left;
      public Token op;
      public Expr right;
    }

    //< expr-binary
    //> expr-call
    public class Call : Expr
    {
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

      public Expr callee;
      public Token paren;
      public List<Expr> arguments;
    }

    //< expr-call
    //> expr-get
    public class Get : Expr
    {
      public Get(Expr obj, Token name) {
        this.obj = obj;
        this.name = name;
      }
    
      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitGetExpr(this);
      }
    
      public Expr obj;
      public Token name;
    }

    //< expr-get
    //> expr-grouping
    public class Grouping : Expr
    {
      public Grouping(Expr expression)
      {
        this.expression = expression;
      }

      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitGroupingExpr(this);
      }

      public Expr expression;
    }

    //< expr-grouping
    //> expr-literal
    public class Literal : Expr
    {
      public Literal(Object value)
      {
        this.value = value;
      }
    
      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitLiteralExpr(this);
      }
    
      public Object value;
    }

    //< expr-literal
    //> expr-logical
    public class Logical : Expr
    {
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

      public Expr left;
      public Token op;
      public Expr right;
    }

    //< expr-logical
    //> expr-set
    public class Set : Expr
    {
      public Set(Expr obj, Token name, Expr value) {
        this.obj = obj;
        this.name = name;
        this.value = value;
      }
    
      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitSetExpr(this);
      }
    
      public Expr obj;
      public Token name;
      public Expr value;
    }

    //< expr-set
    //> expr-super
    // public class Super : Expr
    // {
    //   public Super(Token keyword, Token method)
    //   {
    //     this.keyword = keyword;
    //     this.method = method;
    //   }
    //
    //   public override T Accept<T>(IVisitor<T> visitor)
    //   {
    //     return visitor.VisitSuperExpr(this);
    //   }
    //
    //   public Token keyword;
    //   public Token method;
    // }

    //< expr-super
    //> expr-this
    // public class This : Expr
    // {
    //   public This(Token keyword)
    //   {
    //     this.keyword = keyword;
    //   }
    //
    //   public override T Accept<T>(IVisitor<T> visitor)
    //   {
    //     return visitor.VisitThisExpr(this);
    //   }
    //
    //   public Token keyword;
    // }

    //< expr-this
    //> expr-unary
    public class Unary : Expr
    {
      public Unary(Token op, Expr right)
      {
        this.op = op;
        this.right = right;
      }
    
      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitUnaryExpr(this);
      }
    
      public Token op;
      public Expr right;
    }

    //< expr-unary
    //> expr-variable
    public class Variable : Expr
    {
      public Variable(Token name)
      {
        this.name = name;
      }

      public override T Accept<T>(IVisitor<T> visitor)
      {
        return visitor.VisitVariableExpr(this);
      }

      public Token name;
    }
    //< expr-variable

    public abstract T Accept<T>(IVisitor<T> visitor);
  }
}