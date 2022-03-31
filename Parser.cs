using System;
using System.Collections.Generic;
using System.Linq;

namespace LoxInterpreter.Properties
{
    public class Parser<T>
    {
        //< parse-error
        private List<Token> tokens;
        private int current = 0;

        Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        //> Statements and State parse
        List<Stmt<T>> parse()
        {
            List<Stmt<T>> statements = new List<Stmt<T>>();
            while (!isAtEnd())
            {
                /* Statements and State parse < Statements and State parse-declaration
                      statements.Add(statement());
                */
                //> parse-declaration
                statements.Add(declaration());
                //< parse-declaration
            }

            return statements; // [parse-error-handling]
        }

        //< Statements and State parse
        //> expression
        private Expr<T> expression()
        {
            /* Parsing Expressions expression < Statements and State expression
                return equality();
            */
            //> Statements and State expression
            return assignment();
            //< Statements and State expression
        }

        //< expression
        //> Statements and State declaration
        private Stmt<T> declaration()
        {
            try
            {
                //> Classes match-class
                //if (match(TokenType.CLASS)) return classDeclaration();
                //< Classes match-class
                //> Functions match-fun
                if (match(TokenType.FUN)) return function("function");
                //< Functions match-fun
                if (match(TokenType.VAR)) return varDeclaration();

                return statement();
            }
            catch (Exception error)
            {
                synchronize();
                return null;
            }
        }

        //< Statements and State declaration
        //> Classes parse-class-declaration
        // private Stmt<T> classDeclaration()
        // {
        //     Token name = consume(TokenType.IDENTIFIER, "Expect class name.");
        //     //> Inheritance parse-superclass
        //
        //     Expr<T>.Variable superclass = null;
        //     if (match(TokenType.LESS))
        //     {
        //         consume(TokenType.IDENTIFIER, "Expect superclass name.");
        //         superclass = new Expr<T>.Variable(previous());
        //     }
        //
        //     //< Inheritance parse-superclass
        //     consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");
        //
        //     var methods = new List<Stmt<T>.Function>();
        //     while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
        //     {
        //         methods.Add(function("method"));
        //     }
        //
        //     consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
        //
        //     /* Classes parse-class-declaration < Inheritance construct-class-ast
        //         return new Stmt<T>.Class(name, methods);
        //     */
        //     //> Inheritance construct-class-ast
        //     return new Stmt<T>.Class(name, superclass, methods);
        //     //< Inheritance construct-class-ast
        // }

        //< Classes parse-class-declaration
        //> Statements and State parse-statement
        private Stmt<T> statement()
        {
            //> Control Flow match-for
            if (match(TokenType.FOR)) return forStatement();
            //< Control Flow match-for
            //> Control Flow match-if
            if (match(TokenType.IF)) return ifStatement();
            //< Control Flow match-if
            if (match(TokenType.PRINT)) return printStatement();
            //> Functions match-return
            if (match(TokenType.RETURN)) return returnStatement();
            //< Functions match-return
            //> Control Flow match-while
            if (match(TokenType.WHILE)) return whileStatement();
            //< Control Flow match-while
            //> parse-block
            if (match(TokenType.LEFT_BRACE)) return new Stmt<T>.Block(block());
            //< parse-block

            return expressionStatement();
        }

        //< Statements and State parse-statement
        //> Control Flow for-statement
        private Stmt<T> forStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            /* Control Flow for-statement < Control Flow for-initializer
                // More here...
            */
            //> for-initializer
            Stmt<T> initializer;
            if (match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (match(TokenType.VAR))
            {
                initializer = varDeclaration();
            }
            else
            {
                initializer = expressionStatement();
            }
            //< for-initializer
            //> for-condition

            Expr<T> condition = null;
            if (!check(TokenType.SEMICOLON))
            {
                condition = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");
            //< for-condition
            //> for-increment

            Expr<T> increment = null;
            if (!check(TokenType.RIGHT_PAREN))
            {
                increment = expression();
            }

            consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");
            //< for-increment
            //> for-body
            Stmt<T> body = statement();

            //> for-desugar-increment
            if (increment != null)
            {
                List<Stmt<T>> tmp = new List<Stmt<T>>();
                tmp.Add(body);
                tmp.Add(new Stmt<T>.Expression(increment));
                body = new Stmt<T>.Block(tmp);
            }

            //< for-desugar-increment
            //> for-desugar-condition
            if (condition == null) condition = new Expr<T>.Literal(true);
            body = new Stmt<T>.While(condition, body);

            //< for-desugar-condition
            //> for-desugar-initializer
            if (initializer != null)
            {
                List<Stmt<T>> tmp = new List<Stmt<T>>();
                tmp.Add(initializer);
                tmp.Add(body);
                body = new Stmt<T>.Block(tmp);
            }

            //< for-desugar-initializer
            return body;
            //< for-body
        }

        //< Control Flow for-statement
        //> Control Flow if-statement
        private Stmt<T> ifStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr<T> condition = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition."); // [parens]

            Stmt<T> thenBranch = statement();
            Stmt<T> elseBranch = null;
            if (match(TokenType.ELSE))
            {
                elseBranch = statement();
            }

            return new Stmt<T>.If(condition, thenBranch, elseBranch);
        }

        //< Control Flow if-statement
        //> Statements and State parse-print-statement
        private Stmt<T> printStatement()
        {
            Expr<T> value = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt<T>.Print(value);
        }

        //< Statements and State parse-print-statement
        //> Functions parse-return-statement
        private Stmt<T> returnStatement()
        {
            Token keyword = previous();
            Expr<T> value = null;
            if (!check(TokenType.SEMICOLON))
            {
                value = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt<T>.Return(keyword, value);
        }

        //< Functions parse-return-statement
        //> Statements and State parse-var-declaration
        private Stmt<T> varDeclaration()
        {
            Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr<T> initializer = null;
            if (match(TokenType.EQUAL))
            {
                initializer = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt<T>.Var(name, initializer);
        }

        //< Statements and State parse-var-declaration
        //> Control Flow while-statement
        private Stmt<T> whileStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr<T> condition = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Stmt<T> body = statement();

            return new Stmt<T>.While(condition, body);
        }

        //< Control Flow while-statement
        //> Statements and State parse-expression-statement
        private Stmt<T> expressionStatement()
        {
            Expr<T> expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt<T>.Expression(expr);
        }

        //< Statements and State parse-expression-statement
        //> Functions parse-function
        private Stmt<T>.Function function(string kind)
        {
            Token name = consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            //> parse-parameters
            consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");
            List<Token> parameters = new List<Token>();
            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    // if (parameters.size() >= 255)
                    // {
                    //     //error(peek(), "Can't have more than 255 parameters.");
                    // }

                    parameters.Add(
                        consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (match(TokenType.COMMA));
            }

            consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");
            //< parse-parameters
            //> parse-body

            consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
            List<Stmt<T>> body = block();
            return new Stmt<T>.Function(name, parameters, body);
            //< parse-body
        }

        //< Functions parse-function
        //> Statements and State block
        private List<Stmt<T>> block()
        {
            List<Stmt<T>> statements = new List<Stmt<T>>();

            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        //< Statements and State block
        //> Statements and State parse-assignment
        private Expr<T> assignment()
        {
            /* Statements and State parse-assignment < Control Flow or-in-assignment
                Expr<T> expr = equality();
            */
            //> Control Flow or-in-assignment
            Expr<T> expr = or();
            //< Control Flow or-in-assignment

            if (match(TokenType.EQUAL))
            {
                Token equals = previous();
                Expr<T> value = assignment();

                if (expr is Expr<T>.Variable)
                {
                    Token name = ((Expr<T>.Variable) expr).name;
                    return new Expr<T>.Assign(name, value);
                    //> Classes assign-set
                }
                else if (expr is Expr<T>.Get)
                {
                    Expr<T>.Get get = (Expr<T>.Get) expr;
                    return new Expr<T>.Set(get.obj, get.name, value);
                    //< Classes assign-set
                }

                //error(equals, "Invalid assignment target."); // [no-throw]
            }

            return expr;
        }

        //< Statements and State parse-assignment
        //> Control Flow or
        private Expr<T> or()
        {
            Expr<T> expr = and();

            while (match(TokenType.OR))
            {
                Token op = previous();
                Expr<T> right = and();
                expr = new Expr<T>.Logical(expr, op, right);
            }

            return expr;
        }

        //< Control Flow or
        //> Control Flow and
        private Expr<T> and()
        {
            Expr<T> expr = equality();

            while (match(TokenType.AND))
            {
                Token op = previous();
                Expr<T> right = equality();
                expr = new Expr<T>.Logical(expr, op, right);
            }

            return expr;
        }

        //< Control Flow and
        //> equality
        private Expr<T> equality()
        {
            Expr<T> expr = comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = previous();
                Expr<T> right = comparison();
                expr = new Expr<T>.Binary(expr, op, right);
            }

            return expr;
        }

        //< equality
        //> comparison
        private Expr<T> comparison()
        {
            Expr<T> expr = term();

            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = previous();
                Expr<T> right = term();
                expr = new Expr<T>.Binary(expr, op, right);
            }

            return expr;
        }

        //< comparison
        //> term
        private Expr<T> term()
        {
            Expr<T> expr = factor();

            while (match(TokenType.MINUS, TokenType.PLUS))
            {
                Token op = previous();
                Expr<T> right = factor();
                expr = new Expr<T>.Binary(expr, op, right);
            }

            return expr;
        }

        //< term
        //> factor
        private Expr<T> factor()
        {
            Expr<T> expr = unary();

            while (match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = previous();
                Expr<T> right = unary();
                expr = new Expr<T>.Binary(expr, op, right);
            }

            return expr;
        }

        //< factor
        //> unary
        private Expr<T> unary()
        {
            if (match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = previous();
                Expr<T> right = unary();
                return new Expr<T>.Unary(op, right);
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
        private Expr<T> finishCall(Expr<T> callee)
        {
            List<Expr<T>> arguments = new List<Expr<T>>();
            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    //> check-max-arity
                    // if (arguments.size() >= 255)
                    // {
                    //     error(peek(), "Can't have more than 255 arguments.");
                    // }

                    //< check-max-arity
                    arguments.Add(expression());
                } while (match(TokenType.COMMA));
            }

            Token paren = consume(TokenType.RIGHT_PAREN,
                "Expect ')' after arguments.");

            return new Expr<T>.Call(callee, paren, arguments);
        }

        //< Functions finish-call
        //> Functions call
        public Expr<T> Call()
        {
            Expr<T> expr = primary();

            while (true)
            {
                // [while-true]
                if (match(TokenType.LEFT_PAREN))
                {
                    expr = finishCall(expr);
                    //> Classes parse-property
                }
                else if (match(TokenType.DOT))
                {
                    Token name = consume(TokenType.IDENTIFIER,
                        "Expect property name after '.'.");
                    expr = new Expr<T>.Get(expr, name);
                    //< Classes parse-property
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        //< Functions call
        //> primary
        private Expr<T> primary()
        {
            if (match(TokenType.FALSE)) return new Expr<T>.Literal(false);
            if (match(TokenType.TRUE)) return new Expr<T>.Literal(true);
            if (match(TokenType.NIL)) return new Expr<T>.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Expr<T>.Literal(previous().literal);
            }
            //> Inheritance parse-super

            // if (match(TokenType.SUPER))
            // {
            //     Token keyword = previous();
            //     consume(TokenType.DOT, "Expect '.' after 'super'.");
            //     Token method = consume(TokenType.IDENTIFIER,
            //         "Expect superclass method name.");
            //     return new Expr<T>.Super(keyword, method);
            // }
            // //< Inheritance parse-super
            // //> Classes parse-this
            //
            // if (match(TokenType.THIS)) return new Expr<T>.This(previous());
            
            //< Classes parse-this
            //> Statements and State parse-identifier

            if (match(TokenType.IDENTIFIER))
            {
                return new Expr<T>.Variable(previous());
            }
            //< Statements and State parse-identifier

            if (match(TokenType.LEFT_PAREN))
            {
                Expr<T> expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr<T>.Grouping(expr);
            }
            //> primary-error

            //throw error(peek(), "Expect expression.");
            //< primary-error

            //won't get here! it's ok!
            return new Expr<T>.Grouping(expression());
        }

        //< primary
        //> match
        private bool match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }

            return false;
        }

        //< match
        //> consume
        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();
            //TODO: is this right
            return advance();

            //throw error(peek(), message);
        }

        //< consume
        //> check
        private bool check(TokenType type)
        {
            if (isAtEnd()) return false;
            return peek().type == type;
        }

        //< check
        //> advance
        private Token advance()
        {
            if (!isAtEnd()) current++;
            return previous();
        }

        //< advance
        //> utils
        private bool isAtEnd()
        {
            return peek().type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
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
        private void synchronize()
        {
            advance();

            while (!isAtEnd())
            {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type)
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

                advance();
            }
        }
    }
    //< synchronize
}