using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Richter
{
    class Threading
    {
        #region Скоординированная отмена
        private static void Count(CancellationToken token, Int32 countTo)
        {
            for (Int32 count = 0; count < countTo; count++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Count is cancelled");
                    break; // Выход их цикла для остановки операции
                }
                Console.WriteLine(count);
                Thread.Sleep(1000); // Для демонстрационных целей просто ждем
            }
            Console.WriteLine("Count is done");
        }

        public void CancelCount()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            // Передаем операции CancellationToken и число
            ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 10));
            Console.WriteLine("Press <Enter> to cancel the operation.");
            Console.ReadLine();
            cts.Cancel();
            // Если метод Count уже вернул управления. Cancel не оказывает никакого эффекта.
            // Cancel немедленно возвращает управление, метод продолжает работу...            
            Console.WriteLine("Подождите...");
            Thread.Sleep(3000);
        }

        public void CoordinatedCancel()
        {
            // Создание объекта CancellationTokenSource
            var cts1 = new CancellationTokenSource();
            Action a1 = () => // не сработает, т.к. не вызывается cts1.Cancel()
            {
                Console.WriteLine("cts1 canceled");
            };
            cts1.Token.Register(a1);

            // Создание второго объекта CancellationTokenSource
            var cts2 = new CancellationTokenSource();
            Action a2 = () => // сработает, когда вызовется cts2.Cancel(), но сначала - linked
            {
                Console.WriteLine("cts2 canceled");
            };
            cts2.Token.Register(a2);

            // Создание нового объекта CancellationTokenSource, отменяемого при отмене cts1 или cts2
            CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
            Action linked = () => // сработает, когда вызовется cts1.Cancel() или cts2.Cancel()
            {
                Console.WriteLine("linkedCts canceled");
            };
            linkedCts.Token.Register(linked);

            // Отмена одного из объектов CancellationTokenSource (я выбрал cts2)
            cts1.Cancel(); // сработает коллбэк linked
            //cts2.Cancel(); // сработает коллбэк linked

            // Показываем, какой из объектов CancellationTokenSource был отменен
            Console.WriteLine("cts1 canceled={0}, cts2 canceled={1}, linkedCts={2}", cts1.IsCancellationRequested, cts2.IsCancellationRequested, linkedCts.IsCancellationRequested);
        }


        #endregion

        private static Int32 Sum(Int32 n)
        {
            Int32 sum = 0;
            for (; n > 0; n--)
            {
                checked
                {
                    sum += n;
                } // при больших n выдается System.OverflowException
                Thread.Sleep(100);
            }
            return sum;
        }

        private static Int32 Sum(CancellationToken сt, Int32 n)
        {
            Int32 sum = 0;
            for (; n > 0; n--)
            {
                // Следующая строка приводит к исключению OperationCanceledException при вызове метода Cancel для объекта CancellationTokenSource, на который ссылается маркер
                сt.ThrowIfCancellationRequested();
                checked { sum += n; } // при больших n появляется исключение System.OverflowException
            }
            return sum;
        }

        #region Завершение задания и получение результата
        public void EndTask() 
        {
            // Создание задания Task (оно пока не выполняется)
            Task<Int32> t = new Task<Int32>(n => Sum((Int32)n), 10); 

            // Можно начать выполнение задания через некоторое время
            t.Start();

            // Можно ожидать завершения задания в явном виде
            // если закомментировать - результат почему-то тот же
            t.Wait(); // ПРИМЕЧАНИЕ. Существует перегруженная версия, принимающая тайм-аут/CancellationToken
                      // Получение результата (свойство Result вызывает метод Wait)
            Console.WriteLine("The Sum is: " + t.Result); // Значение Int32
        }

        public void EndTask2()
        {
            Task<Int32> t = new Task<Int32>(n => Sum((Int32)n), 10);
            //Console.WriteLine("The Sum is: " + t.Result); // зависнет - задача не запущена
            Console.WriteLine("The Sum is: "); // тут же выполнится - задача просто объявлена
        }
        #endregion

        #region Отмена задания
        public void CancelTask()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<Int32> t = new Task<Int32>(() => Sum(cts.Token, 10000), cts.Token);
            t.Start();
            // Позднее отменим CancellationTokenSource, чтобы отменить Task
            cts.Cancel(); // Это асинхронный запрос, задача уже может быть завершена
            try
            {
                // В случае отмены задания метод Result генерирует исключение AggregateException
                Console.WriteLine("The sum is: " + t.Result); // Значение Int32
            }
            catch (AggregateException x)
            {
                // Считаем обработанными все объекты OperationCanceledException
                // Все остальные исключения попадают в новый объект AggregateException, состоящий только из необработанных исключений
                x.Handle(e => e is OperationCanceledException); // комментирование не повлияет
                // Строка выполняется, если все исключения уже обработаны
                Console.WriteLine("Sum was canceled");
            }
        }
        #endregion

        #region Автоматический запуск задания по завершении предыдущего
        public void RunTaskAfterEndPrevious() 
        {
            // Создание объекта Task с отложенным запуском
            Task<Int32> t = Task.Run(() => Sum(CancellationToken.None, 10));
            // Метод ContinueWith возвращает объект Task, но обычно он не используется
            Task cwt = t.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result));

            //t.Start(); // System.InvalidOperationException: "Start нельзя вызывать для уже запущенной задачи."
            //t.Wait();
            //Console.WriteLine("The Sum is: " + t.Result); // Значение Int32
        }
                                                           
        public void RunTaskAfterEndPrevious2()
        {
            // Создание и запуск задания с продолжением
            Task<Int32> t = Task.Run(() => Sum(10));
            // Метод ContinueWith возвращает объект Task, но обычно он не используется
            t.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
            // обернуть в исключение
            t.ContinueWith(task => Console.WriteLine("Sum threw: " + task.Exception), TaskContinuationOptions.OnlyOnFaulted);
            // отменить задачу. У task нет метода отмены
            t.ContinueWith(task => Console.WriteLine("Sum was canceled"), TaskContinuationOptions.OnlyOnCanceled);
        }
        #endregion

        #region Дочерние задания
        public void ChildTasks() 
        {
            Task<Int32[]> parent = new Task<Int32[]>(() => {
                var results = new Int32[3]; // Создание массива для результатов. Создание и запуск 3 дочерних заданий
                new Task(() => results[0] = Sum(10000), TaskCreationOptions.AttachedToParent).Start();
                new Task(() => results[1] = Sum(20000), TaskCreationOptions.AttachedToParent).Start();
                new Task(() => results[2] = Sum(30000), TaskCreationOptions.AttachedToParent).Start();
                // Возвращается ссылка на массив (элементы могут быть не инициализированы)
                return results;
            });
            // Вывод результатов после завершения родительского и дочерних заданий
            var cwt = parent.ContinueWith(parentTask => { Array.ForEach(parentTask.Result, Console.WriteLine); Console.WriteLine("ContinueWith"); });
            // Запуск родительского задания, которое запускает дочерние
            Console.WriteLine("parent.Start()");
            parent.Start();
        }
        #endregion

        #region Фабрика заданий
        const int _1_SEC = 1000;
        const int _2_SEC = 2000;
        const int _3_SEC = 3000;

        int Delay1s() { Thread.Sleep(_1_SEC); return _1_SEC; }
        int Delay2s() { Thread.Sleep(_2_SEC); return _2_SEC; }
        int Delay3s() { Thread.Sleep(_3_SEC); return _3_SEC; }

        public void TaskFactory()
        {
            Task parent = new Task(() => {
                var cts = new CancellationTokenSource();
                var tf = new TaskFactory<Int32>(cts.Token, TaskCreationOptions.AttachedToParent, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                // Задание создает и запускает 3 дочерних задания
                var childTasks = new[] {
                    tf.StartNew(() => Delay1s()),
                    tf.StartNew(() => Delay2s()),
                    tf.StartNew(() => Delay3s())
                };
                for (Int32 task = 0; task < childTasks.Length; task++)
                {
                    // если TaskContinuationOptions.OnlyOnFaulted и нет исключения коллбэк в ContinueWith не сработает
                    // как поставить точку останова в колбэке?
                    // всегда показывает 3, потому что к моменту ContinueWhenAll цикл уже прогнан
                    // как сделать порядковый номер задачи?
                    // коллбэк сработает после ContinueWhenAll
                    childTasks[task].ContinueWith(t => Console.WriteLine($"ContinueWith Id: {t.Id} iterator: {task}"), TaskContinuationOptions.None);
                }
                // После завершения дочерних заданий получаем максимальное возвращенное значение и передаем его другому заданию для вывода
                tf.ContinueWhenAll(childTasks, completedTasks => completedTasks.Where(t => !t.IsFaulted && !t.IsCanceled).Max(t => t.Result), CancellationToken.None)
                  .ContinueWith(t => Console.WriteLine("The maximum is: " + t.Result), TaskContinuationOptions.ExecuteSynchronously);
            });
            // Запуск родительского задания, которое может запускать дочерние
            Console.WriteLine("parent.Start()");
            parent.Start();
        }

        int Log()
        {
            Console.WriteLine("Log");
            return 0;
        }

        public void TaskFactoryException() 
        {
            Task parent = new Task(() => {
                var cts = new CancellationTokenSource();
                var tf = new TaskFactory<Int32>(cts.Token, TaskCreationOptions.AttachedToParent, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                // Задание создает и запускает 3 дочерних задания
                var childTasks = new[] {
                    tf.StartNew(() => Log()), // Thread.Sleep
                    tf.StartNew(() => Sum(cts.Token, 10000)),
                    tf.StartNew(() => Sum(cts.Token, 20000)),
                    tf.StartNew(() => Sum(cts.Token, Int32.MaxValue)) // Исключение OverflowException
                };
                // Если дочернее задание становится источником исключения, отменяем все дочерние задания
                for (Int32 task = 0; task < childTasks.Length; task++)
                {
                    childTasks[task].ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
                }
                // После завершения дочерних заданий получаем максимальное возвращенное значение и передаем его другому заданию для вывода
                tf.ContinueWhenAll(childTasks, completedTasks => completedTasks.Where(t => !t.IsFaulted && !t.IsCanceled).Max(t => t.Result), CancellationToken.None)
                  .ContinueWith(t => Console.WriteLine("The maximum is: " + t.Result), TaskContinuationOptions.ExecuteSynchronously);
            });
            // После завершения дочерних заданий выводим, в том числе, и необработанные исключения
            parent.ContinueWith(p => {
                // p.Result
                // Текст помещен в StringBuilder и однократно вызван метод Console.WriteLine просто потому, что это задание
                // может выполняться параллельно с предыдущим, и я не хочу путаницы в выводимом результате
                StringBuilder sb = new StringBuilder("The following exception(s) occurred:" + Environment.NewLine);
                foreach (var e in p.Exception.Flatten().InnerExceptions)
                {
                    sb.AppendLine(" " + e.GetType().ToString());
                }
                Console.WriteLine(sb.ToString());
            }, TaskContinuationOptions.OnlyOnFaulted);
            //parent.ContinueWith().ContinueWith()...
            // Запуск родительского задания, которое может запускать дочерние
            Console.WriteLine("parent.Start()");
            parent.Start();
        }
        #endregion

        #region Периодические вычислительные операции
        #endregion

        #region Расширяемость асинхронных функций
        public static async Task Go()
        {
            #if DEBUG
                // Использование TaskLogger приводит к лишним затратам памяти и снижению производительности; включить для отладочной версии
                TaskLogger.LogLevel = TaskLogger.TaskLogLevel.Pending;
            #endif
            // Запускаем 3 задачи; для тестирования TaskLogger их продолжительность задается явно.
            var tasks = new List<Task> 
            {
                Task.Delay(2000).Log("2s op"),
                Task.Delay(5000).Log("5s op"),
                Task.Delay(6000).Log("6s op")
            };
            try
            {
                // Ожидание всех задач с отменой через 3 секунды; только одна задача должна завершиться в указанное время.
                // Примечание: WithCancellation - мой метод расширения, описанный позднее в этой главе.
                //await Task.WhenAll(tasks).WithCancellation(new CancellationTokenSource(3000).Token);
                
            }
            catch (OperationCanceledException)
            {
            }
            // Запрос информации о незавершенных задачах и их сортировка по убыванию продолжительности ожидания
            foreach (var op in TaskLogger.GetLogEntries().OrderBy(tle => tle.LogTime))
            {
                Console.WriteLine(op);
            }
        }
        #endregion

        #region Асинхронные функции в FCL 
        // нигде не используется, просто описан
        private static async Task<String> AwaitWebClient(Uri uri)
        {
            // Класс System.Net.WebClient поддерживает событийную модель асинхронного программирования
            var wc = new System.Net.WebClient();
            // Создание объекта TaskCompletionSource и его внутреннего объекта Task
            var tcs = new TaskCompletionSource<String>();
            // При завершении загрузки строки объект WebClient инициирует событие DownloadStringCompleted, завершающее TaskCompletionSource
            wc.DownloadStringCompleted += (s, e) => {
                if (e.Cancelled)
                {
                    tcs.SetCanceled();
                }
                else if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult(e.Result);
                }
            };
            // Начало асинхронной операции
            wc.DownloadStringAsync(uri);
            // Теперь мы можем взять объект Task из TaskCompletionSource и обработать результат обычным способом.
            String result = await tcs.Task;
            // Обработка строки результата (если нужно)...
            return result;
        }
        #endregion

        #region
        private static async Task<String> IssueClientRequestAsync(String serverName, String message)
        {
            using (var pipe = new NamedPipeClientStream(serverName, "PipeName", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough))
            {
                pipe.Connect(); // Прежде чем задавать ReadMode, необходимо
                pipe.ReadMode = PipeTransmissionMode.Message; // вызвать Connect. Асинхронная отправка данных серверу
                Byte[] request = Encoding.UTF8.GetBytes(message);
                await pipe.WriteAsync(request, 0, request.Length);
                // Асинхронное чтение ответа сервера
                Byte[] response = new Byte[1000];
                Int32 bytesRead = await pipe.ReadAsync(response, 0, response.Length);
                return Encoding.UTF8.GetString(response, 0, bytesRead);
            } // Закрытие канала
        }
        #endregion

        #region Другие возможности асинхронных функций
        // Task.Run вызывается в потоке графического интерфейса
        /*Task.Run(async () => {
             // Этот код выполняется в потоке из пула
             // TODO: Подготовительные вычисления...
             await XxxAsync(); // Инициирование асинхронной операции
                               // Продолжение обработки...
        });*/

        private static async void StartServer()
        {
            while (true)
            {
                string c_pipeName = null;
                var pipe = new NamedPipeServerStream(c_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
                // Асинхронный прием клиентского подключения. ПРИМЕЧАНИЕ: NamedPipeServerStream использует старую модель асинхронного программирования.
                // Я преобразую ее к новой модели Task при помощи метода FromAsync класса TaskFactory.
                await Task.Factory.FromAsync(pipe.BeginWaitForConnection, pipe.EndWaitForConnection, null);
                // Начало обслуживания клиента; управление возвращается немедленно, потому что операция выполняется асинхронно.
                //ServiceClientRequestAsync(pipe);
            }
        }

        public static async Task Go_AnotherAbilitiesOfAsyncFunctions()
        {
            // Запуск сервера немедленно возвращает управление, потому что сервер ожидает клиентские запросы в асинхронном режиме
            StartServer(); // Возвращает void, компилятор выдает предупреждение. Создание набора асинхронных клиентских запросов; сохраняем Task<String> каждого клиента.
            List<Task<String>> requests = new List<Task<String>>(10000);
            for (Int32 n = 0; n < requests.Capacity; n++)
            {
                requests.Add(IssueClientRequestAsync("localhost", "Request #" + n));
            }
            // Асинхронное ожидание завершения всех клиентских запросов. ВНИМАНИЕ: если 1+ заданий выдадут исключение, WhenAll заново инициирует последнее исключение
            String[] responses = await Task.WhenAll(requests);
            // Обработка всех запросов
            for (Int32 n = 0; n < responses.Length; n++)
            {
                Console.WriteLine(responses[n]);
            }
        }

        public static async Task Go_AnotherAbilitiesOfAsyncFunctions2()
        {
            // Запуск сервера немедленно возвращает управление, потому что сервер ожидает клиентские запросы в асинхронном режиме
            StartServer();
            // Создание набора асинхронных клиентских запросов; сохраняем Task<String> каждого клиента.
            List<Task<String>> requests = new List<Task<String>>(10000);
            for (Int32 n = 0; n < requests.Capacity; n++)
            {
                //requests.Add(IssueClientRequestAsync("localhost", "Request #" + n));
            }

            // Продолжение с завершением КАЖДОЙ задачи
            while (requests.Count > 0)
            {
                // Последовательная обработка каждого завершенного ответа
                Task<String> response = await Task.WhenAny(requests);
                requests.Remove(response); // Удаление завершенной задачи из коллекции. Обработка одного ответа
                Console.WriteLine(response.Result);
            }
        }
        #endregion

        // Потоковые модели приложений (WPF)

        #region Отмена операций ввода-вывода
        public static async Task Go_CancelIO()
        {
            // Создание объекта CancellationTokenSource, отменяющего себя через заданный промежуток времени в миллисекундах
            var cts = new CancellationTokenSource(5000); // Чтобы отменить ранее, вызовите cts.Cancel()
            var ct = cts.Token; 
            try
            {
                // Я использую Task.Delay для тестирования; замените другим методом, возвращающим Task
                // Delay возвращает Task, а не Task<TResult>, поэтому .WithCancellation(ct) применить не получится
                //await Task.Delay(10000).WithCancellation(ct);
                //await Task.Delay(10000).WithCancellationEmpty(ct);
                Console.WriteLine("Task completed");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Task cancelled");
            }
        }
        #endregion

        #region Приоритеты запросов ввода-вывода
        //using (ThreadIO.BeginBackgroundProcessing()) {
        // // Здесь располагается низкоприоритетный запрос ввода-вывода
        // // (например, вызов BeginRead/BeginWrite)
        //}
        #endregion

        #region Volatile-конструкции
        internal static class StrangeBehavior
        {
            // Далее вы увидите, что проблема решается объявлением этого поля volatile
            private static Boolean s_stopWorker = false;
            
            public static void MainStrangeBehavior()
            {
                Console.WriteLine("Main: letting worker run for 5 seconds");
                Thread t = new Thread(Worker);
                t.Start();
                Thread.Sleep(1);
                s_stopWorker = true;
                Console.WriteLine("Main: waiting for worker to stop");
                t.Join();
            }

            private static void Worker(Object o) // почему параметр, new Thread(Worker) - без параметра
            {
                double x = 0;
                while (!s_stopWorker) 
                {
                    x++; 
                }
                Console.WriteLine("Worker: stopped when x={0}", x/100000);
            }
        }

        internal sealed class ThreadsSharingData
        {
            private Int32 m_flag = 0;
            private Int32 m_value = 0;
            // Этот метод исполняется одним потоком
            public void Thread1()
            {
                // ПРИМЕЧАНИЕ. Они могут выполняться в обратном порядке
                m_value = 5;
                m_flag = 1;
            }
            // Этот метод исполняется другим потоком
            public void Thread2()
            {
                // ПРИМЕЧАНИЕ. Поле m_value может быть прочитано раньше, чем m_flag
                if (m_flag == 1)
                {
                    Console.WriteLine(m_value);
                }
            }
        }

        internal sealed class ThreadsSharingData2
        {
            private Int32 m_flag = 0;
            private Int32 m_value = 0;
            // Этот метод выполняется одним потоком
            public void Thread1()
            {
                // ПРИМЕЧАНИЕ. 5 нужно записать в m_value до записи 1 в m_flag
                m_value = 5;
                Volatile.Write(ref m_flag, 1);
            }
            // Этот метод выполняется вторым потоком
            public void Thread2()
            {
                // ПРИМЕЧАНИЕ. Поле m_value должно быть прочитано после m_flag
                if (Volatile.Read(ref m_flag) == 1)
                {
                    Console.WriteLine(m_value);
                }
            }
        }

        internal sealed class ThreadsSharingData3
        {
            private volatile Int32 m_flag = 0;
            private Int32 m_value = 0;
            // Этот метод исполняется одним потоком
            public void Thread1()
            {
                // ПРИМЕЧАНИЕ. Значение 5 должно быть записано в m_value перед записью 1 в m_flag
                m_value = 5;
                m_flag = 1;
            }
            // Этот метод исполняется другим потоком
            public void Thread2()
            {
                // ПРИМЕЧАНИЕ. Поле m_value должно быть прочитано после m_flag
                if (m_flag == 1)
                {
                    Console.WriteLine(m_value);
                }
            }
        }
        #endregion

        #region MultiWebRequests
        internal enum CoordinationStatus { AllDone, Timeout, Cancel };

        internal sealed class MultiWebRequests
        {
            // Этот класс Helper координирует все асинхронные операции
            private AsyncCoordinator m_ac = new AsyncCoordinator();
            
            // Набор веб-серверов, к которым будут посылаться запросы. Хотя к этому словарю возможны одновременные обращения,
            // в синхронизации доступа нет необходимости, потому что ключи после создания доступны только для чтения
            private Dictionary<String, Object> m_servers = new Dictionary<String, Object> {
                 { "http://Wintellect.com/", null },
                 { "http://Microsoft.com/", null },
                 { "http://1.1.1.1/", null }
            };

            public MultiWebRequests(Int32 timeout = Timeout.Infinite)
            {
                // Асинхронное инициирование всех запросов
                var httpClient = new HttpClient();
                foreach (var server in m_servers.Keys)
                {
                    m_ac.AboutToBegin(1);
                    httpClient.GetByteArrayAsync(server).ContinueWith(task => ComputeResult(server, task));
                }
                // Сообщаем AsyncCoordinator, что все операции были инициированы и 
                // что он должен вызвать AllDone после завершения всех операций, вызова Cancel или тайм-аута
                m_ac.AllBegun(AllDone, timeout);
            }

            private void ComputeResult(String server, Task<Byte[]> task)
            {
                Object result;
                if (task.Exception != null)
                {
                    result = task.Exception.InnerException;
                }
                else
                {
                    // Обработка завершения ввода-вывода - здесь или в потоке(-ах) пула. Разместите свой вычислительный алгоритм...
                    result = task.Result.Length; // В данном примере
                } // просто возвращается длина. Сохранение результата (исключение/сумма) и обозначение одной завершенной операции
                m_servers[server] = result;
                m_ac.JustEnded();
            }

            // При вызове этого метода результаты игнорируются
            public void Cancel() 
            { 
                m_ac.Cancel(); 
            }

            // Этот метод вызывается после получения ответа от всех веб-серверов, вызова Cancel или тайм-аута
            private void AllDone(CoordinationStatus status)
            {
                switch (status)
                {
                    case CoordinationStatus.Cancel:
                        Console.WriteLine("Operation canceled.");
                        break;
                    case CoordinationStatus.Timeout:
                        Console.WriteLine("Operation timedout.");
                        break;
                    case CoordinationStatus.AllDone:
                        Console.WriteLine("Operation completed; results below:");
                        foreach (var server in m_servers)
                        {
                            Console.Write("{0} ", server.Key);
                            Object result = server.Value;
                            if (result is Exception)
                            {
                                Console.WriteLine("failed due to {0}.", result.GetType().Name);
                            }
                            else
                            {
                                Console.WriteLine("returned {0:N0} bytes.", result);
                            }
                        }
                        break;
                }
            }
        }

        internal sealed class AsyncCoordinator
        {
            private Int32 m_opCount = 1; // Уменьшается на 1 методом AllBegun
            private Int32 m_statusReported = 0; // 0=false, 1=true
            private Action<CoordinationStatus> m_callback;
            private Timer m_timer;

            // Этот метод ДОЛЖЕН быть вызван ДО инициирования операции
            public void AboutToBegin(Int32 opsToAdd = 1)
            {
                Interlocked.Add(ref m_opCount, opsToAdd);
            }

            // Этот метод ДОЛЖЕН быть вызван ПОСЛЕ обработки результата
            public void JustEnded()
            {
                if (Interlocked.Decrement(ref m_opCount) == 0)
                {
                    ReportStatus(CoordinationStatus.AllDone);
                }
            }

            // Этот метод ДОЛЖЕН быть вызван ПОСЛЕ инициирования ВСЕХ операций
            public void AllBegun(Action<CoordinationStatus> callback, Int32 timeout = Timeout.Infinite)
            {
                m_callback = callback;
                if (timeout != Timeout.Infinite)
                {
                    m_timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
                }
                JustEnded();
            }

            private void TimeExpired(Object o)
            {
                ReportStatus(CoordinationStatus.Timeout);
            }

            public void Cancel() 
            { 
                ReportStatus(CoordinationStatus.Cancel); 
            }

            private void ReportStatus(CoordinationStatus status)
            {
                // Если состояние ни разу не передавалось, передать его; в противном случае оно игнорируется
                if (Interlocked.Exchange(ref m_statusReported, 1) == 0)
                {
                    m_callback(status);
                }
            }
        }
        #endregion

        #region Реализация простой циклической блокировки
        internal struct SimpleSpinLock
        {
            private Int32 m_ResourceInUse; // 0=false (по умолчанию), 1=true
            
            public void Enter()
            {
                while (true)
                {
                    // Всегда указывать, что ресурс используется. Если поток переводит его из свободного состояния, вернуть управление
                    if (Interlocked.Exchange(ref m_ResourceInUse, 1) == 0) 
                    { 
                        return; 
                    }
                    // Здесь что-то происходит...
                }
            }
            
            public void Leave()
            {
                // Помечаем ресурс, как свободный
                Volatile.Write(ref m_ResourceInUse, 0);
            }
        }
        // создать конкуренцию из нескольких потоков
        public sealed class SomeResource
        {
            private SimpleSpinLock m_sl = new SimpleSpinLock();
            public void AccessResource()
            {
                m_sl.Enter();
                // Доступ к ресурсу в каждый момент времени имеет только один поток...
                m_sl.Leave();
            }
        }
        #endregion

        #region Универсальный Interlocked-паттерн
        public static Int32 Maximum(ref Int32 target, Int32 value)
        {
            Int32 currentVal = target, startVal, desiredVal;
            // Параметр target может использоваться другим потоком, его трогать не стоит
            do
            {
                // Запись начального значения этой итерации
                startVal = currentVal;
                // Вычисление желаемого значения в контексте startVal и value
                desiredVal = Math.Max(startVal, value);
                // ПРИМЕЧАНИЕ. Здесь поток может быть прерван!
                if (target == startVal) 
                { 
                    target = desiredVal; 
                }
                // Возвращение значения, предшествующего потенциальным изменениям
                currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
                // Если начальное значение на этой итерации изменилось, повторить
            } while (startVal != currentVal);
            // Возвращаем максимальное значение, когда поток пытается его присвоить
            return desiredVal;
        }

        public delegate Int32 Morpher<TResult, TArgument>(Int32 startValue, TArgument argument, out TResult morphResult);
        /// <summary>
        /// Morpher
        /// </summary>
        public static TResult Morph<TResult, TArgument>(ref Int32 target, TArgument argument, Morpher<TResult, TArgument> morpher)
        {
            TResult morphResult;
            Int32 currentVal = target, startVal, desiredVal;
            do
            {
                startVal = currentVal;
                desiredVal = morpher(startVal, argument, out morphResult);
                currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
            } while (startVal != currentVal);
            return morphResult;
        }
        #endregion

        #region События
        public void Events() 
        {
            Int32 x = 0;
            const Int32 iterations = 10000000; // 10 миллионов.
            
            // Сколько времени займет инкремент x 10 миллионов раз?
            Stopwatch sw = Stopwatch.StartNew();
            for (Int32 i = 0; i < iterations; i++)
            {
                x++;
            }
            Console.WriteLine("Incrementing x: {0:N0}", sw.ElapsedMilliseconds);
            
            // Сколько времени займет инкремент x 10 миллионов раз, если добавить вызов ничего не делающего метода?
            sw.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                M(); 
                x++; 
                M();
            }
            Console.WriteLine("Incrementing x in M: {0:N0}", sw.ElapsedMilliseconds);
            
            // Сколько времени займет инкремент x 10 миллионов раз, если добавить вызов неконкурирующего объекта SimpleSpinLock?
            SpinLock sl = new SpinLock(false);
            sw.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                Boolean taken = false; 
                sl.Enter(ref taken); 
                x++; 
                sl.Exit();
            }
            Console.WriteLine("Incrementing x in SpinLock: {0:N0}", sw.ElapsedMilliseconds);
            
            // Сколько времени займет инкремент x 10 миллионов раз, если добавить вызов неконкурирующего объекта SimpleWaitLock?
            using (SimpleWaitLock swl = new SimpleWaitLock(1))
            {
                sw.Restart();
                for (Int32 i = 0; i < iterations; i++)
                {
                    swl.Enter();
                    x++; 
                    swl.Leave();
                }
                Console.WriteLine("Incrementing x in SimpleWaitLock: {0:N0}", sw.ElapsedMilliseconds);
            }
        }

        public sealed class SimpleWaitLock : IDisposable
        {
            private Semaphore m_AvailableResources;
            public SimpleWaitLock(Int32 maximumConcurrentThreads)
            {
                m_AvailableResources = new Semaphore(maximumConcurrentThreads, maximumConcurrentThreads);
            }

            public void Enter()
            {
                // Ожидаем в ядре доступа к ресурсу и возвращаем управление
                m_AvailableResources.WaitOne();
            }

            public void Leave()
            {
                // Этому потоку доступ больше не нужен; его может получить другой поток
                m_AvailableResources.Release();
            }
            public void Dispose() 
            { 
                m_AvailableResources.Close(); 
            }
        }

        void M() 
        { 
        }
        #endregion

        #region Мьютексы
        internal sealed class RecursiveAutoResetEvent : IDisposable
        {
            private AutoResetEvent m_lock = new AutoResetEvent(true);
            private Int32 m_owningThreadId = 0;
            private Int32 m_recursionCount = 0;
            
            public void Enter()
            {
                // Получаем идентификатор вызывающего потока
                Int32 currentThreadId = Thread.CurrentThread.ManagedThreadId;
                // Если вызывающий поток блокируется, увеличиваем рекурсивный счетчик
                if (m_owningThreadId == currentThreadId)
                {
                    m_recursionCount++;
                    return;
                }
                // Вызывающий поток не имеет блокировки, ожидаем
                m_lock.WaitOne();
                // Теперь вызывающий поток блокируется, инициализируем идентификатор этого потока и рекурсивный счетчик
                m_owningThreadId = currentThreadId;
                m_recursionCount = 1;
            }

            public void Leave()
            {
                // Если вызывающий поток не является владельцем блокировки, произошла ошибка
                if (m_owningThreadId != Thread.CurrentThread.ManagedThreadId) 
                {
                    throw new InvalidOperationException();
                }
                // Вычитаем единицу из рекурсивного счетчика
                if (--m_recursionCount == 0)
                {
                    // Если рекурсивный счетчик равен 0, ни один поток не владеет блокировкой
                    m_owningThreadId = 0;
                    m_lock.Set(); // Пробуждаем один ожидающий поток (если такие есть)
                }
            }

            public void Dispose() 
            { 
                m_lock.Dispose(); 
            }
        }
        #endregion

        #region Простая гибридная блокировка
        internal sealed class SimpleHybridLock : IDisposable
        {
            // Int32 используется примитивными конструкциями пользовательского режима (Interlocked-методы)
            private Int32 m_waiters = 0;
            // AutoResetEvent - примитивная конструкция режима ядра
            private AutoResetEvent m_waiterLock = new AutoResetEvent(false);
            
            public void Enter()
            {
                // Поток хочет получить блокировку
                if (Interlocked.Increment(ref m_waiters) == 1) 
                {
                    // Блокировка свободна, конкуренции нет, возвращаем управление
                    // Блокировка захвачена другим потоком (конкуренция), приходится ждать.
                    return;
                }
                m_waiterLock.WaitOne(); // Значительное снижение производительности. Когда WaitOne возвращет управление, этот поток блокируется
            }
            
            public void Leave()
            {
                // Этот поток освобождает блокировку
                if (Interlocked.Decrement(ref m_waiters) == 0) 
                {
                    return; // Другие потоки не заблокированы, возвращаем управление. Другие потоки заблокированы, пробуждаем один из них
                }
                m_waiterLock.Set(); // Значительное снижение производительности
            }
            
            public void Dispose() 
            { 
                m_waiterLock.Dispose(); 
            }
        }
        #endregion

        #region Зацикливание, владение потоком и рекурсия
        internal sealed class AnotherHybridLock : IDisposable
        {
            // Int32 используется примитивом в пользовательском режиме (методы Interlocked)
            private Int32 m_waiters = 0;
            // AutoResetEvent — примитивная конструкция режима ядра
            private AutoResetEvent m_waiterLock = new AutoResetEvent(false);
            // Это поле контролирует зацикливание с целью поднять производительность
            private Int32 m_spincount = 4000; // Произвольно выбранное значение. Эти поля указывают, какой поток и сколько раз блокируется
            private Int32 m_owningThreadId = 0, m_recursion = 0;

            public void Enter()
            {
                // Если вызывающий поток уже захватил блокировку, увеличим рекурсивный счетчик на единицу и вернем управление
                Int32 threadId = Thread.CurrentThread.ManagedThreadId;
                if (threadId == m_owningThreadId) 
                { 
                    m_recursion++; 
                    return; 
                }
                // Вызывающий поток не захватил блокировку, пытаемся получить ее
                SpinWait spinwait = new SpinWait();
                for (Int32 spinCount = 0; spinCount < m_spincount; spinCount++)
                {
                    // Если блокирование возможно, этот поток блокируется. Задаем некоторое состояние и возвращаем управление
                    if (Interlocked.CompareExchange(ref m_waiters, 1, 0) == 0) 
                    { 
                        goto GotLock; 
                    }
                    // Даем остальным потокам шанс выполниться в надежде на снятие блокировки
                    spinwait.SpinOnce();
                }
                // Зацикливание завершено, а блокировка не снята, пытаемся еще раз
                if (Interlocked.Increment(ref m_waiters) > 1)
                {
                    // Остальные потоки заблокированы и этот также должен быть заблокирован
                    m_waiterLock.WaitOne(); // Ожидаем возможности блокирования; производительность падает
                                            // Проснувшись, этот поток получает право на блокирование
                                            // Задаем некоторое состояние и возвращаем управление
                }
            GotLock:
                // Когда поток блокируется, записываем его идентификатор и указываем, что он получил право на блокирование впервые
                m_owningThreadId = threadId; 
                m_recursion = 1;
            }

            public void Leave()
            {
                // Если вызывающий поток не заперт, ошибка
                Int32 threadId = Thread.CurrentThread.ManagedThreadId;
                if (threadId != m_owningThreadId) 
                { 
                    throw new SynchronizationLockException("Lock not owned by calling thread");
                }
                // Уменьшаем на единицу рекурсивный счетчик. Если поток все еще заперт, просто возвращаем управление
                if (--m_recursion > 0)
                {
                    return;
                }
                m_owningThreadId = 0; // Запертых потоков больше нет. Если нет других заблокированных потоков, возвращаем управление
                if (Interlocked.Decrement(ref m_waiters) == 0) 
                {
                    return; 
                }
                // Остальные потоки заблокированы, пробуждаем один из них
                m_waiterLock.Set(); // Значительное падение производительности
            }

            public void Dispose() 
            { 
                m_waiterLock.Dispose(); 
            }
        }
        #endregion

        #region Класс Monitor и блоки синхронизации
        internal sealed class Transaction
        {
            private DateTime m_timeOfLastTrans;

            public void PerformTransaction()
            {
                Monitor.Enter(this);
                // Этот код имеет эксклюзивный доступ к данным...
                m_timeOfLastTrans = DateTime.Now;
                Monitor.Exit(this);
            }

            public DateTime LastTransaction
            {
                get
                {
                    Monitor.Enter(this);
                    // Этот код имеет совместный доступ к данным...
                    DateTime temp = m_timeOfLastTrans;
                    Monitor.Exit(this);
                    return temp;
                }
            }
        }

        internal sealed class Transaction2 : IDisposable
        {
            private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            private DateTime m_timeOfLastTrans;

            public void PerformTransaction()
            {
                m_lock.EnterWriteLock();
                // Этот код имеет монопольный доступ к данным...
                m_timeOfLastTrans = DateTime.Now;
                m_lock.ExitWriteLock();
            }

            public DateTime LastTransaction
            {
                get
                {
                    m_lock.EnterReadLock();
                    // Этот код имеет совместный доступ к данным...
                    DateTime temp = m_timeOfLastTrans;
                    m_lock.ExitReadLock();
                    return temp;
                }
            }

            public void Dispose() 
            { 
                m_lock.Dispose(); 
            }
        }
        #endregion

        #region Блокировка с двойной проверкой
        internal sealed class Singleton
        {
            // Объект s_lock требуется для обеспечения безопасности в многопоточной среде. 
            // Наличие этого объекта предполагает, что для создания одноэлементного объекта требуется больше
            // ресурсов, чем для объекта System.Object и что эта процедура может вовсе не понадобиться. 
            // В противном случае проще и эффективнее получить одноэлементный объект в конструкторе класса
            private static readonly Object s_lock = new Object();
            // Это поле ссылается на один объект Singleton
            private static Singleton s_value = null;
            // Закрытый конструктор не дает внешнему коду создавать экземпляры
            private Singleton()
            {
                // Код инициализации объекта Singleton
            }
            // Открытый статический метод, возвращающий объект Singleton (создавая его при необходимости)
            public static Singleton GetSingleton()
            {
                // Если объект Singleton уже создан, возвращаем его
                if (s_value != null) 
                { 
                    return s_value;
                }
                Monitor.Enter(s_lock);
                // Если не создан, позволяем одному потоку сделать это
                if (s_value == null)
                {
                    // Если объекта все еще нет, создаем его
                    Singleton temp = new Singleton();
                    // Сохраняем ссылку в переменной s_value (см. обсуждение далее)
                    Volatile.Write(ref s_value, temp);
                }
                Monitor.Exit(s_lock);
                // Возвращаем ссылку на объект Singleton
                return s_value;
            }
        }

        internal sealed class Singleton2
        {
            private static Singleton2 s_value = new Singleton2();
            // Закрытый конструктор не дает коду вне данного класса создавать экземпляры
            private Singleton2()
            {
                // Код инициализации объекта Singleton
            }
            // Открытый статический метод, возвращающий объект Singleton (и создающий его, если это нужно)
            public static Singleton2 GetSingleton()
            { 
                return s_value; 
            }
        }

        internal sealed class Singleton3
        {
            private static Singleton3 s_value = null;
            // Закрытый конструктор не дает коду вне данного класса создавать экземпляры
            private Singleton3()
            {
                // Код инициализации объекта Singleton
            }
            // Открытый статический метод, возвращающий объект Singleton (и создающий его, если это нужно)
            public static Singleton3 GetSingleton()
            {
                if (s_value != null)
                { 
                    return s_value;
                }
                // Создание нового объекта Singleton и превращение его в корень, если этого еще не сделал другой поток
                Singleton3 temp = new Singleton3();
                Interlocked.CompareExchange(ref s_value, temp, null);
                // При потере этого потока второй объект Singleton утилизируется сборщиком мусора
                return s_value; // Возвращение ссылки на объект
            }
        }

        public void Lazy()
        {
            Lazy<String> s = new Lazy<String>(() => DateTime.Now.ToLongTimeString(), LazyThreadSafetyMode.PublicationOnly);
            Console.WriteLine(s.IsValueCreated); // Возвращается false, так как запроса к Value еще не было
            Console.WriteLine(s.Value); // Вызывается этот делегат
            Console.WriteLine(s.IsValueCreated); // Возвращается true, так как был запрос к Value
            Thread.Sleep(10000); // Ждем 10 секунд и снова выводим время
            Console.WriteLine(s.Value); // Теперь делегат НЕ вызывается, результат прежний

            //public class Lazy<T>
            //{ 
            //     public override string ToString();
            //}
        }
        #endregion

        #region Паттерн условной переменной
        internal sealed class ConditionVariablePattern
        {
            private readonly Object m_lock = new Object();
            private Boolean m_condition = false;
            
            public void Thread1()
            {
                Monitor.Enter(m_lock); // Взаимоисключающая блокировка
                // "Атомарная" проверка сложного условия блокирования
                while (!m_condition)
                {
                    // Если условие не соблюдается, ждем, что его поменяет другой поток
                    Monitor.Wait(m_lock); // На время снимаем блокировку, чтобы другой поток мог ее получить
                }
                // Условие соблюдено, обрабатываем данные...
                Monitor.Exit(m_lock); // Снятие блокировки
            }
            
            public void Thread2()
            {
                Monitor.Enter(m_lock); // Взаимоисключающая блокировка. Обрабатываем данные и изменяем условие...
                m_condition = true;
                Monitor.Pulse(m_lock); // Будим одного ожидающего ПОСЛЕ отмены блокировки
                Monitor.PulseAll(m_lock); // Будим всех ожидающих ПОСЛЕ отмены блокировки
                Monitor.Exit(m_lock); // Снятие блокировки
            }
        }

        internal sealed class SynchronizedQueue<T>
        {
            private readonly Object m_lock = new Object();
            private readonly Queue<T> m_queue = new Queue<T>();
            
            public void Enqueue(T item)
            {
                Monitor.Enter(m_lock);
                // После постановки элемента в очередь пробуждаем один/все ожидающие потоки
                m_queue.Enqueue(item);
                Monitor.PulseAll(m_lock);
                Monitor.Exit(m_lock);
            }
            
            public T Dequeue()
            {
                Monitor.Enter(m_lock);
                // Выполняем цикл, пока очередь не опустеет (условие)
                while (m_queue.Count == 0)
                {
                    Monitor.Wait(m_queue);
                }
                // Удаляем элемент из очереди и возвращаем его на обработку
                T item = m_queue.Dequeue();
                Monitor.Exit(m_lock);
                return item;
            }
        }
        #endregion

        #region Асинхронная синхронизация
        public static void ConcurrentExclusiveSchedulerDemo()
        {
            var cesp = new ConcurrentExclusiveSchedulerPair();
            var tfExclusive = new TaskFactory(cesp.ExclusiveScheduler);
            var tfConcurrent = new TaskFactory(cesp.ConcurrentScheduler);
            for (Int32 operation = 0; operation < 5; operation++)
            {
                var exclusive = operation < 2; // Для демонстрации создаются 2 монопольных и 3 параллельных задания
                (exclusive ? tfExclusive : tfConcurrent).StartNew(() => {
                    Console.WriteLine("{0} access", exclusive ? "exclusive" : "concurrent");
                    // TODO: Здесь выполняется монопольная запись или параллельное чтение...
                });
            }
        }

        public enum OneManyMode { Exclusive, Shared }

        private static async Task AccessResourceViaAsyncSynchronization(AsyncOneManyLock asyncLock)
        {
            // TODO: Здесь выполняется любой код...
            // Передайте OneManyMode.Exclusive или OneManyMode.Shared в зависимости от нужного параллельного доступа
            //await asyncLock.AcquireAsync(OneManyMode.Shared); 
            // Запросить общий доступ
            // Когда управление передается в эту точку, потоки, выполняющие запись в ресурс, отсутствуют; другие потоки могут читать данные
            // TODO: Чтение из ресурса...
            // Завершив работу с ресурсом, снимаем блокировку, чтобы ресурс стал доступным для других потоков.
            asyncLock.Release();
            // TODO: Здесь выполняется любой код...
        }

        public sealed class AsyncOneManyLock
        {
            #region Lock code
            private SpinLock m_lock = new SpinLock(true); // Не используем readonly с SpinLock

            private void Lock() 
            { 
                Boolean taken = false; 
                m_lock.Enter(ref taken); 
            }

            private void Unlock() 
            { 
                m_lock.Exit(); 
            }
            #endregion

            #region Lock state and helper methods
            private Int32 m_state = 0;

            private Boolean IsFree { get { return m_state == 0; } }

            private Boolean IsOwnedByWriter { get { return m_state == 1; } }

            private Boolean IsOwnedByReaders { get { return m_state > 0; } }

            private Int32 AddReaders(Int32 count) { return m_state += count; }

            private Int32 SubtractReader() 
            { 
                return m_state; 
            }

            private void MakeWriter() 
            { 
                m_state = 1; 
            }

            private void MakeFree() 
            { 
                m_state = 0; 
            }
            #endregion

            // Для отсутствия конкуренции (с целью улучшения производительности и сокращения затрат памяти)
            private readonly Task m_noContentionAccessGranter;

            // Каждый ожидающий поток записи пробуждается через свой объект TaskCompletionSource, находящийся в очереди
            private readonly Queue<TaskCompletionSource<Object>> m_qWaitingWriters = new Queue<TaskCompletionSource<Object>>();

            // Все ожидающие потоки чтения пробуждаются по одному объекту TaskCompletionSource
            private TaskCompletionSource<Object> m_waitingReadersSignal = new TaskCompletionSource<Object>();
            private Int32 m_numWaitingReaders = 0;

            public AsyncOneManyLock()
            {
                m_noContentionAccessGranter = Task.FromResult<Object>(null);
            }

            public Task WaitAsync(OneManyMode mode)
            {
                Task accressGranter = m_noContentionAccessGranter; // Предполагается отсутствие конкуренции
                Lock();
                switch (mode)
                {
                    case OneManyMode.Exclusive:
                        if (IsFree)
                        {
                            MakeWriter(); // Без конкуренции
                        }
                        else
                        {
                            // Конкуренция: ставим в очередь новое задание записи
                            var tcs = new TaskCompletionSource<Object>();
                            m_qWaitingWriters.Enqueue(tcs);
                            accressGranter = tcs.Task;
                        }
                        break;
                    case OneManyMode.Shared:
                        if (IsFree || (IsOwnedByReaders && m_qWaitingWriters.Count == 0))
                        {
                            AddReaders(1); // Отсутствие конкуренции
                        }
                        else
                        { 
                            // Конкуренция. Увеличиваем количество ожидающих заданий чтения
                                                                         // когда срабатывает колбэк?
                            accressGranter = m_waitingReadersSignal.Task.ContinueWith(t => t.Result);
                        }
                        break;
                }
                Unlock();
                return accressGranter;
            }

            public void Release()
            {
                TaskCompletionSource<Object> accessGranter = null;
                Lock();

                if (IsOwnedByWriter)
                {
                    MakeFree(); // Ушло задание записи
                }
                else 
                { 
                    SubtractReader(); // Ушло задание чтения
                }

                if (IsFree)
                {
                    // Если ресурс свободен, пробудить одно ожидающее задание записи или все задания чтения
                    if (m_qWaitingWriters.Count > 0)
                    {
                        MakeWriter();
                        accessGranter = m_qWaitingWriters.Dequeue();
                    }
                    else if (m_numWaitingReaders > 0)
                    {
                        AddReaders(m_numWaitingReaders);
                        m_numWaitingReaders = 0;
                        accessGranter = m_waitingReadersSignal;
                        // Создание нового объекта TCS для будущих заданий, которым придется ожидать
                        m_waitingReadersSignal = new TaskCompletionSource<Object>();
                    }
                }
                Unlock();
                // Пробуждение задания чтения/записи вне блокировки снижает вероятность конкуренции и повышает производительность
                if (accessGranter != null) 
                { 
                    accessGranter.SetResult(null); 
                }
            }
        }
        #endregion

        #region Классы коллекций для параллельного доступа
        public void BlockingCollection() 
        {
            var bl = new BlockingCollection<Int32>(new ConcurrentQueue<Int32>());
            // Поток пула получает элементы
            ThreadPool.QueueUserWorkItem(ConsumeItems, bl);
            // Добавляем в коллекцию 5 элементов
            for (Int32 item = 0; item < 5; item++)
            {
                Console.WriteLine("Producing: " + item);
                bl.Add(item);
            }
            // Информируем поток-потребитель, что больше элементов не будет
            bl.CompleteAdding();
            Console.ReadLine(); // Для целей тестирования
        }

        private static void ConsumeItems(Object o)
        {
            var bl = (BlockingCollection<Int32>)o;
            // Блокируем до получения элемента, затем обрабатываем его
            foreach (var item in bl.GetConsumingEnumerable())
            {
                Console.WriteLine("Consuming: " + item);
            }
            // Коллекция пуста и там больше не будет элементов
            Console.WriteLine("All items have been consumed");
        }

        /*
            Producing: 0
            Producing: 1
            Producing: 2
            Producing: 3
            Producing: 4
            Consuming: 0
            Consuming: 1
            Consuming: 2
            Consuming: 3
            Consuming: 4
            All items have been consumed

            Если вы попробуете запустить этот код, строчки Producing (производство)
            и Consuming (потребление) могут быть перемешаны, но строка All items have been
            consumed (все элементы потреблены) всегда будет замыкать список вывода.
         */
        #endregion

        #region Microsoft
        static Thread thread1, thread2;

        public void ThreadingMicrosoft() 
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("\n*********************\n");
                thread1 = new Thread(ThreadProc);
                thread1.Name = "Thread1";
                thread1.Start();

                thread2 = new Thread(ThreadProc);
                thread2.Name = "Thread2";
                thread2.Start();
                Thread.Sleep(1000);
            }
        }

        static void ThreadProc()
        {
            Console.WriteLine("Current thread: {0}", Thread.CurrentThread.Name);
            if (Thread.CurrentThread.Name == "Thread1" && thread2.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                thread2.Join();
            }
            //Console.WriteLine("----- Sleep -----");            
            Thread.Sleep(500);
            //Console.WriteLine("\nCurrent thread: {0}", Thread.CurrentThread.Name);
            Console.WriteLine("Thread1: {0}", thread1.ThreadState);
            Console.WriteLine("Thread2: {0}\n", thread2.ThreadState);
            //Thread.ThreadState - методы, классы экземплярного, статического Thread
        }
        #endregion
    }
}
