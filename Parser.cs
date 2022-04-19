using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    public class Parser
    {
        //< parse-error
        private readonly List<Token> tokens;
        private int current;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        //> Statements and State parse
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
                /* Statements and State parse < Statements and State parse-declaration
                          statements.Add(statement());
                    */
                //> parse-declaration
                statements.Add(Declaration());
            //< parse-declaration

            return statements; // [parse-error-handling]
        }

        //< Statements and State parse
        //> expression
        private Expr Expression()
        {
            /* Parsing Expressions expression < Statements and State expression
                return equality();
            */
            //> Statements and State expression
            return Assignment();
            //< Statements and State expression
        }

        //< expression
        //> Statements and State declaration
        private Stmt Declaration()
        {
            try
            {
                //> Classes match-class
                //if (match(TokenType.CLASS)) return classDeclaration();
                //< Classes match-class
                //> Functions match-fun
                if (Match(TokenType.FUN)) return Function("function");
                //< Functions match-fun
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (Exception)
            {
                Synchronize();
                return null;
            }
        }

        //< Statements and State declaration
        //> Classes parse-class-declaration
        // private Stmt classDeclaration()
        // {
        //     Token name = consume(TokenType.IDENTIFIER, "Expect class name.");
        //     //> Inheritance parse-superclass
        //
        //     Expr.Variable superclass = null;
        //     if (match(TokenType.LESS))
        //     {
        //         consume(TokenType.IDENTIFIER, "Expect superclass name.");
        //         superclass = new Expr.Variable(previous());
        //     }
        //
        //     //< Inheritance parse-superclass
        //     consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");
        //
        //     var methods = new List<Stmt.Function>();
        //     while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
        //     {
        //         methods.Add(function("method"));
        //     }
        //
        //     consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
        //
        //     /* Classes parse-class-declaration < Inheritance construct-class-ast
        //         return new Stmt.Class(name, methods);
        //     */
        //     //> Inheritance construct-class-ast
        //     return new Stmt.Class(name, superclass, methods);
        //     //< Inheritance construct-class-ast
        // }

        //< Classes parse-class-declaration
        //> Statements and State parse-statement
        private Stmt Statement()
        {
            //> Control Flow match-for
            if (Match(TokenType.FOR)) return ForStatement();
            //< Control Flow match-for
            //> Control Flow match-if
            if (Match(TokenType.IF)) return IfStatement();
            //< Control Flow match-if
            if (Match(TokenType.PRINT)) return PrintStatement();
            //> Functions match-return
            if (Match(TokenType.RETURN)) return ReturnStatement();
            //< Functions match-return
            //> Control Flow match-while
            if (Match(TokenType.WHILE)) return WhileStatement();
            //< Control Flow match-while
            //> parse-block
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());
            //< parse-block

            return ExpressionStatement();
        }

        //< Statements and State parse-statement
        //> Control Flow for-statement
        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            /* Control Flow for-statement < Control Flow for-initializer
                // More here...
            */
            //> for-initializer
            Stmt initializer;
            if (Match(TokenType.SEMICOLON))
                initializer = null;
            else if (Match(TokenType.VAR))
                initializer = VarDeclaration();
            else
                initializer = ExpressionStatement();
            //< for-initializer
            //> for-condition

            Expr condition = null;
            if (!Check(TokenType.SEMICOLON)) condition = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");
            //< for-condition
            //> for-increment

            Expr increment = null;
            if (!Check(TokenType.RIGHT_PAREN)) increment = Expression();

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");
            //< for-increment
            //> for-body
            var body = Statement();

            //> for-desugar-increment
            if (increment != null)
            {
                var tmp = new List<Stmt>
                {
                    body,
                    new Stmt.Expression(increment)
                };
                body = new Stmt.Block(tmp);
            }

            //< for-desugar-increment
            //> for-desugar-condition
            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            //< for-desugar-condition
            //> for-desugar-initializer
            if (initializer != null)
            {
                var tmp = new List<Stmt>
                {
                    initializer,
                    body
                };
                body = new Stmt.Block(tmp);
            }

            //< for-desugar-initializer
            return body;
            //< for-body
        }

        //< Control Flow for-statement
        //> Control Flow if-statement
        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition."); // [parens]

            var thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(TokenType.ELSE)) elseBranch = Statement();

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        //< Control Flow if-statement
        //> Statements and State parse-print-statement
        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        //< Statements and State parse-print-statement
        //> Functions parse-return-statement
        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            Expr value = null;
            if (!Check(TokenType.SEMICOLON)) value = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword, value);
        }

        //< Functions parse-return-statement
        //> Statements and State parse-var-declaration
        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL)) initializer = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        //< Statements and State parse-var-declaration
        //> Control Flow while-statement
        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            var body = Statement();

            return new Stmt.While(condition, body);
        }

        //< Control Flow while-statement
        //> Statements and State parse-expression-statement
        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        //< Statements and State parse-expression-statement
        //> Functions parse-function
        private Stmt.Function Function(string kind)
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            //> parse-parameters
            Consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");
            var parameters = new List<Token>();
            if (!Check(TokenType.RIGHT_PAREN))
                do
                {
                    // if (parameters.size() >= 255)
                    // {
                    //     //error(peek(), "Can't have more than 255 parameters.");
                    // }

                    parameters.Add(
                        Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (Match(TokenType.COMMA));

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");
            //< parse-parameters
            //> parse-body

            Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
            var body = Block();
            return new Stmt.Function(name, parameters, body);
            //< parse-body
        }

        //< Functions parse-function
        //> Statements and State block
        private List<Stmt> Block()
        {
            var statements = new List<Stmt>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) statements.Add(Declaration());

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        //< Statements and State block
        //> Statements and State parse-assignment
        private Expr Assignment()
        {
            /* Statements and State parse-assignment < Control Flow or-in-assignment
                Expr expr = equality();
            */
            //> Control Flow or-in-assignment
            var expr = Or();
            //< Control Flow or-in-assignment

            if (Match(TokenType.EQUAL))
            {
                var equals = Previous(); //TODO: this is not used
                var value = Assignment();

                if (expr is Expr.Variable)
                {
                    var name = ((Expr.Variable) expr).name;
                    return new Expr.Assign(name, value);
                    //> Classes assign-set
                }

                if (expr is Expr.Get)
                {
                    var get = (Expr.Get) expr;
                    return new Expr.Set(get.obj, get.name, value);
                    //< Classes assign-set
                }

                //error(equals, "Invalid assignment target."); // [no-throw]
            }

            return expr;
        }

        //< Statements and State parse-assignment
        //> Control Flow or
        private Expr Or()
        {
            var expr = And();

            while (Match(TokenType.OR))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        //< Control Flow or
        //> Control Flow and
        private Expr And()
        {
            var expr = Equality();

            while (Match(TokenType.AND))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        //< Control Flow and
        //> equality
        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //< equality
        //> comparison
        private Expr Comparison()
        {
            var expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var op = Previous();
                var right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //< comparison
        //> term
        private Expr Term()
        {
            var expr = Factor();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                var op = Previous();
                var right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //< term
        //> factor
        private Expr Factor()
        {
            var expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        //< factor
        //> unary
        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            /* Parsing Expressions unary < Functions unary-call
                return primary();
            */
            //> Functions unary-call
            return Call();
            //< Functions unary-call
        }

        //< unary
        //> Functions finish-call
        private Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!Check(TokenType.RIGHT_PAREN))
                do
                {
                    //> check-max-arity
                    // if (arguments.size() >= 255)
                    // {
                    //     error(peek(), "Can't have more than 255 arguments.");
                    // }

                    //< check-max-arity
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));

            var paren = Consume(TokenType.RIGHT_PAREN,
                "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);
        }

        //< Functions finish-call
        //> Functions call
        public Expr Call()
        {
            var expr = Primary();

            while (true)
                // [while-true]
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                    //> Classes parse-property
                }
                else if (Match(TokenType.DOT))
                {
                    var name = Consume(TokenType.IDENTIFIER,
                        "Expect property name after '.'.");
                    expr = new Expr.Get(expr, name);
                    //< Classes parse-property
                }
                else
                {
                    break;
                }

            return expr;
        }

        //< Functions call
        //> primary
        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING)) return new Expr.Literal(Previous().literal);
            //> Inheritance parse-super

            // if (match(TokenType.SUPER))
            // {
            //     Token keyword = previous();
            //     consume(TokenType.DOT, "Expect '.' after 'super'.");
            //     Token method = consume(TokenType.IDENTIFIER,
            //         "Expect superclass method name.");
            //     return new Expr.Super(keyword, method);
            // }
            // //< Inheritance parse-super
            // //> Classes parse-this
            //
            // if (match(TokenType.THIS)) return new Expr.This(previous());

            //< Classes parse-this
            //> Statements and State parse-identifier

            if (Match(TokenType.IDENTIFIER)) return new Expr.Variable(Previous());
            //< Statements and State parse-identifier

            if (Match(TokenType.LEFT_PAREN))
            {
                var expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
            //> primary-error

            //throw error(peek(), "Expect expression.");
            //< primary-error

            //won't get here! it's ok!
            return new Expr.Grouping(Expression());
        }

        //< primary
        //> match
        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
                if (Check(type))
                {
                    Advance();
                    return true;
                }

            return false;
        }

        //< match
        //> consume
        private Token Consume(TokenType type, string message) //TODO: message is not used
        {
            if (Check(type)) return Advance();
            //TODO: is this right
            return Advance();

            //throw error(peek(), message);
        }

        //< consume
        //> check
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().type == type;
        }

        //< check
        //> advance
        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        //< advance
        //> utils
        private bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        //< utils
        //> error
        // private ParseError error(Token token, string message)
        // {
        //     Lox.error(token, message);
        //     return new ParseError();
        //}

        //< error
        //> synchronize
        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.SEMICOLON) return;

                switch (Peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }
    //< synchronize
}