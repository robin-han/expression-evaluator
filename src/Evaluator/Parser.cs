using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines parse class to parse a string to linq expression.
    /// </summary>
    public class Parser
    {
        #region Fileds
        private static readonly Expression True = Expression.Constant(new ExpressionValue(true));
        private static readonly Expression False = Expression.Constant(new ExpressionValue(false));
        private static readonly Expression Null = Expression.Constant(new ExpressionValue(null));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Parser class.
        /// </summary>
        /// <param name="context">The parse context.</param>
        public Parser(ParseContext context)
        {
            this.Context = context;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the context.
        /// </summary>
        public ParseContext Context
        {
            get;
            private set;
        }
        #endregion

        /// <summary>
        /// Parses the expresion string to linq expression.
        /// </summary>
        /// <param name="expr">The expression string. </param>
        /// <returns>The linq expression</returns>
        public Expression Parse(string expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException("expr");
            }

            Lexer lexer = new Lexer(expr);
            lexer.MoveNext();
            return this.Parse(lexer);
        }

        /// <summary>
        /// Parse to linq expression.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression Parse(Lexer lexer)
        {
            return ParseConditionOperator(lexer);
        }

        /// <summary>
        /// Parses condition(?:) operator. 
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseConditionOperator(Lexer lexer)
        {
            Expression expr = ParseLogicalcOrOperator(lexer);
            if (lexer.Token == Token.Question)
            {
                lexer.MoveNext();
                Expression ifTrueExpr = Parse(lexer);
                ValidateToken(lexer, Token.Colon);
                lexer.MoveNext();
                Expression ifFalseExpr = Parse(lexer);
                expr = Expression.Condition(Expression.IsTrue(expr), ifTrueExpr, ifFalseExpr);
            }
            return expr;
        }

        /// <summary>
        /// Parses logical or(||) operator.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed logical or expression.</returns>
        private Expression ParseLogicalcOrOperator(Lexer lexer)
        {
            Expression left = ParseLogicalAndOperator(lexer);
            while (lexer.Token == Token.BarBar)
            {
                lexer.MoveNext();
                Expression right = ParseLogicalAndOperator(lexer);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parses logical and(&&).
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed logical and expression.</returns>
        private Expression ParseLogicalAndOperator(Lexer lexer)
        {
            Expression left = ParseBitOrOperator(lexer);
            while (lexer.Token == Token.AmpersandAmpersand)
            {
                lexer.MoveNext();
                Expression right = ParseBitOrOperator(lexer);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parses bit or(|) operator;
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseBitOrOperator(Lexer lexer)
        {
            Expression left = ParseBitExclusiveOrOperator(lexer);
            while (lexer.Token == Token.Bar)
            {
                lexer.MoveNext();
                Expression right = ParseBitExclusiveOrOperator(lexer);
                left = Expression.Or(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parses bit exclusive or(^) operator.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseBitExclusiveOrOperator(Lexer lexer)
        {
            Expression left = ParseBitAndOperator(lexer);
            while (lexer.Token == Token.Caret)
            {
                lexer.MoveNext();
                Expression right = ParseBitAndOperator(lexer);
                left = Expression.ExclusiveOr(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parses bit and(&) operator.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The expression.</returns>
        private Expression ParseBitAndOperator(Lexer lexer)
        {
            Expression left = ParseEqualityOperator(lexer);
            while (lexer.Token == Token.Ampersand)
            {
                lexer.MoveNext();
                Expression right = ParseEqualityOperator(lexer);
                left = Expression.And(left, right);
            }
            return left;
        }

        /// <summary>
        /// Parses (==, !=) operator.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseEqualityOperator(Lexer lexer)
        {
            Expression left = ParseComparisonOperator(lexer);
            while (lexer.Token == Token.EqualEqual || lexer.Token == Token.ExclamationEqual)
            {
                TokenType tokenType = lexer.Token.Type;
                lexer.MoveNext();
                Expression right = ParseComparisonOperator(lexer);
                switch (tokenType)
                {
                    case TokenType.EqualsEqual:
                        left = Expression.Equal(left, right);
                        break;

                    case TokenType.ExclamationEqual:
                        left = Expression.NotEqual(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parse compare operator(>, <=, <, <=).
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseComparisonOperator(Lexer lexer)
        {
            Expression left = ParseBitShiftOperator(lexer);
            while (lexer.Token == Token.GreaterThan
                || lexer.Token == Token.GreaterThanEqual
                || lexer.Token == Token.LessThan
                || lexer.Token == Token.LessThanEqual)
            {
                TokenType tokenType = lexer.Token.Type;
                lexer.MoveNext();
                Expression right = ParseBitShiftOperator(lexer);
                switch (tokenType)
                {
                    case TokenType.GreaterThan:
                        left = Expression.GreaterThan(left, right);
                        break;

                    case TokenType.GreaterThanEqual:
                        left = Expression.GreaterThanOrEqual(left, right);
                        break;

                    case TokenType.LessThan:
                        left = Expression.LessThan(left, right);
                        break;

                    case TokenType.LessThanEqual:
                        left = Expression.LessThanOrEqual(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parses (<<, >>) operator.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseBitShiftOperator(Lexer lexer)
        {
            Expression left = ParseAddSubtractOperator(lexer);
            while (lexer.Token == Token.LessThanLessThan
                || lexer.Token == Token.GreaterThanGreaterThan)
            {
                TokenType tokenType = lexer.Token.Type;
                lexer.MoveNext();
                Expression right = ParseAddSubtractOperator(lexer);
                if (right is ConstantExpression)
                {
                    object value = ((ExpressionValue)((ConstantExpression)right).Value).Value;
                    if (int.TryParse(value == null ? "" : value.ToString(), out int bitCount))
                    {
                        right = Expression.Constant(bitCount);
                    }
                }
                switch (tokenType)
                {
                    case TokenType.LessThanLessThan:
                        left = Expression.LeftShift(left, right);
                        break;

                    case TokenType.GreaterThanGreaterThan:
                        left = Expression.RightShift(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parses (+, -) operator.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseAddSubtractOperator(Lexer lexer)
        {
            Expression left = ParseMultiplyDivideOperator(lexer);
            while (lexer.Token == Token.Plus || lexer.Token == Token.Minus)
            {
                TokenType tokenType = lexer.Token.Type;
                lexer.MoveNext();
                Expression right = ParseMultiplyDivideOperator(lexer);
                switch (tokenType)
                {
                    case TokenType.Plus:
                        left = Expression.Add(left, right);
                        break;

                    case TokenType.Minus:
                        left = Expression.Subtract(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parses (*, /, %) operator.
        /// </summary>
        /// <returns></returns>
        private Expression ParseMultiplyDivideOperator(Lexer lexer)
        {
            Expression left = ParseUnaryOperator(lexer);
            while (lexer.Token == Token.Asterisk
                || lexer.Token == Token.Slash
                || lexer.Token == Token.Percent)
            {
                TokenType tokenType = lexer.Token.Type;
                lexer.MoveNext();
                Expression right = ParseUnaryOperator(lexer);
                switch (tokenType)
                {
                    case TokenType.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;

                    case TokenType.Slash:
                        left = Expression.Divide(left, right);
                        break;

                    case TokenType.Percent:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        /// <summary>
        /// Parses unary operator(-, !, ~).
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expresion.</returns>
        private Expression ParseUnaryOperator(Lexer lexer)
        {
            if (lexer.Token == Token.Minus
                || lexer.Token == Token.Exclamation
                || lexer.Token == Token.Tilde)
            {
                TokenType tokenType = lexer.Token.Type;
                lexer.MoveNext();
                Expression expr = ParseUnaryOperator(lexer);
                switch (tokenType)
                {
                    case TokenType.Minus:
                        expr = Expression.Negate(expr);
                        break;

                    case TokenType.Exclamation:
                        expr = Expression.Not(expr);
                        break;

                    case TokenType.Tilde:
                        expr = Expression.OnesComplement(expr);
                        break;
                }
                return expr;
            }
            else if (lexer.Token == Token.Plus)
            {
                lexer.MoveNext();
                return ParseUnaryOperator(lexer);
            }
            else
            {
                return ParseOperand(lexer);
            }
        }

        /// <summary>
        /// Parses operand.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The operand expression.</returns>
        private Expression ParseOperand(Lexer lexer)
        {
            Expression expr;
            switch (lexer.Token.Type)
            {
                case TokenType.Dot:
                case TokenType.NumberLiteral:
                    expr = ParseNumber(lexer);
                    break;

                case TokenType.StringLiteral:
                    expr = ParseString(lexer);
                    break;

                case TokenType.Identifier:
                    expr = ParseIdentifier(lexer);
                    break;

                case TokenType.OpenParen:
                    expr = ParseParen(lexer);
                    break;

                default:
                    throw new ParseException($"Invalid token {lexer.Token.Text}.");
            }

            while (lexer.MoveNext())
            {
                if (lexer.Token == Token.Dot)
                {
                    expr = this.ParseDot(lexer, expr);
                }
                else if (lexer.Token == Token.OpenBracket)
                {
                    expr = this.ParseBracket(lexer, expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        /// <summary>
        /// Parse number literal to expression.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseNumber(Lexer lexer)
        {
            string preText = "";
            if (lexer.Token == Token.Dot)
            {
                lexer.MoveNext();
                preText = ".";
            }

            System.Diagnostics.Debug.Assert(lexer.Token.Type == TokenType.NumberLiteral);

            return Expression.Constant(new ExpressionValue(double.Parse(preText + lexer.Token.Text)));
        }

        /// <summary>
        /// Parses string literal to expression.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseString(Lexer lexer)
        {
            System.Diagnostics.Debug.Assert(lexer.Token.Type == TokenType.StringLiteral);

            return Expression.Constant(new ExpressionValue(lexer.Token.Text));
        }

        /// <summary>
        /// Parses identifier to expression.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The parsed expression.</returns>
        private Expression ParseIdentifier(Lexer lexer)
        {
            string identifier = lexer.Token.Text;
            switch (identifier)
            {
                case "true":
                    return True;

                case "false":
                    return False;

                case "null":
                    return Null;

                default:
                    return Expression.Call(
                        null,
                        Context.PropertyMethod,
                        Expression.Constant(Context.ValueObject),
                        Expression.Constant(identifier));
            }
        }

        /// <summary>
        /// Parses expression in paren.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The expression in paren.</returns>
        private Expression ParseParen(Lexer lexer)
        {
            System.Diagnostics.Debug.Assert(lexer.Token.Type == TokenType.OpenParen);

            lexer.MoveNext();
            Expression expr = Parse(lexer);
            ValidateToken(lexer, Token.CloseParen);
            return expr;
        }

        /// <summary>
        /// Parses bracket for index property.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <returns>The indexes expression.</returns>
        private Expression ParseBracket(Lexer lexer, Expression expr)
        {
            System.Diagnostics.Debug.Assert(lexer.Token.Type == TokenType.OpenBracket);

            Expression parameters = ParseParameters(lexer, Token.OpenBracket, Token.CloseBracket);
            return Expression.Call(
                null,
                Context.IndexMethod,
                expr,
                parameters);
        }

        /// <summary>
        /// Parse member access expression, eg: .a, .b()
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <param name="expr">The current expression.</param>
        /// <returns></returns>
        private Expression ParseDot(Lexer lexer, Expression expr)
        {
            System.Diagnostics.Debug.Assert(lexer.Token == Token.Dot);

            lexer.MoveNext();
            if (lexer.Token.Type != TokenType.Identifier)
            {
                throw new ParseException($"Cannot parse .{lexer.Token.Text}");
            }

            string name = lexer.Token.Text;
            Token nextToken = lexer.PeekToken();
            if (nextToken == Token.OpenParen)
            {
                lexer.MoveNext();
                Expression parameters = ParseParameters(lexer, Token.OpenParen, Token.CloseParen);
                return Expression.Call(
                    null,
                    Context.InvokeMethod,
                    expr,
                    Expression.Constant(name),
                    parameters);
            }
            else
            {
                return Expression.Call(
                    null,
                    Context.PropertyMethod,
                    expr,
                    Expression.Constant(name));
            }
        }

        /// <summary>
        /// Parses parameters for invoke method or index property.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <param name="openToken">The open token.</param>
        /// <param name="endToken">The end token.</param>
        /// <returns>The parameter expression.</returns>
        private Expression ParseParameters(Lexer lexer, Token openToken, Token endToken)
        {
            ValidateToken(lexer, openToken);
            List<Expression> parameters = new List<Expression>();
            while (lexer.MoveNext())
            {
                if (lexer.Token == endToken)
                {
                    break;
                }

                Expression expr = Parse(lexer);
                if (lexer.Token == Token.Comma)
                {
                    parameters.Add(expr);
                }
                else if (lexer.Token == endToken)
                {
                    parameters.Add(expr);
                    break;
                }
            }
            ValidateToken(lexer, endToken);

            return Expression.NewArrayInit(typeof(object), parameters);
        }

        /// <summary>
        /// Validates lexer's current token is expected token.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <param name="expectedToken">The expected token.</param>
        private void ValidateToken(Lexer lexer, Token expectedToken)
        {
            if (lexer.Token.Type != expectedToken.Type)
            {
                throw new ParseException($"Expected token {expectedToken.Text}, but {lexer.Token.Text}");
            }
        }
    }
}
