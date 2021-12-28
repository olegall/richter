using System;
using System.Reflection;

namespace Reflection
{
    internal static class ReflectionExtensions
    {
        // Метод расширения, упрощающий синтаксис создания делегата
        public static TDelegate CreateDelegate<TDelegate>(this MethodInfo mi, Object target = null)
        {
            return (TDelegate)(Object)mi.CreateDelegate(typeof(TDelegate), target);
        }
    }
}
