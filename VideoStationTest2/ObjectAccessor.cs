using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VideoStationTest2
{
    /// <summary>
    /// Gives a solution to access / inject new values to a class non-public fields and properties.
    /// For test purpose.
    /// </summary>
    public static class ObjectAccessor
    {
        #region Field and property manipulation

        /// <summary>
        /// Provides access to private or protected property of a class and modify its value.
        /// This class is intended to be used with tests.
        /// </summary>
        /// <param name="baseObject"> The object whose field value will be set.</param>
        /// <param name="propertyName"> The name of the property whose value will be set.</param>
        /// <param name="newValue"> The new value to assign to the property.</param>
        [DebuggerStepThrough]
        public static void SetProperty(object baseObject, string propertyName, object newValue)
        {
            if (baseObject == null) throw new ArgumentNullException(nameof(baseObject));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException(nameof(propertyName));

            var baseType = baseObject.GetType();
            var propertyToModify = baseType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (propertyToModify == null) throw new ArgumentException(nameof(propertyName));

            try
            {
                propertyToModify.SetValue(baseObject, newValue, null);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        [DebuggerStepThrough]
        public static object GetProperty(object baseObject, string propertyName)
        {
            if (baseObject == null) throw new ArgumentNullException(nameof(baseObject));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var baseType = baseObject.GetType();
            var propertyToModify = baseType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (propertyToModify == null)
            {
                throw new ArgumentException($"Property with the name {propertyName} does not exist in the baseObject", propertyName);
            }

            return propertyToModify.GetValue(baseObject);
        }

        /// <summary>
        /// Provides access to a field of a class and modify its value.
        /// This class is intended to be used with tests.
        /// </summary>
        /// <param name="baseObject"> The object whose field value will be set.</param>
        /// <param name="fieldName"> The name of the property whose value will be set.</param>
        /// <param name="newValue"> The new value to assign to the field.</param>
        [DebuggerStepThrough]
        public static void SetField(object baseObject, string fieldName, object newValue)
        {
            if (baseObject == null) throw new ArgumentNullException(nameof(baseObject));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException(nameof(fieldName));

            var baseType = baseObject.GetType();
            var fieldToModify = baseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .SingleOrDefault(f => f.Name.Equals(fieldName));

            if (fieldToModify == null)
            {
                throw new ArgumentException($"Field with the name {fieldName} does not exist in the baseObject", fieldName);
            }

            fieldToModify.SetValue(baseObject, newValue);
        }

        [DebuggerStepThrough]
        public static object GetField(object baseObject, string fieldName)
        {
            if (baseObject == null) throw new ArgumentNullException(nameof(baseObject));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException(nameof(fieldName));

            var baseType = baseObject.GetType();
            var fieldToModify = baseType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .SingleOrDefault(f => f.Name.Equals(fieldName));

            if (fieldToModify == null)
            {
                throw new ArgumentException($"Field with the name {fieldName} does not exist in the baseObject", fieldName);
            }

            return fieldToModify.GetValue(baseObject);
        }

        #endregion

        #region Method manipulation

        [DebuggerStepThrough]
        public static object CallMethod(object baseObject, string methodName, object[] parameters = null)
        {
            if (baseObject == null) throw new ArgumentNullException(nameof(baseObject));
            if (string.IsNullOrEmpty(methodName)) throw new ArgumentException(nameof(methodName));

            var baseType = baseObject.GetType();
            var parameterTypes = (parameters == null) ? new Type[] { } : parameters.Select(p => p.GetType()).ToArray();
            var method = baseType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static, null, parameterTypes, null);
            if (method == null) throw new ArgumentException(nameof(methodName));

            try
            {
                var objRet = method.Invoke(baseObject, parameters);
                return objRet;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        [DebuggerStepThrough]
        public static T CallMethod<T>(object baseObject, string methodName, object[] parameters = null, bool useMethodNameOnly = false)
        {
            if (baseObject == null) throw new ArgumentNullException(nameof(baseObject));
            if (string.IsNullOrEmpty(methodName)) throw new ArgumentException(nameof(methodName));

            var baseType = baseObject.GetType();

            var methodInfo = useMethodNameOnly
                ? baseType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
                : baseType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static, null, (parameters == null) ? new Type[] { } : parameters.Select(p => p.GetType()).ToArray(), null);

            if (methodInfo == null) throw new ArgumentException(nameof(methodName));

            try
            {
                var objRet = methodInfo.Invoke(baseObject, parameters);
                return (T)objRet;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        #endregion

        #region Object creation

        [DebuggerStepThrough]
        public static T CreateInstance<T>(params object[] parameters) where T : class
        {
            var objectType = typeof(T);
            var typeList = new Type[] { };

            if (parameters.Length > 0)
            {
                typeList = parameters
                    .Select(p => p.GetType())
                    .ToArray();
            }

            var constructorInfo = objectType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, typeList, null);
            if (constructorInfo == null) throw new ArgumentException("constructor");

            try
            {
                var instance = constructorInfo.Invoke(parameters);
                return instance as T;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets the custom attributes of either a property or a method.
        /// </summary>
        /// <typeparam name="TBase"> The type of the declaring class</typeparam>
        /// <typeparam name="TAttr"> The type of the attribute to get</typeparam>
        /// <param name="call"></param>
        /// <returns></returns>
        public static TAttr GetAttribute<TBase, TAttr>(Expression<Func<TBase, string>> call)
        {
            if (call.Body is MemberExpression expression)
            {
                var attributes = expression.Member.GetCustomAttributes(false);
                return (TAttr)attributes.FirstOrDefault(a => a.GetType() == typeof(TAttr));
            }

            throw new NotSupportedException("This type of expression call (" + call.Body.Type + ") is not supported");
        }

        #endregion
    }
}