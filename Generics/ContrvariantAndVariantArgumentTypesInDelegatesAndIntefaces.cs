using System;
using System.Collections.Generic;

namespace Generics
{
    //public interface IEnumerator<out T> : IEnumerator
    //{
    //    Boolean MoveNext();
    //    T Current { get; }
    //}

    class ContrvariantAndVariantArgumentTypesInDelegatesAndInterfaces
    {
        public delegate TResult Func<in T, out TResult>(T arg);

        Func<Object, ArgumentException> fn1 = null;
        //Func<Int32, ArgumentException> fn1 = null; // aleek

        public ContrvariantAndVariantArgumentTypesInDelegatesAndInterfaces()
        {
            Func<String, Exception> fn2 = fn1; // Явного приведения типа не требуется

            //Exception e = fn2("");
            //String e = fn2(""); // aleek

            // Этот вызов передает IEnumerable<String> в Count
            Int32 c = Count(new[] { "Grant" });
        }

        // Этот метод допускает интерфейс IEnumerable любого ссылочного типа
        Int32 Count(IEnumerable<Object> collection)
        { 
            return 0; 
        }
    }
}
