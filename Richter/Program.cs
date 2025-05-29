using System;
using System.Collections.Generic;
using System.Threading;
using static Richter.Threading;

namespace Richter
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Threading
            // TaskCreationOptions
            // TaskScheduler

            var threading = new Threading();

            //SynchronizedQueue();

            #region aleek
            //Console.WriteLine("\n*** Потоки для асинхронных вычислительных операций ***");
            //Console.WriteLine("Main thread: starting a dedicated thread " + "to do an asynchronous operation");
            //Thread dedicatedThread = new Thread(ComputeBoundOp);
            //dedicatedThread.Start(5);
            //Console.WriteLine("Main thread: Doing other work here...");
            //Thread.Sleep(10000); // Имитация другой работы (10 секунд)
            // если на брейкпоинте ждать сколько задержка в ComputeBoundOp - следующая строка - мгновенно из-за Start. Дочерний поток запущен и выполняется, пока стоим на брейкпоинте
            // заметно, если поставить в ComputeBoundOp задержку сек 10. с 1 сек незаметно из-за асинхрона брейкпоинта
            // без Start - ошибка
            // есть смысл в строке и примере? главный и саб поток синхронны. Join выполнится выполнится мгновенно
            //dedicatedThread.Join(); // Ожидание завершения потока. ждём завершения dedicatedThread
            Console.WriteLine("Hit <Enter> to end this program...");
            /*
                Результат компиляции и запуска такого кода:
                Main thread: starting a dedicated thread to do an asynchronous operation
                Main thread: Doing other work here...
                In ComputeBoundOp: state=5

                Так как мы не можем контролировать очередность исполнения потоков
                в Windows, возможен и другой результат:
                Main thread: starting a dedicated thread to do an asynchronous operation // ? не получается
                In ComputeBoundOp: state=5
                Main thread: Doing other work here...
            */
            #endregion

            #region region aleek
            //Console.WriteLine("\n*** Фоновые и активные потоки ***");
            //// Создание нового потока (по умолчанию активного)
            //Thread t = new Thread(Worker);
            //// Превращение потока в фоновый
            ////t.IsBackground = true; // ни на что не влияет
            //t.Start(); // Старт потока
            //           // В случае активного потока приложение будет работать около 10 секунд
            //           // В случае фонового потока приложение немедленно прекратит работу
            //Console.WriteLine("Returning from Main");
            #endregion

            #region
            //Console.WriteLine("\n*** Простые вычислительные операции ***");
            //Console.WriteLine("Main thread: queuing an asynchronous operation");
            //ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5);
            //Console.WriteLine("Main thread: Doing other work here...");
            //Thread.Sleep(1000); // Имитация другой работы (10 секунд)
            //Console.WriteLine("Hit <Enter> to end this program...");
            /*
                Результат компиляции и запуска этого кода:
                Main thread: queuing an asynchronous operation
                Main thread: Doing other work here...
                In ComputeBoundOp: state=5
                
                // не получается. воможно из-за разного окружения, т.к. книга была написана давно
                // ошибка? нужно результата не будет. поменять строки Console.WriteLine("Main thread: Doing other work here..."); и Thread.Sleep(1000);  местами
                Впрочем, возможен и такой результат: 
                Main thread: queuing an asynchronous operation
                In ComputeBoundOp: state=5
                Main thread: Doing other work here...       
            
                Разный порядок следования строк в данном случае объясняется асинхронным
                выполнением методов. Планировщик Windows решает, какой поток должен выполняться первым, или же планирует их для одновременного выполнения на
                многопроцессорном компьютере
            */
            #endregion

            #region
            //Console.WriteLine("\n*** Контексты исполнения ***");
            //// Помещаем данные в контекст логического вызова потока метода Main
            //CallContext.LogicalSetData("Name", "Jeffrey");
            //// Заставляем поток из пула работать
            //// Поток из пула имеет доступ к данным контекста логического вызова
            //ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
            //// Запрещаем копирование контекста исполнения потока метода Main
            //ExecutionContext.SuppressFlow();
            //// Заставляем поток из пула выполнить работу. Поток из пула НЕ имеет доступа к данным контекста логического вызова
            //ThreadPool.QueueUserWorkItem(state => Console.WriteLine("Name={0}", CallContext.LogicalGetData("Name")));
            //// Восстанавливаем копирование контекста исполнения потока метода Main на случай будущей работы с другими потоками из пула
            //ExecutionContext.RestoreFlow();
            #endregion

            #region
            //Console.WriteLine("\n*** Задания ***");
            //ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5); // Вызов QueueUserWorkItem
            //new Task(ComputeBoundOp, 5).Start(); // Аналог предыдущей строки
            //Task.Run(() => ComputeBoundOp(5)); // Еще один аналог
            #endregion

            #region
            //Console.WriteLine("\n*** Автоматический запуск задания по завершении предыдущего ***");
            //threading.RunTaskAfterEndPrevious();
            //threading.RunTaskAfterEndPrevious2();

            //Console.WriteLine("\n*** Дочерние задания ***");
            //threading.ChildTasks();

            //Console.WriteLine("\n*** Фабрика заданий ***");
            //Console.WriteLine("\n*** TaskFactory без исключений ***");
            //threading.TaskFactory();

            //Console.WriteLine("\n*** TaskFactory с исключениями ***");
            //threading.TaskFactoryException();
            #endregion

            //Threading.StrangeBehavior.MainStrangeBehavior();// internal, public - результат такой же

            // Планировщики заданий - сделать приложение WPF/Winforms

            // Parallel LINQ

            // Периодические вычислительные операции

            #region
            //Console.WriteLine("\n*** Блокировка с двойной проверкой ***");
            //String name = null;
            //// Так как имя равно null, запускается делегат и инициализирует поле имени
            //LazyInitializer.EnsureInitialized(ref name, () => "Jeffrey");
            //Console.WriteLine(name); // Выводится "Jeffrey". Так как имя отлично от null, делегат не запускается и имя не меняется
            //LazyInitializer.EnsureInitialized(ref name, () => "Richter");
            //Console.WriteLine(name); // Снова выводится "Jeffrey"
            #endregion

            //Console.WriteLine("\n*** Расширяемость асинхронных функций ***");
            //Go();

            //Console.WriteLine("\n*** Другие возможности асинхронных функций ***");
            //Threading.GoAnother();
            //Threading.GoAnother2();

            //Console.WriteLine("\n*** Отмена операций ввода-вывода ***");
            //Threading.GoCancelIO();

            //ConcurrentExclusiveSchedulerDemo();

            //AccessResourceViaAsyncSynchronizationWrapper(new AsyncOneManyLock());

            //threading.MainConsumeItems();
            #endregion

            Console.ReadLine();
        }

        //static Thread thread1, thread2;
        //private static void ThreadProc()
        //{
        //    Console.WriteLine("\nCurrent thread: {0}", Thread.CurrentThread.Name);
        //    if (Thread.CurrentThread.Name == "Thread1" && thread2.ThreadState != ThreadState.Unstarted) 
        //    {
        //        thread2.Join();
        //    }
        //    Thread.Sleep(1000);
        //    Console.WriteLine("\nCurrent thread: {0}", Thread.CurrentThread.Name);
        //    Console.WriteLine("Thread1: {0}", thread1.ThreadState);
        //    Console.WriteLine("Thread2: {0}\n", thread2.ThreadState);
        //}

        // Сигнатура метода должна совпадать с сигнатурой делегата ParameterizedThreadStart
        private static void ComputeBoundOp(Object state) // параметр 5 берётся из Start
        {
            // Метод, выполняемый выделенным потоком
            Console.WriteLine("In ComputeBoundOp: state={0}", state);
            Thread.Sleep(1000); // Имитация другой работы (1 секунда). После возвращения методом управления выделенный поток завершается
        }

        private static void Worker()
        {
            Thread.Sleep(10000); // Имитация 10 секунд работ. Следующая строка выводится только для кода, исполняемого активным потоком
            Console.WriteLine("Returning from Worker");
        }

        private static void SynchronizedQueue() 
        {
            // значения в очереди будут в таком порядке, даже если без Monitor, т.к. в одном главном потоке
            //var synchronizedQueue = new SynchronizedQueue<int>();
            //synchronizedQueue.Enqueue(1);
            //synchronizedQueue.Enqueue(2);
            //synchronizedQueue.Enqueue(3);
            //synchronizedQueue.Enqueue(4);
            //synchronizedQueue.Enqueue(5);
            //synchronizedQueue.Dequeue();

            //for (int i = 0; i < 10; i++)
            //{
            //    var t1 = new Thread(() => {});
            //    t1.Start();
            //    t1.Start(); // на какой-то итерации падает
            //}

            #region region
            // значения в очереди будут в таком порядке, если с Monitor, без Monitor - результат не предсказуем            new Thread(() => synchronizedQueue.Enqueue(1)).Start();
            var synchronizedQueue = new SynchronizedQueue<int>();

            #region region
            //var arr = new[] { 
            //    new Thread(() => synchronizedQueue.Enqueue(1)),
            //    new Thread(() => synchronizedQueue.Enqueue(2)),
            //    new Thread(() => synchronizedQueue.Enqueue(3)),
            //    new Thread(() => synchronizedQueue.Enqueue(4)),
            //    new Thread(() => synchronizedQueue.Enqueue(5))
            //};
            //arr[new Random().Next(1, 5)].Start(); // сделать строго разный индекс от 1 до 6
            //arr[new Random().Next(1, 5)].Start();
            //arr[new Random().Next(1, 5)].Start();
            //arr[new Random().Next(1, 5)].Start();
            //arr[new Random().Next(1, 5)].Start();
            #endregion

            // сделать через паттерн сочетания без потока/под потоком/счётчик 1000/счётчик 10000
            var dt1 = DateTime.Now;
            var res1 = dt1.Second + " " + dt1.Millisecond;
            //for (int i = 0; i < 100000000; i++) // зависает под потоком
            for (int i = 0; i < 10; i++)
            {
                //lock (new object()) 
                //{
                    //synchronizedQueue.Enqueue(i);

                    // некоторые итерации игнорируются потоком. последняя итерация по любому зафиксируется
                    // не успевает создаваться новый поток каждую итерацию. эффект запоминания i
                    // кейс m_queue[2]...m_queue[5] = 5. притормаживаются 4 потока, а потом скопом отрабатывают на последнем i = 5. видно в дебаггере. правильно: m_queue[2]...m_queue[5] - 3...6 
                    new Thread(() => synchronizedQueue.Enqueue(i)).Start(); // Monitor в Enqueue не влияет
                    
                    //Thread.Sleep(10); // с паузой m_queue заполняется корректно
                //}

                var a1 = synchronizedQueue.M_queue; // точка останова на каждой итерации - то корректно, то некорректно
            }
            // точка останова на каждой итерации for. быстро кликать - некорректно, медленно кликать - корректно
            // если for (int i = 0; i < 10; i++) откуда-то появляется значение 10
            var a2 = synchronizedQueue.M_queue; 
            
            var dt2 = DateTime.Now;
            var res2 = dt2.Second + " " + dt2.Millisecond;

            synchronizedQueue.Dequeue();
            #endregion

            var tCnt = 10;
            var dict = new Dictionary<int, Thread>();
            for (int i = 0; i < tCnt; i++)
            {
                var rand = new Random().Next(0, tCnt);
                try
                {

                    dict.Add(rand, new Thread(() => synchronizedQueue.Enqueue(i)));
                    //dict.Add(rand, synchronizedQueue.Enqueue(i));

                }
                catch
                {
                    //continue;

                }
                dict[rand].Start();
            }
            //dict.Add(new Random().Next(1, 5), new Thread(() => synchronizedQueue.Enqueue(2)));
            //dict.Add(new Random().Next(1, 5), new Thread(() => synchronizedQueue.Enqueue(3)));
            //dict.Add(new Random().Next(1, 5), new Thread(() => synchronizedQueue.Enqueue(4)));
            //dict.Add(new Random().Next(1, 5), new Thread(() => synchronizedQueue.Enqueue(5)));
            for (int i = 0; i < tCnt; i++)
            {
                try
                {
                    dict[i].Start();
                }
                catch { }
            }

            synchronizedQueue.Dequeue();
        }
    }
}

