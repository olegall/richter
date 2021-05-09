using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Richter
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Threading
            var threading = new Threading();
            // Потоки для асинхронных вычислительных операций
            Console.WriteLine("Main thread: starting a dedicated thread " + "to do an asynchronous operation");
            Thread dedicatedThread = new Thread(ComputeBoundOp);
            dedicatedThread.Start(5);
            Console.WriteLine("Main thread: Doing other work here...");
            Thread.Sleep(10000); // Имитация другой работы (10 секунд)
            dedicatedThread.Join(); // Ожидание завершения потока
            Console.WriteLine("Hit <Enter> to end this program...");
            /*
                Результат компиляции и запуска такого кода:
                Main thread: starting a dedicated thread to do an asynchronous operation
                Main thread: Doing other work here...
                In ComputeBoundOp: state=5

                Так как мы не можем контролировать очередность исполнения потоков
                в Windows, возможен и другой результат:
                Main thread: starting a dedicated thread to do an asynchronous operation
                In ComputeBoundOp: state=5
                Main thread: Doing other work here...
            */

            // Фоновые и активные потоки
            // Создание нового потока (по умолчанию активного)
            Thread t = new Thread(Worker);
            // Превращение потока в фоновый
            t.IsBackground = true;
            t.Start(); // Старт потока
                       // В случае активного потока приложение будет работать около 10 секунд
                       // В случае фонового потока приложение немедленно прекратит работу
            Console.WriteLine("Returning from Main");

            // Простые вычислительные операции
            Console.WriteLine("Main thread: queuing an asynchronous operation");
            ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5);
            Console.WriteLine("Main thread: Doing other work here...");
            Thread.Sleep(10000); // Имитация другой работы (10 секунд)
            Console.WriteLine("Hit <Enter> to end this program...");
            /*
                Результат компиляции и запуска этого кода:
                Main thread: queuing an asynchronous operation
                Main thread: Doing other work here...
                In ComputeBoundOp: state=5
                
                Впрочем, возможен и такой результат:
                Main thread: queuing an asynchronous operation
                In ComputeBoundOp: state=5
                Main thread: Doing other work here...       
            
                Разный порядок следования строк в данном случае объясняется асинхронным
                выполнением методов. Планировщик Windows решает, какой поток должен выполняться первым, или же планирует их для одновременного выполнения на
                многопроцессорном компьютере
            */

            // Контексты исполнения
            // Помещаем данные в контекст логического вызова потока метода Main
            CallContext.LogicalSetData("Name", "Jeffrey");
            // Заставляем поток из пула работать
            // Поток из пула имеет доступ к данным контекста логического вызова
            ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
            // Запрещаем копирование контекста исполнения потока метода Main
            ExecutionContext.SuppressFlow();

            // Заставляем поток из пула выполнить работу.
            // Поток из пула НЕ имеет доступа к данным контекста логического вызова
            ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
            // Восстанавливаем копирование контекста исполнения потока метода Main
            // на случай будущей работы с другими потоками из пула
            ExecutionContext.RestoreFlow();

            // Скоординированная отмена
            threading.CoordinatedCancel();

            // Задания
            ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5); // Вызов QueueUserWorkItem
            new Task(ComputeBoundOp, 5).Start(); // Аналог предыдущей строки
            Task.Run(() => ComputeBoundOp(5)); // Еще один аналог

            // Завершение задания и получение результата
            threading.EndTask();

            // Отмена задания
            threading.CancelTask();

            // Автоматический запуск задания по завершении предыдущего
            threading.RunTaskAfterEndPrevious();
            threading.RunTaskAfterEndPrevious2();

            // Дочерние задания
            threading.ChildTasks();

            // Планировщики заданий - сделать приложение WPF/Winforms

            // Parallel LINQ

            // Периодические вычислительные операции

            // События
            threading.Events();

            // Блокировка с двойной проверкой
            String name = null;
            // Так как имя равно null, запускается делегат и инициализирует поле имени
            LazyInitializer.EnsureInitialized(ref name, () => "Jeffrey");
            Console.WriteLine(name); // Выводится "Jeffrey"
                                     // Так как имя отлично от null, делегат не запускается и имя не меняется
            LazyInitializer.EnsureInitialized(ref name, () => "Richter");
            Console.WriteLine(name); // Снова выводится "Jeffrey"
            #endregion

            Console.ReadLine();
        }

        #region Threading
        // Сигнатура метода должна совпадать
        // с сигнатурой делегата ParameterizedThreadStart
        private static void ComputeBoundOp(Object state)
        {
            // Метод, выполняемый выделенным потоком
            Console.WriteLine("In ComputeBoundOp: state={0}", state);
            Thread.Sleep(1000); // Имитация другой работы (1 секунда)
                                // После возвращения методом управления выделенный поток завершается
        }

        private static void Worker()
        {
            Thread.Sleep(10000); // Имитация 10 секунд работы
                                 // Следующая строка выводится только для кода,
                                 // исполняемого активным потоком
            Console.WriteLine("Returning from Worker");
        }
    }
    #endregion
}

