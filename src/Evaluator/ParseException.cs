using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines parse exception class.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ParseException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParseException(string message) : base(message)
        {
        }
    }
}
