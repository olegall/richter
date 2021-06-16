using System;

namespace Generics
{
    /// <summary>
    /// Другие проблемы верификации
    /// </summary>
    class AnotherVerificationProblems
    {
        private static void CastingAGenericTypeVariable1<T>(T obj)
        {
            Int32 x = (Int32)obj; // Ошибка
            String s = (String)obj; // Ошибка
        }

        private static void CastingAGenericTypeVariable2<T>(T obj)
        {
            Int32 x = (Int32)(Object)obj; // Ошибки нет
            String s = (String)(Object)obj; // Ошибки нет
        }

        private static void CastingAGenericTypeVariable3<T>(T obj)
        {
            String s = obj as String; // Ошибки нет
        }

        private static void SettingAGenericTypeVariableToNull<T>()
        {
            T temp = null; // CS0403: нельзя привести null к параметру типа T
                           // because it could be a value type...
                           // (Ошибка CS0403: нельзя привести null к параметру типа Т,
                           // поскольку T может иметь значимый тип...)
        }

        private static void SettingAGenericTypeVariableToDefaultValue<T>()
        {
            T temp = default(T); // Работает
        }

        private static void ComparingAGenericTypeVariableWithNull<T>(T obj)
        {
            if (obj == null)
            { 
                /* Этот код никогда не исполняется для значимого типа */ 
            }
        }

        private static void ComparingTwoGenericTypeVariables<T>(T o1, T o2)
        {
            if (o1 == o2) // Ошибка
            {
            } 
        }

        private static T Sum<T>(T num) where T : struct
        {
            T sum = default(T);
            for (T n = default(T); n < num; n++)
            {
                sum += n;
            }
            return sum;
        }
    }
}
