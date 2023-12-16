using System;

namespace Generics
{
    /// <summary>
    /// Ограничения конструктора
    /// </summary>
    internal sealed class ConstructorConstraint<T> where T : new()
    {
        public static T Factory()
        {
            // Допустимо, потому что у всех значимых типов неявно есть открытый конструктор без параметров, и потому что
            // это ограничение требует, чтобы у всех указанных ссылочных типов также был открытый конструктор без параметров
            return new T();
        }

        public static void Foo()
        {
        }

        public ConstructorConstraint() {} // aleek
    }
    
    internal sealed class ConstructorConstraint // перегрузка по дженерику aleek
    {
        public static Int32 Factory2()
        {
            return new Int32();
        }
    }
}