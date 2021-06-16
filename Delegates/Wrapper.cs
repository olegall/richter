using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Delegates
{
    /// <summary>
    /// Куски кода
    /// </summary>
    class Wrapper
    {
        public delegate Int32 Feedback(Int32 value);

        object _invocationList = null;
        Delegate _methodPtr = null;
        object _target = null;

        Int32 Invoke(Int32 value)
        {
            Int32 result = 0;
            Delegate[] delegateSet = _invocationList as Delegate[];
            if (delegateSet != null)
            {
                // Массив указывает на делегаты, которые нужно вызвать
                foreach (Feedback d in delegateSet)
                {
                    result = d(value); // Вызов делегата
                }
            }
            else
            {
                // Этот делегат определяет используемый метод обратного вызова.
                // Этот метод вызывается для указанного объекта.
                //result = _methodPtr.Invoke(_target, value);
                // Строка выше — имитация реального кода.
                // То, что происходит в действительности, не выражается средствами C#.
            }
            return result;
        }

        internal sealed class AClass
        {
            public static void CallbackWithoutNewingADelegateObject()
            {
                ThreadPool.QueueUserWorkItem(SomeAsyncTask, 5);
            }
            private static void SomeAsyncTask(Object o)
            {
                Console.WriteLine(o);
            }
        }

        internal sealed class AClass2
        {
            public static void CallbackWithoutNewingADelegateObject()
            {
                ThreadPool.QueueUserWorkItem(obj => Console.WriteLine(obj), 5);
            }
        }

        //internal sealed class AClass3
        //{
        //    // Это закрытое поле создано для кэширования делегата
        //    // Преимущество: CallbackWithoutNewingADelegateObject не будет
        //    // создавать новый объект при каждом вызове
        //    // Недостатки: кэшированные объекты недоступны для сборщика мусора
        //    [CompilerGenerated]
        //    private static WaitCallback<>9__CachedAnonymousMethodDelegate1;
        //    public static void CallbackWithoutNewingADelegateObject()
        //    {
        //        if (<> 9__CachedAnonymousMethodDelegate1 == null) {
        //             // При первом вызове делегат создается и кэшируется
        //             <> 9__CachedAnonymousMethodDelegate1 =
        //              new WaitCallback(< CallbackWithoutNewingADelegateObject > b__0);
        //        }
        //        ThreadPool.QueueUserWorkItem(<> 9__CachedAnonymousMethodDelegate1, 5);
        //    }

        //    [CompilerGenerated]
        //    private static void <CallbackWithoutNewingADelegateObject>b__0(Object obj)
        //    {
        //        Console.WriteLine(obj);
        //    }
        //}

        internal sealed class AClass4
        {
            private static String sm_name; // Статическое поле
            public static void CallbackWithoutNewingADelegateObject()
            {
                ThreadPool.QueueUserWorkItem(
                // Код обратного вызова может обращаться к статическим членам
                obj => Console.WriteLine(sm_name + ": " + obj),
                5);
            }
        }

        internal sealed class AClass5
        {
            private String m_name; // Поле экземпляра
                                   // Метод экземпляра
            public void CallbackWithoutNewingADelegateObject()
            {
                ThreadPool.QueueUserWorkItem(
                // Код обратного вызова может ссылаться на члены экземпляра
                obj => Console.WriteLine(m_name + ": " + obj),
                5);
            }
        }

        Func<Int32, Int32, String> f7 = (n1, n2) =>
        {
            Int32 sum = n1 + n2; 
            return sum.ToString();
        };

        internal sealed class AClass6
        {
            public static void UsingLocalVariablesInTheCallbackCode(Int32 numToDo)
            {
                // Локальные переменные
                Int32[] squares = new Int32[numToDo];
                AutoResetEvent done = new AutoResetEvent(false);
                // Выполнение задач в других потоках
                for (Int32 n = 0; n < squares.Length; n++)
                {
                    ThreadPool.QueueUserWorkItem(
                    obj =>
                    {
                        Int32 num = (Int32)obj;
                        // Обычно решение этой задачи требует больше времени
                        squares[num] = num * num;
                        // Если это последняя задача, продолжаем выполнять главный поток
                        if (Interlocked.Decrement(ref numToDo) == 0)
                            done.Set();
                    },
                    n);
                }
                // Ожидаем завершения остальных потоков
                done.WaitOne();
                // Вывод результатов
                for (Int32 n = 0; n < squares.Length; n++)
                    Console.WriteLine("Index {0}, Square={1}", n, squares[n]);
            }
        }

        /*internal sealed class AClass7
        {
            public static void UsingLocalVariablesInTheCallbackCode(Int32 numToDo)
            {
                // Локальные переменные
                WaitCallback callback1 = null;
                // Создание экземпляра вспомогательного класса
                <> c__DisplayClass2 class1 = new <> c__DisplayClass2();
                // Инициализация полей вспомогательного класса
                class1.numToDo = numToDo;
                class1.squares = new Int32[class1.numToDo];
                class1.done = new AutoResetEvent(false);
                // Выполнение задач в других потоках
                for (Int32 n = 0; n < class1.squares.Length; n++)
                {
                    if (callback1 == null)
                    {
                        // Новый делегат привязывается к объекту вспомогательного класса
                        // и его анонимному экземплярному методу
                        callback1 = new WaitCallback(
                        class1.< UsingLocalVariablesInTheCallbackCode > b__0);
                    }
                    ThreadPool.QueueUserWorkItem(callback1, n);
                }
                
                // Ожидание завершения остальных потоков
                class1.done.WaitOne();
                // Вывод результатов
                for (Int32 n = 0; n < class1.squares.Length; n++)
                    Console.WriteLine("Index {0}, Square={1}", n, class1.squares[n]);
            }
        }*/

        // Вспомогательному классу присваивается необычное имя, чтобы
        // избежать конфликтов и предотвратить доступ из класса AClass
        /*[CompilerGenerated]
        private sealed class <>c__DisplayClass2 : Object {
         // В коде обратного вызова для каждой локальной переменной
         // используется одно открытое поле
         public Int32[] squares;
                public Int32 numToDo;
                public AutoResetEvent done;
                // Открытый конструктор без параметров
                public <>c__DisplayClass2 { }
            // Открытый экземплярный метод с кодом обратного вызова
            public void <UsingLocalVariablesInTheCallbackCode>b__0(Object obj)
            {
                Int32 num = (Int32)obj;
                squares[num] = num * num;
                if (Interlocked.Decrement(ref numToDo) == 0)
                    done.Set();
            }
        }*/

        
    }
}
