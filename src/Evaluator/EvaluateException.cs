using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines evaluate exception class.
    /// </summary>
    public class EvaluateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the EvaluateException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EvaluateException(string message) : base(message)
        {
        }
    }
}
