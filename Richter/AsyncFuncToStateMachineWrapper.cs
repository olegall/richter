using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Richter
{
    // Преобразование асинхронной функции в конечный автомат
    class AsyncFuncToStateMachineWrapper
    {
        internal sealed class Type1 { }
        internal sealed class Type2 { }

        private static async Task<Type1> Method1Async()
        {
            // Асинхронная операция, возвращающая объект Type1
            return null;
        }

        private static async Task<Type2> Method2Async()
        {
            // Асинхронная операция, возвращающая объект Type2
            return null;
        }

        // Структура конечного автомата
        [CompilerGenerated, StructLayout(LayoutKind.Auto)]
        struct StateMachine : IAsyncStateMachine
        {
            // Поля для построителя конечного автомата (Task) и его местонахождения
            public AsyncTaskMethodBuilder<String> m_builder;

            public Int32 m_state;

            // Аргумент и локальные переменные становятся полями:
            public Int32 m_argument, m_local, m_x;

            public Type1 m_resultType1;
            
            public Type2 m_resultType2;

            // Одно поле на каждый тип Awaiter.
            // В любой момент времени важно только одно из этих полей. 
            // В нем хранится ссылка на последний выполненный экземпляр await, который завершается асинхронно:
            private TaskAwaiter<Type1> m_awaiterType1;
            
            private TaskAwaiter<Type2> m_awaiterType2;

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                throw new NotImplementedException();
            }

            void MoveNext() { } // родной метод этого класса. конфликта с интерфейсным нет - почему?

            // Сам конечный автомат
            void IAsyncStateMachine.MoveNext() // интерфейсный метод
            {
                String result = null; // Результат Task. Вставленный компилятором блок try гарантирует завершение задания конечного автомата
                try
                {
                    Boolean executeFinally = true; // Логический выход из блока 'try'
                    if (m_state == 0)
                    {
                        // Если метод конечного автомата выполняется впервые
                        m_local = m_argument; // Выполнить начало исходного метода
                    }
                    // Блок try из исходного кода
                    try
                    {
                        TaskAwaiter<Type1> awaiterType1;
                        TaskAwaiter<Type2> awaiterType2;
                        switch (m_state)
                        {
                            case 1:
                                // Начало исполнения кода в 'try'. вызвать Method1Async и получить его объект ожидания
                                awaiterType1 = Method1Async().GetAwaiter();
                                if (!awaiterType1.IsCompleted)
                                {
                                    m_state = 0; // 'Method1Async' завершается асинхронно
                                    m_awaiterType1 = awaiterType1; // Сохранить объект ожидания до возвращения
                                                                   // Приказать объекту ожидания вызвать MoveNext после завершения операции
                                    m_builder.AwaitUnsafeOnCompleted(ref awaiterType1, ref this);
                                    // Предыдущая строка вызывает метод OnCompleted объекта awaiterType1, что приводит к вызову
                                    // ContinueWith(t => MoveNext()) для Task. При завершении Task ContinueWith вызывает MoveNext
                                    executeFinally = false; // Без логического выхода из блока 'try'
                                    return; // Поток возвращает управление вызывающей стороне 'Method1Async' завершается синхронно
                                }

                                break;
                            case 0: // 'Method1Async' завершается асинхронно
                                awaiterType1 = m_awaiterType1; // Восстановление последнего объекта ожидания
                                break;
                            // case 1:
                            case 11: // 'Method2Async' завершается асинхронно
                                awaiterType2 = m_awaiterType2; // Восстановление последнего объекта ожидания
                                goto ForLoopEpilog;
                        }
                        // После первого await сохраняем результат и запускаем цикл 'for'

                        m_resultType1 = awaiterType1.GetResult(); // Получение результата
                    ForLoopPrologue:
                        m_x = 0; // Инициализация цикла 'for'
                        goto ForLoopBody; // Переход к телу цикла 'for'
                    ForLoopEpilog:
                        m_resultType2 = awaiterType2.GetResult();
                        m_x++; // Увеличение x после каждой итерации. Переход к телу цикла 'for'
                    ForLoopBody:
                        if (m_x < 3)
                        { // Условие цикла 'for'. Вызов Method2Async и получение объекта ожидания
                            awaiterType2 = Method2Async().GetAwaiter();
                            if (!awaiterType2.IsCompleted)
                            {
                                m_state = 1; // 'Method2Async' завершается асинхронно
                                m_awaiterType2 = awaiterType2; // Сохранение объекта ожидания до возвращения
                                                               // Приказываем вызвать MoveNext при завершении операции
                                m_builder.AwaitUnsafeOnCompleted(ref awaiterType2, ref this);
                                executeFinally = false; // Без логического выхода из блока 'try'
                                return; // Поток возвращает управление вызывающей стороне. 'Method2Async' завершается синхронно
                            }
                            goto ForLoopEpilog; // Синхронное завершение, возврат
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Catch");
                    }
                    finally
                    {
                        // Каждый раз, когда блок физически выходит из 'try', выполняется 'finally'.
                        // Этот код должен выполняться только при логическом выходе из 'try'.
                        if (executeFinally)
                        {
                            Console.WriteLine("Finally");
                        }
                    }
                    result = "Done"; // То, что в конечном итоге должна вернуть асинхронная функция.
                }
                catch (Exception exception) // когда сработает catch?
                {
                    // Необработанное исключение: задание конечного автомата завершается с исключением.
                    m_builder.SetException(exception);
                    return;
                }
                // Исключения нет: задание конечного автомата завершается с результатом

                m_builder.SetResult(result);
            }
        }
    }
}
