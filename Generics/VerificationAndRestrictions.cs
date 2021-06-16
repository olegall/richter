﻿using System;

namespace Generics
{
    // Можно определить следующие типы:
    internal sealed class AType { }
    internal sealed class AType<T> { }
    internal sealed class AType<T1, T2> { }
    // Ошибка: конфликт с типом AType<T>, у которого нет ограничений.
    internal sealed class AType<T> where T : IComparable<T> { }
    // Ошибка: конфликт с типом AType<T1, T2>
    internal sealed class AType<T3, T4> { }
    internal sealed class AnotherType
    {
        // Можно определить следующие методы:
        private static void M() { }
        private static void M<T>() { }
        private static void M<T1, T2>() { }
        // Ошибка: конфликт с типом M<T>, у которого нет ограничений
        private static void M<T>() where T : IComparable<T> { }
        // Ошибка: конфликт с типом M<T1, T2>.
        private static void M<T3, T4>() { }
    }

    internal class Base
    {
        public virtual void M<T1, T2>()
        where T1 : struct
        where T2 : class
        {
        }
    }
    internal sealed class Derived : Base // если убрать 2 строки where - скомпилируется
    {
        public override void M<T3, T4>()
        where T3 : EventArgs // Ошибка
        where T4 : class // Ошибка
        { }
    }

    /// <summary>
    /// Верификация и ограничения
    /// </summary>
    class VerificationAndRestrictions
    {
        private static Boolean MethodTakingAnyType<T>(T o)
        {
            T temp = o;
            Console.WriteLine(o.ToString());
            Boolean b = temp.Equals(o);
            return b;
        }

        //private static T Min<T>(T o1, T o2)
        //{
        //    if (o1.CompareTo(o2) < 0) return o1;
        //    return o2;
        //}

        public static T Min<T>(T o1, T o2) where T : IComparable<T>
        {
            if (o1.CompareTo(o2) < 0) return o1;
            return o2;
        }

        private static void CallMin()
        {
            Object o1 = "Jeff", o2 = "Richter";
            Object oMin = Min<Object>(o1, o2); // Ошибка CS0311
        }
    }
}
