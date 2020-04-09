using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines evaluator class to evaluate a expression's value.
    /// </summary>
    public class Evaluator
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Evaluator class to evaluatea expression's value.
        /// </summary>
        public Evaluator() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Evaluator class to evaluatea expression's value.
        /// </summary>
        public Evaluator(EvaluateContext context)
        {
            this.Context = context;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the evaluate context.
        /// </summary>
        public EvaluateContext Context
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Evaluate a expression's value.
        /// </summary>
        /// <param name="expr">The expression.</param>
        /// <param name="instance">The instance to get value from it.</param>
        /// <returns>The eval value</returns>
        public object Evaluate(string expr)
        {
            ParseContext parseContext = new ParseContext(Context?.ValueObject);
            Parser paser = new Parser(parseContext);
            Expression linqExpr = paser.Parse(expr);
            if (linqExpr == null)
            {
                throw new ParseException($"Invalid expression {expr}");
            }

            LambdaExpression lambdaExpr = Expression.Lambda(linqExpr);
            Delegate deleg = lambdaExpr.Compile();
            object value = deleg.DynamicInvoke();
            if (value is ExpressionValue)
            {
                value = ((ExpressionValue)value).Value;
            }
            return value;
        }
        #endregion
    }
}
