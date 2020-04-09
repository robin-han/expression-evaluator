using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Code.Expressions.CSharp
{
    /// <summary>
    /// Defines ObjectAccessor class get object's property value or invoke object's method.
    /// </summary>
    internal class MemberAccessor
    {
        /// <summary>
        /// Gets a property's value.
        /// </summary>
        /// <param name="instance">The instance to get value</param>
        /// <param name="name">The property name</param>
        /// <returns>The property value.</returns>
        public static ExpressionValue Property(object instance, string name)
        {
            instance = CoerceInstance(instance);
            if (instance is IDictionary<string, object>)
            {
                IDictionary<string, object> dic = (IDictionary<string, object>)instance;
                if (dic.ContainsKey(name))
                {
                    return new ExpressionValue(dic[name]);
                }
            }

            Type instanceType = instance.GetType();
            PropertyInfo propInfo = instanceType.GetProperty(name);
            if (propInfo != null)
            {
                return new ExpressionValue(propInfo.GetValue(instance));
            }

            FieldInfo fieldInfo = instanceType.GetField(name);
            if (fieldInfo != null)
            {
                return new ExpressionValue(fieldInfo.GetValue(instance));
            }

            throw new EvaluateException($"Cannot get property {name} from instance.");
        }

        /// <summary>
        /// Gets value at given index.
        /// </summary>
        /// <param name="instance">The instance to get its value</param>
        /// <param name="name"></param>
        /// <returns>The index value.</returns>
        public static ExpressionValue Index(object instance, object[] indexes)
        {
            instance = CoerceInstance(instance);
            indexes = CoerceArray(indexes);

            if (instance is IList)
            {
                for (int i = 0; i < indexes.Length; i++)
                {
                    indexes[i] = Convert.ToInt32(indexes[i]);
                }
            }

            PropertyInfo propInfo = instance.GetType().GetProperty("Item");
            if (propInfo != null)
            {
                return new ExpressionValue(propInfo.GetValue(instance, indexes));
            }

            throw new EvaluateException("Cannnot get index value from Item");
        }

        /// <summary>
        /// Invokes a method and gets its value.
        /// </summary>
        /// <param name="instance">The instance to invoke method.</param>
        /// <param name="name">The method name.</param>
        /// <param name="parameters">The inovke parameters.</param>
        /// <returns></returns>
        public static ExpressionValue Invoke(object instance, string name, object[] parameters)
        {
            instance = CoerceInstance(instance);
            parameters = CoerceArray(parameters);

            MethodInfo methodInfo = FindMethod(instance.GetType(), name, parameters);
            if (methodInfo != null)
            {
                if (methodInfo.ReturnType == null)
                {
                    return null;
                }
                return new ExpressionValue(methodInfo.Invoke(instance, parameters));
            }

            throw new EvaluateException($"Cannot invoke method {name} from instance.");
        }

        /// <summary>
        /// Find method from a type by given name and parameters.
        /// </summary>
        /// <param name="type">The object type to find method.</param>
        /// <param name="name">The method name.</param>
        /// <param name="parameters">The method parameters.</param>
        /// <returns>The method.</returns>
        private static MethodInfo FindMethod(Type type, string name, object[] parameters)
        {
            int parameterCount = parameters == null ? 0 : parameters.Length;
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == name && method.GetParameters().Length == parameterCount)
                {
                    return method;
                }
            }
            return null;
        }

        /// <summary>
        /// Get actual array items.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static object[] CoerceArray(object[] array)
        {
            if (array == null)
            {
                return null;
            }

            for (int i = 0; i < array.Length; i++)
            {
                object item = array[i];
                if (item is ExpressionValue)
                {
                    array[i] = ((ExpressionValue)item).Value;
                }
            }
            return array;
        }

        /// <summary>
        /// Gets the actual invoke instance.
        /// </summary>
        /// <param name="instance">The instance to coerce.</param>
        /// <returns>The actual invoke instance.</returns>
        private static object CoerceInstance(object instance)
        {
            if (instance is ExpressionValue)
            {
                instance = ((ExpressionValue)instance).Value;
            }

            if (instance == null)
            {
                throw new NullReferenceException("Cannot get property value from null object");
            }

            return instance;
        }

    }
}
