using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using JetBrains.Annotations;

namespace EasyToolKit.Core
{
    public static class ReflectionExtensions
    {
        public static bool IsDefined<T>(this MemberInfo member, bool inherit = false) where T : Attribute
        {
            return member.IsDefined(typeof(T), inherit);
        }

        public static bool IsDefined<T>(this Type type, bool inherit = false, bool includeInterface = false)
            where T : Attribute
        {
            if (type.IsDefined(typeof(T), inherit))
                return true;

            if (!includeInterface)
                return false;

            var queue = new Queue<Type>(type.GetInterfaces());

            while (queue.Count > 0)
            {
                var iface = queue.Dequeue();

                if (iface.IsDefined(typeof(T)))
                    return true;

                foreach (var sub in iface.GetInterfaces())
                    queue.Enqueue(sub);
            }

            return false;
        }


        public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes());
        }

        public static Type FindType(this IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            return assemblies.GetAllTypes().FirstOrDefault(predicate);
        }

        public static Type FindTypeByName(this IEnumerable<Assembly> assemblies, string fullName)
        {
            return assemblies.GetAllTypes().FirstOrDefault(t => t.FullName == fullName);
        }

        public static string GetSignature(this MemberInfo member)
        {
            var sb = new StringBuilder();

            // Append the member type (e.g., Method, Property, Field, etc.)
            sb.Append(member.MemberType.ToString());
            sb.Append(" ");

            // Append the member's declaring type (including namespace)
            sb.Append(member.DeclaringType.FullName);
            sb.Append(".");

            // Append the member name
            sb.Append(member.Name);

            // If the member is a method, append parameter types and return type
            if (member is MethodInfo methodInfo)
            {
                sb.Append($"({GetMethodParametersSignature(methodInfo)}) : ");
                sb.Append(methodInfo.ReturnType.FullName);
            }
            else if (member is PropertyInfo propertyInfo)
            {
                // If the member is a property, append the property type
                sb.Append(" : ");
                sb.Append(propertyInfo.PropertyType.FullName);
            }
            else if (member is FieldInfo fieldInfo)
            {
                // If the member is a field, append the field type
                sb.Append(" : ");
                sb.Append(fieldInfo.FieldType.FullName);
            }
            else if (member is EventInfo eventInfo)
            {
                // If the member is an event, append the event handler type
                sb.Append(" : ");
                sb.Append(eventInfo.EventHandlerType.FullName);
            }

            return sb.ToString();
        }


        public static string GetMethodParametersSignature(this MethodInfo method)
        {
            return string.Join(", ",
                method.GetParameters().Select(x => $"{TypeExtensions.GetAliases(x.ParameterType)} {x.Name}"));
        }

        public static Type GetMemberType([NotNull] this MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (member is FieldInfo field)
            {
                return field.FieldType;
            }

            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            if (member is MethodInfo method)
            {
                return method.ReturnType;
            }

            throw new NotSupportedException();
        }

        // public static Func<object> GetStaticMemberValueGetter([NotNull] this MemberInfo memberInfo)
        // {
        //     if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
        //     if (memberInfo is FieldInfo fieldInfo)
        //     {
        //         return EmitUtilities.CreateWeakStaticFieldGetter(fieldInfo);
        //     }
        //
        //     if (memberInfo is PropertyInfo propertyInfo)
        //     {
        //         var getMethodInfo = propertyInfo.GetGetMethod(true) ?? throw new ArgumentException($"Property '{memberInfo.DeclaringType}.{memberInfo.Name}' has no getter");
        //         return (Func<object>)getMethodInfo.CreateDelegate(typeof(Func<object>));
        //     }
        //
        //     if (memberInfo is MethodInfo methodInfo)
        //     {
        //         return (Func<object>)methodInfo.CreateDelegate(typeof(Func<object>));
        //     }
        //
        //     throw new NotSupportedException($"Can't get static member value getter for '{memberInfo.DeclaringType}.{memberInfo.Name}'");
        // }
        //
        // public static Action<object> GetStaticMemberValueSetter([NotNull] this MemberInfo memberInfo)
        // {
        //     if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
        //     if (memberInfo is FieldInfo fieldInfo)
        //     {
        //         return EmitUtilities.CreateWeakStaticFieldSetter(fieldInfo);
        //     }
        //
        //     if (memberInfo is PropertyInfo propertyInfo)
        //     {
        //         var setMethodInfo = propertyInfo.GetSetMethod(true) ?? throw new ArgumentException($"Property '{memberInfo.DeclaringType}.{memberInfo.Name}' has no setter");
        //         return (Action<object>)setMethodInfo.CreateDelegate(typeof(Action<object>));
        //     }
        //
        //     throw new NotSupportedException($"Can't get static member value setter for '{memberInfo.DeclaringType}.{memberInfo.Name}'");
        // }
        //
        // public static WeakValueGetter GetInstanceMemberValueGetter([NotNull] this MemberInfo memberInfo, [NotNull] Type instanceType)
        // {
        //     if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
        //     if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
        //     if (memberInfo is FieldInfo fieldInfo)
        //     {
        //         return EmitUtilities.CreateWeakInstanceFieldGetter(instanceType, fieldInfo);
        //     }
        //
        //     if (memberInfo is PropertyInfo propertyInfo)
        //     {
        //         return EmitUtilities.CreateWeakInstancePropertyGetter(instanceType, propertyInfo);
        //     }
        //
        //     if (memberInfo is MethodInfo methodInfo)
        //     {
        //         var getter = (Func<object, object>)methodInfo.CreateDelegate(typeof(Func<object, object>));
        //         return (ref object instance) => getter(instance);
        //     }
        //
        //     throw new NotSupportedException($"Can't get instance member value getter for '{memberInfo.DeclaringType}.{memberInfo.Name}'");
        // }
        //
        // public static WeakValueSetter GetInstanceMemberValueSetter([NotNull] this MemberInfo memberInfo, [NotNull] Type instanceType)
        // {
        //     if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
        //     if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
        //     if (memberInfo is FieldInfo fieldInfo)
        //     {
        //         return EmitUtilities.CreateWeakInstanceFieldSetter(instanceType, fieldInfo);
        //     }
        //
        //     if (memberInfo is PropertyInfo propertyInfo)
        //     {
        //         return EmitUtilities.CreateWeakInstancePropertySetter(instanceType, propertyInfo);
        //     }
        //
        //     throw new NotSupportedException($"Can't get instance member value setter for '{memberInfo.DeclaringType}.{memberInfo.Name}'");
        // }

        public static Func<T> GetStaticValueGetter<T>([NotNull] this FieldInfo fieldInfo)
        {
            return EmitUtilities.CreateStaticFieldGetter<T>(fieldInfo);
        }

        public static Func<T> GetStaticValueGetter<T>([NotNull] this PropertyInfo propertyInfo)
        {
            return EmitUtilities.CreateStaticPropertyGetter<T>(propertyInfo);
        }

        public static Action<T> GetStaticValueSetter<T>([NotNull] this FieldInfo fieldInfo)
        {
            return EmitUtilities.CreateStaticFieldSetter<T>(fieldInfo);
        }

        public static Action<T> GetStaticValueSetter<T>([NotNull] this PropertyInfo propertyInfo)
        {
            return EmitUtilities.CreateStaticPropertySetter<T>(propertyInfo);
        }

        public static ValueGetter<TInstance, TValue> GetInstanceValueGetter<TInstance, TValue>(
            [NotNull] this FieldInfo fieldInfo)
        {
            return EmitUtilities.CreateInstanceFieldGetter<TInstance, TValue>(fieldInfo);
        }

        public static ValueGetter<TInstance, TValue> GetInstanceValueGetter<TInstance, TValue>(
            [NotNull] this PropertyInfo propertyInfo)
        {
            return EmitUtilities.CreateInstancePropertyGetter<TInstance, TValue>(propertyInfo);
        }

        public static ValueSetter<TInstance, TValue> GetInstanceValueSetter<TInstance, TValue>(
            [NotNull] this FieldInfo fieldInfo)
        {
            return EmitUtilities.CreateInstanceFieldSetter<TInstance, TValue>(fieldInfo);
        }

        public static ValueSetter<TInstance, TValue> GetInstanceValueSetter<TInstance, TValue>(
            [NotNull] this PropertyInfo propertyInfo)
        {
            return EmitUtilities.CreateInstancePropertySetter<TInstance, TValue>(propertyInfo);
        }

        public static Action GetStaticMethodCaller([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            return EmitUtilities.CreateStaticMethodCaller(methodInfo);
        }

        public static Action<object> GetInstanceMethodCaller([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            return EmitUtilities.CreateWeakInstanceMethodCaller(methodInfo);
        }

        public static Action<T> GetInstanceMethodCaller<T>([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            return EmitUtilities.CreateInstanceMethodCaller<T>(methodInfo);
        }

        public static Action<T, TArg1> GetInstanceMethodCaller<T, TArg1>([NotNull] this MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            return EmitUtilities.CreateInstanceMethodCaller<T, TArg1>(methodInfo);
        }
    }
}
