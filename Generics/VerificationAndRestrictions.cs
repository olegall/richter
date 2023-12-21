using System;

namespace Generics
{
    // Можно определить следующие типы:
    internal sealed class AType { }
    internal sealed class AType<T> { }
    internal sealed class AType<T1, T2> { }

    // Ошибка: конфликт с типом AType<T>, у которого нет ограничений.
    //internal sealed class AType<T> where T : IComparable<T> { }

    // Ошибка: конфликт с типом AType<T1, T2>
    //internal sealed class AType<T3, T4> { }

    internal sealed class AnotherType
    {
        // Можно определить следующие методы:
        private static void M() { }
        private static void M<T>() { }
        private static void M<T1, T2>() { }
        
        // Ошибка: конфликт с типом M<T>, у которого нет ограничений
        //private static void M<T>() where T : IComparable<T> { }
        
        // Ошибка: конфликт с типом M<T1, T2>.
        //private static void M<T3, T4>() { }
    }

    class Foo { }

    class Base
    {
        public virtual void M<T1, T2>() where T1 : struct
                                        where T2 : class 
        {
        }
    }

    class Derived : Base
    {
        public override void M<T3, T4>() // ошибки c where
        //public void M<T3, T4>()
            //where T3 : EventArgs
            //where T3 : Foo
            //where T3 : struct
            //where T4 : class
        { 
        }
    }

    internal class BaseOneWhere
    {
        public virtual void M<T1>() where T1 : class
        {
        }
    }

    internal sealed class DerivedOneWhere : BaseOneWhere
    {
        //public override void M<T1>() where T1 : class
        //public override void M<T2>() where T2 : class
        public override void M<T1>()
        {
        }
    }

    class OverrideTest : Base
    {
        public override void M<T1, T2>() { }
        //public void M<T1, T2>() {} // ошибка
    }

    /// <summary>
    /// Верификация и ограничения
    /// </summary>
    class VerificationAndRestrictions
    {
        public VerificationAndRestrictions()
        {
            CallMin();
        }

        private static Boolean MethodTakingAnyType<T>(T o)
        {
            T temp = o;
            Boolean b = temp.Equals(o);
            return b;
        }
        
        private void TypeEqualityTest<T>(T o)
        {
            T temp = o;

            //T t1, t2 = null; // ошибка
            T t1, t2 = default(T);
            t1 = t2;
        }

        private static T Min<T>(T o1, T o2)
        {
            //if (o1.CompareTo(o2) < 0) { return o1; }
            return o2;
        }

        //public static T Min<T>(T o1, T o2) where T : IComparable<T>
        ////public static T Min<T>(T o1, T o2) // ошибка
        //{
        //    if (o1.CompareTo(o2) < 0)
        //    {
        //        return o1; 
        //    }
        //    return o2;
        //}

        private static void CallMin()
        {
            Object o1 = "Jeff", o2 = "Richter";
            Object oMin = Min<Object>(o1, o2); // Ошибка CS0311
            //Object oMinError = Min<string>(o1, o2); // ошибка
            Object oMin2 = Min<string>((string)o1, (string)o2);
            Object oMin3 = Min<object>((string)o1, (string)o2);
            Object oMin4 = Min((IComparable<object>)o1, (IComparable<object>)o2);
        }
    }
}