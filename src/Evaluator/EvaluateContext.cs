using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines evaluate context class.
    /// </summary>
    public class EvaluateContext
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ParseContext class.
        /// </summary>
        /// <param name="valueObj">The object to get property value or method value during evaluating expression.</param>
        public EvaluateContext(object valueObj)
        {
            this.ValueObject = valueObj;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the object that used to get its value during evalute expression.
        /// </summary>
        public object ValueObject
        {
            get;
            private set;
        }
        #endregion
    }
}
