using System;
using System.Collections.Generic;
using LoxInterpreter;
using LoxInterpreter.Properties;
using Environment = System.Environment;

namespace LoxInterpreter
{
  public class Interpreter<T> : Expr<T>.IVisitor, Stmt<T>.IVisitor {
//< Statements and State interpreter
/* Statements and State environment-field < Functions global-environment
  private Environment environment = new Environment();
*/
//> Functions global-environment
    static Environment globals = new Environment();

    Environment environment = globals;

//< Functions global-environment
//> Resolving and Binding locals-field
    private Dictionary<Expr<Stmt<T>>, int> locals = new Dictionary<Expr<Stmt<T>>, int>();
//< Resolving and Binding locals-field
//> Statements and State environment-field

//< Statements and State environment-field
//> Functions interpreter-constructor
    public Interpreter()
    {
      //TODO: instance of interface?
      globals.Define("clock", new Clock<T>());
    }

//< Functions interpreter-constructor
/* Evaluating Expressions interpret < Statements and State interpret
  void interpret(Expr<Stmt<T>> expression) { // [void]
    try {
      object value = Evaluate(expression);
      System.out.println(stringify(value));
    } catch (RuntimeError error) {
      Lox.runtimeError(error);
    }
  }
*/
//> Statements and State interpret
    public void Interpret(List<Stmt<T>> statements)
    {
      foreach (var statement in statements) 
      {
        Execute(statement);
      }
      // try
      // {
      //   for (Stmt statement :
      //   statements) {
      //     Execute(statement);
      //   }
      // }
      // catch (RuntimeError error)
      // {
      //   Lox.runtimeError(error);
      // }
    }

//< Statements and State interpret
//> Evaluate
    private object Evaluate(Expr<Stmt<T>> expr)
    {
      return expr.Accept(this);
    }

//< Evaluate
//> Statements and State Execute
    private void Execute(Stmt<T> stmt)
    {
      stmt.Accept(this);
    }

//< Statements and State Execute
//> Resolving and Binding resolve
    public void Resolve(Expr<Stmt<T>> expr, int depth)
    {
      locals.Add(expr, depth);
    }

//< Resolving and Binding resolve
//> Statements and State Execute-block
    public void ExecuteBlock(List<Stmt<T>> statements,
      Environment environment)
    {
      Environment previous = this.environment;
      try
      {
        this.environment = environment;

        foreach (var statement in statements) 
        {
          Execute(statement);
        }
      }
      finally
      {
        this.environment = previous;
      }
    }

//< Statements and State Execute-block
//> Statements and State Visit-block

    public void VisitBlockStmt(Stmt<T>.Block stmt)
    {
      ExecuteBlock(stmt.statements, new Environment(environment));
    }

//< Statements and State Visit-block
//> Classes interpreter-Visit-class
    

    public void VisitClassStmt(Stmt<T>.Class stmt)
    {
//> Inheritance interpret-superclass
      object superclass = null;
      if (stmt.superclass != null)
      {
        superclass = Evaluate(stmt.superclass);
        if (!(superclass instanceof LoxClass)) {
          throw new RuntimeError(stmt.superclass.name,
            "Superclass must be a class.");
        }
      }

//< Inheritance interpret-superclass
      environment.Define(stmt.name.lexeme, null);
//> Inheritance begin-superclass-environment

      if (stmt.superclass != null)
      {
        environment = new Environment(environment);
        environment.Define("super", superclass);
      }
//< Inheritance begin-superclass-environment
//> interpret-methods

      Map<string, LoxFunction> methods = new HashMap<>();
      for (Stmt<T>.Function method :
      stmt.methods) {
/* Classes interpret-methods < Classes interpreter-method-initializer
      LoxFunction function = new LoxFunction(method, environment);
*/
//> interpreter-method-initializer
        LoxFunction function = new LoxFunction(method, environment,
          method.name.lexeme.equals("init"));
//< interpreter-method-initializer
        methods.put(method.name.lexeme, function);
      }

/* Classes interpret-methods < Inheritance interpreter-construct-class
    LoxClass klass = new LoxClass(stmt.name.lexeme, methods);
*/
//> Inheritance interpreter-construct-class
      LoxClass klass = new LoxClass(stmt.name.lexeme,
        (LoxClass) superclass, methods);
//> end-superclass-environment

      if (superclass != null)
      {
        environment = environment.enclosing;
      }
//< end-superclass-environment

//< Inheritance interpreter-construct-class
//< interpret-methods
/* Classes interpreter-Visit-class < Classes interpret-methods
    LoxClass klass = new LoxClass(stmt.name.lexeme);
*/
      environment.assign(stmt.name, klass);
      return null;
    }

//< Classes interpreter-Visit-class
//> Statements and State Visit-expression-stmt
    

    public void VisitExpressionStmt(Stmt<T>.Expression stmt)
    {
      Evaluate(stmt.expression);
      return null;
    }

//< Statements and State Visit-expression-stmt
//> Functions Visit-function
    

    public void VisitFunctionStmt(Stmt<T>.Function stmt)
    {
/* Functions Visit-function < Functions Visit-closure
    LoxFunction function = new LoxFunction(stmt);
*/
/* Functions Visit-closure < Classes construct-function
    LoxFunction function = new LoxFunction(stmt, environment);
*/
//> Classes construct-function
      LoxFunction function = new LoxFunction(stmt, environment,
        false);
//< Classes construct-function
      environment.Define(stmt.name.lexeme, function);
      return null;
    }

//< Functions Visit-function
//> Control Flow Visit-if
    

    public void VisitIfStmt(Stmt<T>.If stmt)
    {
      if (isTruthy(Evaluate(stmt.condition)))
      {
        Execute(stmt.thenBranch);
      }
      else if (stmt.elseBranch != null)
      {
        Execute(stmt.elseBranch);
      }

      return null;
    }

//< Control Flow Visit-if
//> Statements and State Visit-print
    

    public void VisitPrintStmt(Stmt<T>.Print stmt)
    {
      object value = Evaluate(stmt.expression);
      System.out.println(stringify(value));
      return null;
    }

//< Statements and State Visit-print
//> Functions Visit-return
    

    public void VisitReturnStmt(Stmt<T>.Return stmt)
    {
      object value = null;
      if (stmt.value != null) value = Evaluate(stmt.value);

      throw new Stmt<>.Return(value);
    }

//< Functions Visit-return
//> Statements and State Visit-var
    

    public void VisitVarStmt(Stmt<T>.Var stmt)
    {
      object value = null;
      if (stmt.initializer != null)
      {
        value = Evaluate(stmt.initializer);
      }

      environment.Define(stmt.name.lexeme, value);
      return null;
    }

//< Statements and State Visit-var
//> Control Flow Visit-while
    

    public void VisitWhileStmt(Stmt<T>.While stmt)
    {
      while (isTruthy(Evaluate(stmt.condition)))
      {
        Execute(stmt.body);
      }

      return null;
    }

//< Control Flow Visit-while
//> Statements and State Visit-assign
    

    public object VisitAssignExpr(Expr<Stmt<T>>.Assign expr)
    {
      object value = Evaluate(expr.value);
/* Statements and State Visit-assign < Resolving and Binding resolved-assign
    environment.assign(expr.name, value);
*/
//> Resolving and Binding resolved-assign

      Integer distance = locals.get(expr);
      if (distance != null)
      {
        environment.assignAt(distance, expr.name, value);
      }
      else
      {
        globals.assign(expr.name, value);
      }

//< Resolving and Binding resolved-assign
      return value;
    }

//< Statements and State Visit-assign
//> Visit-binary
    

    public object VisitBinaryExpr(Expr<Stmt<T>>.Binary expr)
    {
      object left = Evaluate(expr.left);
      object right = Evaluate(expr.right); // [left]

      switch (expr.operator.

      type) {
//> binary-equality
        case BANG_EQUAL:
        return !isEqual(left, right);
        case EQUAL_EQUAL:
        return isEqual(left, right);
//< binary-equality
//> binary-comparison
        case GREATER:
//> check-greater-operand
        checkNumberOperands(expr.operator, left, right);
//< check-greater-operand
        return (double) left > (double) right;
        case GREATER_EQUAL:
//> check-greater-equal-operand
        checkNumberOperands(expr.operator, left, right);
//< check-greater-equal-operand
        return (double) left >= (double) right;
        case LESS:
//> check-less-operand
        checkNumberOperands(expr.operator, left, right);
//< check-less-operand
        return (double) left < (double) right;
        case LESS_EQUAL:
//> check-less-equal-operand
        checkNumberOperands(expr.operator, left, right);
//< check-less-equal-operand
        return (double) left <= (double) right;
//< binary-comparison
        case MINUS:
//> check-minus-operand
        checkNumberOperands(expr.operator, left, right);
//< check-minus-operand
        return (double) left - (double) right;
//> binary-plus
        case PLUS:
        if (left instanceof Double && right instanceof Double) {
          return (double) left + (double) right;
        } // [plus]

        if (left instanceof string && right instanceof string) {
          return (string) left + (string) right;
        }

/* Evaluating Expressions binary-plus < Evaluating Expressions string-wrong-type
        break;
*/
//> string-wrong-type
        throw new RuntimeError(expr.operator,
          "Operands must be two numbers or two strings.");
//< string-wrong-type
//< binary-plus
        case SLASH:
//> check-slash-operand
        checkNumberOperands(expr.operator, left, right);
//< check-slash-operand
        return (double) left / (double) right;
        case STAR:
//> check-star-operand
        checkNumberOperands(expr.operator, left, right);
//< check-star-operand
        return (double) left * (double) right;
      }

      // Unreachable.
      return null;
    }

//< Visit-binary
//> Functions Visit-call
    

    public object VisitCallExpr(Expr<Stmt<T>>.Call expr)
    {
      object callee = Evaluate(expr.callee);

      List<object> arguments = new ArrayList<>();
      for (Expr<Stmt<T>> argument :
      expr.arguments) {
        // [in-order]
        arguments.add(Evaluate(argument));
      }

//> check-is-callable
      if (!(callee instanceof LoxCallable)) {
        throw new RuntimeError(expr.paren,
          "Can only call functions and classes.");
      }

//< check-is-callable
      LoxCallable function = (LoxCallable) callee;
//> check-arity
      if (arguments.size() != function.arity())
      {
        throw new RuntimeError(expr.paren, "Expected " +
                                           function.arity() + " arguments but got " +
                                           arguments.size() + ".");
      }

//< check-arity
      return function.call(this, arguments);
    }

//< Functions Visit-call
//> Classes interpreter-Visit-get
    

    public object VisitGetExpr(Expr<Stmt<T>>.Get expr)
    {
      object object = Evaluate(expr.object);
      if (object instanceof LoxInstance) {
        return ((LoxInstance) object).get(expr.name);
      }

      throw new RuntimeError(expr.name,
        "Only instances have properties.");
    }

//< Classes interpreter-Visit-get
//> Visit-grouping
    

    public object VisitGroupingExpr(Expr<Stmt<T>>.Grouping expr)
    {
      return Evaluate(expr.expression);
    }

//< Visit-grouping
//> Visit-literal
    

    public object VisitLiteralExpr(Expr<Stmt<T>>.Literal expr)
    {
      return expr.value;
    }

//< Visit-literal
//> Control Flow Visit-logical
    

    public object VisitLogicalExpr(Expr<Stmt<T>>.Logical expr)
    {
      object left = Evaluate(expr.left);

      if (expr.operator.type == TokenType.OR) {
        if (isTruthy(left)) return left;
      } else {
        if (!isTruthy(left)) return left;
      }

      return Evaluate(expr.right);
    }

//< Control Flow Visit-logical
//> Classes interpreter-Visit-set
    

    public object VisitSetExpr(Expr<Stmt<T>>.Set expr)
    {
      object object = Evaluate(expr.object);

      if (!(object instanceof LoxInstance)) {
        // [order]
        throw new RuntimeError(expr.name,
          "Only instances have fields.");
      }

      object value = Evaluate(expr.value);
      ((LoxInstance) object).set(expr.name, value);
      return value;
    }

//< Classes interpreter-Visit-set
//> Inheritance interpreter-Visit-super
    

    public object VisitSuperExpr(Expr<Stmt<T>>.Super expr)
    {
      int distance = locals.get(expr);
      LoxClass superclass = (LoxClass) environment.getAt(
        distance, "super");
//> super-find-this

      LoxInstance object = (LoxInstance) environment.getAt(
        distance - 1, "this");
//< super-find-this
//> super-find-method

      LoxFunction method = superclass.findMethod(expr.method.lexeme);
//> super-no-method

      if (method == null)
      {
        throw new RuntimeError(expr.method,
          "Undefined property '" + expr.method.lexeme + "'.");
      }

//< super-no-method
      return method.bind(object);
//< super-find-method
    }

//< Inheritance interpreter-Visit-super
//> Classes interpreter-Visit-this
    

    public object VisitThisExpr(Expr<Stmt<T>>.This expr)
    {
      return lookUpVariable(expr.keyword, expr);
    }

//< Classes interpreter-Visit-this
//> Visit-unary
    

    public object VisitUnaryExpr(Expr<Stmt<T>>.Unary expr)
    {
      object right = Evaluate(expr.right);

      switch (expr.operator.

      type) {
//> unary-bang
        case BANG:
        return !isTruthy(right);
//< unary-bang
        case MINUS:
//> check-unary-operand
        checkNumberOperand(expr.operator, right);
//< check-unary-operand
        return -(double) right;
      }

      // Unreachable.
      return null;
    }

//< Visit-unary
//> Statements and State Visit-variable
    

    public object VisitVariableExpr(Expr<Stmt<T>>.Variable expr)
    {
/* Statements and State Visit-variable < Resolving and Binding call-look-up-variable
    return environment.get(expr.name);
*/
//> Resolving and Binding call-look-up-variable
      return lookUpVariable(expr.name, expr);
//< Resolving and Binding call-look-up-variable
    }

//> Resolving and Binding look-up-variable
    private object lookUpVariable(Token name, Expr<Stmt<T>> expr)
    {
      Integer distance = locals.get(expr);
      if (distance != null)
      {
        return environment.getAt(distance, name.lexeme);
      }
      else
      {
        return globals.get(name);
      }
    }

//< Resolving and Binding look-up-variable
//< Statements and State Visit-variable
//> check-operand
    private void checkNumberOperand(Token operator, object operand)
    {
      if (operand instanceof Double) return;
      throw new RuntimeError(operator, "Operand must be a number.");
    }

//< check-operand
//> check-operands
    private void checkNumberOperands(Token operator,
      object left, object right)
    {
      if (left instanceof Double && right instanceof Double) return;
      // [operand]
      throw new RuntimeError(operator, "Operands must be numbers.");
    }

//< check-operands
//> is-truthy
    private boolean isTruthy(object object) {
      if (object == null) return false;
      if (object instanceof Boolean) return (boolean) object;
      return true;
    }

//< is-truthy
//> is-equal
    private boolean isEqual(object a, object b)
    {
      if (a == null && b == null) return true;
      if (a == null) return false;

      return a.equals(b);
    }

//< is-equal
//> stringify
    private string stringify(object object) {
      if (object == null) return "nil";

      if (object instanceof Double) {
        string text = object.toString();
        if (text.endsWith(".0"))
        {
          text = text.substring(0, text.length() - 2);
        }

        return text;
      }

      return object.toString();
    }
//< stringify
  }
}