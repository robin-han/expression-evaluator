using System;
using System.Collections;
using System.Text;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines ExpressionValue class to wrap expression's value during evaluating.
    /// </summary>
    internal class ExpressionValue
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the InvokeValue class.
        /// </summary>
        /// <param name="value">The actual value.</param>
        public ExpressionValue(object value)
        {
            if (value is ExpressionValue)
            {
                this.Value = ((ExpressionValue)value).Value;
            }
            else
            {
                this.Value = value;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the expression actual value.
        /// </summary>
        public object Value
        {
            get;
            private set;
        }
        #endregion

        #region Operator Overloading
        /// <summary>
        /// Overloading ExpressionValue to operator true.
        /// </summary>
        /// <param name="value">The expression value.</param>
        /// <returns>true</returns>
        public static bool operator true(ExpressionValue value)
        {
            return ToBoolean(value) == true;
        }

        /// <summary>
        /// Overloading ExpressionValue to operator false.
        /// </summary>
        /// <param name="value">The expression value.</param>
        /// <returns>false</returns>
        public static bool operator false(ExpressionValue value)
        {
            return ToBoolean(value) == false;
        }

        /// <summary>
        /// Returns a ExpressionValue whose value is the negated value of the specified instance.
        /// </summary>
        /// <param name="exprValue">The expression value.</param>
        /// <returns>A ExpressionValue whose value is negated.</returns>
        public static ExpressionValue operator -(ExpressionValue exprValue)
        {
            object value = exprValue.Value;

            if (IsNumber(value))
            {
                return new ExpressionValue(-ToDouble(value));
            }
            else if (IsTimeSpan(value))
            {
                return new ExpressionValue(-(TimeSpan)value);
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(value, "-"));
            }
        }

        /// <summary>
        /// Returns a ExpressionValue whose value is the ones complement value of the specified instance.
        /// </summary>
        /// <param name="exprValue">The expression value.</param>
        /// <returns>A ExpressionValue whose value is ones complement.</returns>
        public static ExpressionValue operator ~(ExpressionValue exprValue)
        {
            object value = exprValue.Value;

            if (IsNumber(value))
            {
                return new ExpressionValue(~ToInt64(value));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(value, "~"));
            }
        }

        /// <summary>
        /// Returns a ExpressionValue whose value is the logic not value of the specified instance.
        /// </summary>
        /// <param name="value">The expression value.</param>
        /// <returns>A ExpressionValue whose value is true or false.</returns>
        public static bool operator !(ExpressionValue value)
        {
            return !ToBoolean(value);
        }

        /// <summary>
        /// Adds two specified ExpressionValue instances.
        /// </summary>
        /// <param name="left">The left value to add.</param>
        /// <param name="right">The right value to add.</param>
        /// <returns>An ExpressionValue whose value is the sum of the values of t1 and t2.</returns>
        public static ExpressionValue operator +(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;

            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToDouble(v1) + ToDouble(v2));
            }
            else if (IsString(v1) || IsString(v2))
            {
                return new ExpressionValue(ToString(v1) + ToString(v2));
            }
            else if (IsDateTime(v1) && IsTimeSpan(v2))
            {
                return new ExpressionValue((DateTime)v1 + (TimeSpan)v2);
            }
            else if (IsTimeSpan(v1) && IsTimeSpan(v2))
            {
                return new ExpressionValue((TimeSpan)v1 + (TimeSpan)v2);
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "+"));
            }
        }

        /// <summary>
        /// Subtracts two specified ExpressionValue instances.
        /// </summary>
        /// <param name="left">The left value to subtract.</param>
        /// <param name="right">The right value to subtract.</param>
        /// <returns>An ExprssionValue whose value is the left subtract the right.</returns>
        public static ExpressionValue operator -(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;

            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToDouble(v1) - ToDouble(v2));
            }
            else if (IsDateTime(v1) && IsDateTime(v2))
            {
                return new ExpressionValue((DateTime)v1 - (DateTime)v2);
            }
            else if (IsDateTime(v1) && IsTimeSpan(v2))
            {
                return new ExpressionValue((DateTime)v1 - (TimeSpan)v2);
            }
            else if (IsTimeSpan(v1) && IsTimeSpan(v2))
            {
                return new ExpressionValue((TimeSpan)v1 - (TimeSpan)v2);
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "-"));
            }
        }

        /// <summary>
        /// Multiply two specified ExpressionValue instances.
        /// </summary>
        /// <param name="left">The left value to multiply.</param>
        /// <param name="right">The right value to multiply.</param>
        /// <returns>An ExpressionValue whose value is the left multiply the right.</returns>
        public static ExpressionValue operator *(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;

            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToDouble(v1) * ToDouble(v2));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "*"));
            }
        }

        /// <summary>
        /// Division two specified ExpressionValue instances.
        /// </summary>
        /// <param name="left">The left value to division.</param>
        /// <param name="right">The right value to division.</param>
        /// <returns>An ExpressionValue whose value is the left division the right.</returns>
        public static ExpressionValue operator /(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;

            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToDouble(v1) / ToDouble(v2));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "/"));
            }
        }

        /// <summary>
        /// Modulo two specified ExpressionValue instance.
        /// </summary>
        /// <param name="left">The left value to modulo.</param>
        /// <param name="right">The right value to modulo.</param>
        /// <returns>A ExpressionValue whose value is left modulo right.</returns>
        public static ExpressionValue operator %(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;
            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToDouble(v1) % ToDouble(v2));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "%"));
            }
        }

        /// <summary>
        /// Right shift expression value.
        /// </summary>
        /// <param name="left">The expression value.</param>
        /// <param name="right">The shift bit number.</param>
        /// <returns>The right shift value.</returns>
        public static ExpressionValue operator >>(ExpressionValue left, int right)
        {
            object value = left.Value;
            if (IsNumber(value))
            {
                return new ExpressionValue(ToInt64(value) >> right);
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(value, right, ">>"));
            }
        }

        /// <summary>
        /// Left shift expression value.
        /// </summary>
        /// <param name="left">The expression value.</param>
        /// <param name="right">The shift bit number.</param>
        /// <returns>The left shift value.</returns>
        public static ExpressionValue operator <<(ExpressionValue left, int right)
        {
            object value = left.Value;
            if (IsNumber(value))
            {
                return new ExpressionValue(ToInt64(value) << right);
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(value, right, "<<"));
            }
        }

        /// <summary>
        /// Compares whether the two instance are equable.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>true the two instance are equable, otherwise false.</returns>
        public static bool operator ==(ExpressionValue left, ExpressionValue right)
        {
            return Compare(left, right) == 0;
        }

        /// <summary>
        /// Compares whether the two instance are not equalble.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>true the two instance are not equable, otherwise false.</returns>
        public static bool operator !=(ExpressionValue left, ExpressionValue right)
        {
            return Compare(left, right) != 0;
        }

        /// <summary>
        /// Compares whether left instance greater than right.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>true left > right, otherwise false.</returns>
        public static bool operator >(ExpressionValue left, ExpressionValue right)
        {
            return Compare(left, right) == 1;
        }

        /// <summary>
        /// Compares whether left instance less than right.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>true left < right, otherwise false.</returns>
        public static bool operator <(ExpressionValue left, ExpressionValue right)
        {
            return Compare(left, right) == -1;
        }

        /// <summary>
        /// Compares whether left instance greater than or equal right.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>true left >= right, otherwise false.</returns>
        public static bool operator >=(ExpressionValue left, ExpressionValue right)
        {
            int result = Compare(left, right);
            return (result == 1 || result == 0);
        }

        /// <summary>
        /// Compares whether left instance less than or equal right.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>true left <= right, otherwise false.</returns>
        public static bool operator <=(ExpressionValue left, ExpressionValue right)
        {
            int result = Compare(left, right);
            return (result == -1 || result == 0);
        }

        /// <summary>
        /// Bit and two specified ExpressionValue instance.
        /// </summary>
        /// <param name="left">The left value to bit and.</param>
        /// <param name="right">The right value to bit and.</param>
        /// <returns>A ExpressionValue whose value is left bit and right.</returns>
        public static ExpressionValue operator &(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;
            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToInt64(v1) & ToInt64(v2));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "&"));
            }
        }

        /// <summary>
        /// Bit exclusive or two specified ExpressionValue instance.
        /// </summary>
        /// <param name="left">The left value to bit exclusive or.</param>
        /// <param name="right">The right value to bit exclusive or.</param>
        /// <returns>A ExpressionValue whose value is left bit exclusive or right.</returns>
        public static ExpressionValue operator ^(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;
            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToInt64(v1) ^ ToInt64(v2));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "^"));
            }
        }

        /// <summary>
        /// Bit or two specified ExpressionValue instance.
        /// </summary>
        /// <param name="left">The left value to bit or.</param>
        /// <param name="right">The right value to bit or.</param>
        /// <returns>A ExpressionValue whose value is left bit or right.</returns>
        public static ExpressionValue operator |(ExpressionValue left, ExpressionValue right)
        {
            object v1 = left.Value;
            object v2 = right.Value;
            if (IsNumber(v1) && IsNumber(v2))
            {
                return new ExpressionValue(ToInt64(v1) | ToInt64(v2));
            }
            else
            {
                throw new EvaluateException(OperatorOverloadingError(v1, v2, "|"));
            }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Gets current object's hashcode.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region
        /// <summary>
        /// Converts value to string.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <returns>The converted string.</returns>
        private static string ToString(object value)
        {
            if (value is ExpressionValue)
            {
                value = ((ExpressionValue)value).Value;
            }

            return Convert.ToString(value);
        }

        /// <summary>
        /// Converts value to double.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <returns>The double value.</returns>
        private static double ToDouble(object value)
        {
            if (value is ExpressionValue)
            {
                value = ((ExpressionValue)value).Value;
            }

            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts value to int32 and truncate fraction.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <returns>The int32 value.</returns>
        private static int ToInt32(object value)
        {
            return Convert.ToInt32(Math.Truncate(ToDouble(value)));
        }

        /// <summary>
        /// Converts value to int64 and truncate fraction.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <returns>The int64 value.</returns>
        private static long ToInt64(object value)
        {
            return Convert.ToInt64(Math.Truncate(ToDouble(value)));
        }

        /// <summary>
        /// Converts ExpressionValue to boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ToBoolean(object value)
        {
            if (value is ExpressionValue)
            {
                value = ((ExpressionValue)value).Value;
            }

            if (value is bool)
            {
                return (bool)value;
            }
            else if (IsNumber(value))
            {
                double d = Convert.ToDouble(value);
                return (d != 0);
            }
            else if (IsString(value))
            {
                string s = (string)value;
                return !string.IsNullOrEmpty(s);
            }
            else
            {
                return value != null;
            }
        }

        /// <summary>
        /// Compares the two object.
        /// </summary>
        /// <param name="x">The x object.</param>
        /// <param name="y">The y object.</param>
        /// <returns>-1:x<y; 0:x==y; 1:x>y </returns>
        private static int Compare(object x, object y)
        {
            if (x is ExpressionValue)
            {
                x = ((ExpressionValue)x).Value;
            }
            if (y is ExpressionValue)
            {
                y = ((ExpressionValue)y).Value;
            }

            //
            if ((x == null && y == null) || object.ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x == null && y != null)
            {
                return -1;
            }
            if (x != null && y == null)
            {
                return 1;
            }

            if (x is IList && y is IList)
            {
                IList xList = (IList)x;
                IList yList = (IList)y;
                if (xList.Count < yList.Count)
                {
                    return -1;
                }
                if (xList.Count > yList.Count)
                {
                    return 1;
                }
                for (int i = 0; i < xList.Count; i++)
                {
                    int result = Compare(xList[i], yList[i]);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                return 0;
            }

            //
            Type xType = x.GetType();
            Type yType = y.GetType();
            if (x is IComparable && (xType == yType || xType.IsAssignableFrom(yType)))
            {
                return ((IComparable)x).CompareTo(y);
            }

            if (y is IComparable && (yType == xType || yType.IsAssignableFrom(xType)))
            {
                return -((IComparable)y).CompareTo(x);
            }

            //
            if (IsNumber(x) && IsNumber(y))
            {
                double xd = ToDouble(x);
                double yd = ToDouble(y);
                return xd.CompareTo(yd);
            }
            else if (IsString(x) || IsString(y))
            {
                string xs = ToString(x);
                string ys = ToString(y);
                return xs.CompareTo(ys);
            }
            else
            {
                throw new EvaluateException($"Cannot compare the two object {x.GetType().FullName} and {y.GetType().FullName}");
            }
        }


        /// <summary>
        /// Check whether object is string object. 
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>true if the object is string object, otherwise false.</returns>
        private static bool IsString(object obj)
        {
            return obj is string;
        }

        /// <summary>
        /// Check whether object is number object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>true if the object is number object, otherwise false.</returns>
        private static bool IsNumber(object obj)
        {
            return obj is double
                || obj is float
                || obj is decimal
                || obj is long
                || obj is int
                || obj is short
                || obj is sbyte
                || obj is ulong
                || obj is uint
                || obj is ushort
                || obj is byte;
        }

        /// <summary>
        /// Check whether object is datetime object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>true if the object is datetime object, otherwise false.</returns>
        public static bool IsDateTime(object obj)
        {
            return obj is DateTime;
        }

        /// <summary>
        /// Check whether object is timespan object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>true if the object is timepsan, otherwise false.</returns>
        public static bool IsTimeSpan(object obj)
        {
            return obj is TimeSpan;
        }

        /// <summary>
        /// Throw exception when cannot do the operator overloading.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="op">The operator.</param>
        /// <returns>The error message.</returns>
        private static string OperatorOverloadingError(object obj, string op)
        {
            return $"Cannot do {op}{obj.GetType().Name}";
        }

        /// <summary>
        /// throw exception when cannot do the operator overloading.
        /// </summary>
        /// <param name="obj1">The object1.</param>
        /// <param name="obj2">The object2.</param>
        /// <param name="op">The operator.</param>
        /// <returns>The error message.</returns>
        private static string OperatorOverloadingError(object obj1, object obj2, string op)
        {
            return $"Cannot do {obj1.GetType().Name} {op} {obj2.GetType().Name}";
        }

        #endregion
    }
}
