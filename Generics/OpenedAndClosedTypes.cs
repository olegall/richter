using System;
using System.Collections.Generic;

namespace Generics
{
    // Частично определенный открытый тип
    internal sealed class DictionaryStringKey<TValue> : Dictionary<String, TValue> { }
    internal sealed class DictionaryStringKey<TKey, TValue> : Dictionary<String, TValue> { } // перегрузка по TKey
    //internal sealed class DictionaryStringKey : Dictionary<String, TValue> { }
    //internal sealed class DictionaryStringKey<> {}
    //internal sealed class DictionaryStringKey<TValue> {}
    internal sealed class DictionaryStringKey<T1, T2, T3, TValue> : Dictionary<String, TValue> { } // д.б. как минимум TValue

    /// <summary>
    /// Открытые и закрытые типы
    /// </summary>
    public class OpenedAndClosedTypes
    {
        public OpenedAndClosedTypes()
        {
            Object o = null;

            // Dictionary<,> — открытый тип с двумя параметрами типа
            Type t = typeof(Dictionary<,>);

            // Попытка создания экземпляра этого типа (неудачная)
            o = CreateInstance(t);

            // DictionaryStringKey<> — открытый тип с одним параметром типа
            t = typeof(DictionaryStringKey<>);

            // Попытка создания экземпляра этого типа (неудачная)
            o = CreateInstance(t);

            // DictionaryStringKey<Guid> — это закрытый тип
            t = typeof(DictionaryStringKey<Guid>);
            
            // Попытка создания экземпляра этого типа (удачная). почему? типизирован? aleek
            o = CreateInstance(t);

            // Проверка успешности попытки
            Console.WriteLine("Object type=" + o.GetType());
        }

        private static Object CreateInstance(Type t)
        {
            Object o = null;
            try
            {
                o = Activator.CreateInstance(t);
                Console.Write("Created instance of {0}", t.ToString());
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            return o;
        }
    }
}
