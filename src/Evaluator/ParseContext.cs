using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines parse context context to provide information for parser.
    /// </summary>
    public class ParseContext
    {
        /// <summary>
        /// Initializes a new instance of the ParseContext class.
        /// </summary>
        /// <param name="valueObj">The object to get its property value or method value</param>
        public ParseContext(object valueObj)
        {
            this.ValueObject = valueObj;

            Type objAccessor = typeof(MemberAccessor);
            this.PropertyMethod = objAccessor.GetMethod("Property");
            this.IndexMethod = objAccessor.GetMethod("Index");
            this.InvokeMethod = objAccessor.GetMethod("Invoke");
        }

        #region Properties
        /// <summary>
        /// Gets the object which to get its property value or invoke method value.
        /// </summary>
        public object ValueObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the method which is used to express's propery value.
        /// </summary>
        public MethodInfo PropertyMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the method which is used to express's index value.
        /// </summary>
        public MethodInfo IndexMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the method which is used to express's invoke method value.
        /// </summary>
        public MethodInfo InvokeMethod
        {
            get;
            private set;
        }
        #endregion

    }
}
