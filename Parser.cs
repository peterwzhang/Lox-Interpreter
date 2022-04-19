using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    /// <summary>
    /// reads tokens and parses them
    /// </summary>
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        // method that begins parsing tokens
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
                statements.Add(Declaration());

            return statements;
        }

        // expands to Assignment
        private Expr Expression()
        {
            return Assignment();
        }

        // allows for variable declarations (parses variables)
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.FUN)) return Function("function");
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (Exception)
            {
                Synchronize();
                return null;
            }
        }

        // parses statements
        private Stmt Statement()
        {
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        // parses for loop
        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            // initializer for for loop
            Stmt initializer;
            if (Match(TokenType.SEMICOLON))
                initializer = null;
            else if (Match(TokenType.VAR))
                initializer = VarDeclaration();
            else
                initializer = ExpressionStatement();

            // sets condition to null
            Expr condition = null;
            if (!Check(TokenType.SEMICOLON)) condition = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            // increments index
            Expr increment = null;
            if (!Check(TokenType.RIGHT_PAREN)) increment = Expression();

            // body of for loop
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");
            var body = Statement();

            if (increment != null)
            {
                var tmp = new List<Stmt>
                {
                    body,
                    new Stmt.Expression(increment)
                };
                body = new Stmt.Block(tmp);
            }

            // checks condition
            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            // checks initializer
            if (initializer != null)
            {
                var tmp = new List<Stmt>
                {
                    initializer,
                    body
                };
                body = new Stmt.Block(tmp);
            }

            return body;
        }

        // parses if statements
        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition."); 
            
            var thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(TokenType.ELSE)) elseBranch = Statement();

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        // match and consume print token
        private Stmt PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        // parses return statement from function
        private Stmt ReturnStatement()
        {
            var keyword = Previous();
            Expr value = null;
            if (!Check(TokenType.SEMICOLON)) value = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword, value);
        }

        private Stmt VarDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL)) initializer = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        // parses while statement
        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            var body = Statement();

            return new Stmt.While(condition, body);
        }

        // parses and consumes an expression
        private Stmt ExpressionStatement()
        {
            var expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        // parses functions
        private Stmt.Function Function(string kind)
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            // parses parameter list and parentheses
            Consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");
            var parameters = new List<Token>();
            if (!Check(TokenType.RIGHT_PAREN))
                do
                {
                    parameters.Add(
                        Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (Match(TokenType.COMMA));

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            // parses body and wraps it in function node
            Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
            var body = Block();
            return new Stmt.Function(name, parameters, body);
        }

        // create empty list, parse statements and add them to the list as they are parsed
        private List<Stmt> Block()
        {
            var statements = new List<Stmt>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) statements.Add(Declaration());

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        // parses an assignment expression
        private Expr Assignment()
        {
            var expr = Or();

            if (Match(TokenType.EQUAL))
            {
                // recursively calls Assignment() on right hand side
                var equals = Previous(); //TODO: this is not used
                var value = Assignment();

                if (expr is Expr.Variable)
                {
                    var name = ((Expr.Variable) expr).name;
                    return new Expr.Assign(name, value);
                }

                if (expr is Expr.Get)
                {
                    var get = (Expr.Get) expr;
                    return new Expr.Set(get.obj, get.name, value);
                }

            }
            return expr;
        }

        // parses or expression
        private Expr Or()
        {
            var expr = And();

            // if one of the values is true
            while (Match(TokenType.OR))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        // parses and expression
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

        // checks for equality
        private Expr Equality()
        {
            var expr = Comparison();

            // checks if != or ==
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

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

        // parses binary operator for addition and subtraction
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

        // parses binary operator for multiplication and division
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

        // parses unary expression
        private Expr Unary()
        {
            // recursively call Unary() to parse operation if current token is ! or -
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            // otherwise, return primary expression (inside Call())
            return Call();
        }

        // parses the argument list
        private Expr FinishCall(Expr callee)
        {
            var arguments = new List<Expr>();
            if (!Check(TokenType.RIGHT_PAREN))
                do
                {
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));

            var paren = Consume(TokenType.RIGHT_PAREN,
                "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);
        }

        // parses function calls
        public Expr Call()
        {
            // parses primary expressions
            var expr = Primary();

            while (true)
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(TokenType.DOT))
                {
                    var name = Consume(TokenType.IDENTIFIER,
                        "Expect property name after '.'.");
                    expr = new Expr.Get(expr, name);
                }
                else
                {
                    break;
                }

            return expr;
        }

        // parses primary expression
        private Expr Primary()
        {
            // if expression is true, false, or null
            if (Match(TokenType.FALSE)) return new Expr.Literal(false);
            if (Match(TokenType.TRUE)) return new Expr.Literal(true);
            if (Match(TokenType.NIL)) return new Expr.Literal(null);

            // is expression is a number or a string
            if (Match(TokenType.NUMBER, TokenType.STRING)) return new Expr.Literal(Previous().literal);

            // if expression is an identifier
            if (Match(TokenType.IDENTIFIER)) return new Expr.Variable(Previous());

            // if expression is a left parenthesis (end of expression)
            if (Match(TokenType.LEFT_PAREN))
            {
                var expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }
            
            // won't get here! it's ok!
            return new Expr.Grouping(Expression());
        }

        /// <summary>
        /// checks if the token is of any the types in TokenTypes, consumes the token
        /// </summary>
        /// <param name="types"></param>
        /// <returns>boolean of whether the given token is of a certain type</returns>
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

        // checks if token is of desired type
        // if true, consumes token and continues
        private Token Consume(TokenType type, string message) //TODO: message is not used
        {
            if (Check(type)) return Advance();
            //TODO: is this right
            return Advance();
        }

        /// <summary>
        /// like Match() but doesn't consume the token
        /// </summary>
        /// <param name="type"></param>
        /// <returns>returns boolean of if a token is of a certain type</returns>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().type == type;
        }

        // consumes current token and returns it (similar to Advance() in Scanner.cs)
        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        // checks if at end of token list
        private bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        // looks at next token (doesn't consume)
        private Token Peek()
        {
            return tokens[current];
        }

        // returns most recently consumed token
        private Token Previous()
        {
            return tokens[current - 1];
        }

        // discards tokens until reaches statement boundary
        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.SEMICOLON) return;

                switch (Peek().type)
                {
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
}