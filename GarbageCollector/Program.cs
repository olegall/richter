using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace GarbageCollector
{
    internal static class GCWatcher
    {
        // ПРИМЕЧАНИЕ. Аккуратнее обращайтесь с типом String из-за интернирования и объектов-представителей MarshalByRefObject
        private readonly static ConditionalWeakTable<Object, NotifyWhenGCd<String>> s_cwt = new ConditionalWeakTable<Object, NotifyWhenGCd<String>>();

        private sealed class NotifyWhenGCd<T>
        {
            private readonly T m_value;

            internal NotifyWhenGCd(T value)
            {
                m_value = value;
            }

            public override string ToString()
            {
                return m_value.ToString();
            }

            ~NotifyWhenGCd()
            {
                Console.WriteLine("GC'd: " + m_value);
            }
        }

        public static T GCWatch<T>(this T @object, String tag) where T : class
        {
            s_cwt.Add(@object, new NotifyWhenGCd<String>(tag));
            return @object;
        }
    }

    class Program
    {
        internal sealed class SomeType
        {
            // Метод финализации
            ~SomeType()
            {
                // Код метода финализации
            }
        }

        internal static class SomeType2
        {
            [DllImport("Kernel32", CharSet = CharSet.Unicode, EntryPoint = "CreateEvent")]
            // Этот прототип неустойчив к сбоям
            private static extern IntPtr CreateEventBad(IntPtr pSecurityAttributes, Boolean manualReset, Boolean initialState, String name);
            
            // Этот прототип устойчив к сбоям
            [DllImport("Kernel32", CharSet = CharSet.Unicode, EntryPoint = "CreateEvent")]
            
            private static extern SafeWaitHandle CreateEventGood(IntPtr pSecurityAttributes, Boolean manualReset, Boolean initialState, String name);

            public static void SomeMethod()
            {
                IntPtr handle = CreateEventBad(IntPtr.Zero, false, false, null);
                SafeWaitHandle swh = CreateEventGood(IntPtr.Zero, false, false, null);
            }
        }

        public abstract class SafeHandleZeroOrMinusOneIsInvalid : SafeHandle
        {
            protected SafeHandleZeroOrMinusOneIsInvalid(Boolean ownsHandle) : base(IntPtr.Zero, ownsHandle)
            {
            }

            public override Boolean IsInvalid
            {
                get
                {
                    if (base.handle == IntPtr.Zero) 
                        return true;

                    if (base.handle == (IntPtr)(-1)) 
                        return true;

                    return false;
                }
            }
        }

        public sealed class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeFileHandle(IntPtr preexistingHandle, Boolean ownsHandle) : base(ownsHandle)
            {
                base.SetHandle(preexistingHandle);
            }

            protected override Boolean ReleaseHandle()
            {
                // Сообщить Windows, что системный ресурс нужно закрыть
                //return Win32Native.CloseHandle(base.handle);
                return true;
            }
        }

        private static void MemoryPressureDemo(Int32 size)
        {
            Console.WriteLine();
            Console.WriteLine("MemoryPressureDemo, size={0}", size);

            // Создание набора объектов с указанием их логического размера
            for (Int32 count = 0; count < 15; count++)
            {
                new BigNativeResource(size);
            }

            // В демонстрационных целях очищаем все
            GC.Collect();
        }

        private sealed class BigNativeResource
        {
            private Int32 m_size;

            public BigNativeResource(Int32 size)
            {
                m_size = size;

                // Пусть уборщик думает, что объект занимает больше памяти
                if (m_size > 0) 
                    GC.AddMemoryPressure(m_size);

                Console.WriteLine("BigNativeResource create.");
            }

            ~BigNativeResource()
            {
                // Пусть уборщик думает, что объект освободил больше памяти
                if (m_size > 0) 
                    GC.RemoveMemoryPressure(m_size);

                Console.WriteLine("BigNativeResource destroy.");
            }
        }

        private static void HandleCollectorDemo()
        {
            Console.WriteLine();
            Console.WriteLine("HandleCollectorDemo");
            for (Int32 count = 0; count < 10; count++) 
            { 
                new LimitedResource(); 
            }

            // В демонстрационных целях очищаем все
            GC.Collect();
        }

        private sealed class LimitedResource
        {
            // Создаем объект HandleCollector и передаем ему указание перейти к очистке,когда в куче появится два или более объекта LimitedResource
            private static HandleCollector s_hc = new HandleCollector("LimitedResource", 2);

            public LimitedResource()
            {
                // Сообщаем HandleCollector, что в кучу добавлен еще один объект LimitedResource
                s_hc.Add();
                Console.WriteLine("LimitedResource create. Count={0}", s_hc.Count);
            }

            ~LimitedResource()
            {
                // Сообщаем HandleCollector, что один объект LimitedResource удален из кучи
                s_hc.Remove();
                Console.WriteLine("LimitedResource destroy. Count={0}", s_hc.Count);
            }
        }

        #region defined in FCL aleek
        //public sealed class WeakReference<T> : ISerializable where T : class
        //{
        //    public WeakReference(T target);
        //    public WeakReference(T target, Boolean trackResurrection);
        //    public void SetTarget(T target);
        //    public Boolean TryGetTarget(out T target);
        //}

        //public sealed class ConditionalWeakTable<TKey, TValue> where TKey : class where TValue : class
        //{
        //    public ConditionalWeakTable();
        //    public void Add(TKey key, TValue value);
        //    public TValue GetValue(TKey key, CreateValueCallback<TKey, TValue> createValueCallback);
        //    public Boolean TryGetValue(TKey key, out TValue value);
        //    public TValue GetOrCreateValue(TKey key);
        //    public Boolean Remove(TKey key);
        //    public delegate TValue CreateValueCallback(TKey key); // Вложенное определение делегата
        //}

        //public sealed class HandleCollector
        //{
        //    public HandleCollector(String name, Int32 initialThreshold);

        //    public HandleCollector(String name, Int32 initialThreshold, Int32 maximumThreshold);

        //    public void Add();
        //    public void Remove();
        //    public Int32 Count { get; }
        //    public Int32 InitialThreshold { get; }
        //    public Int32 MaximumThreshold { get; }
        //    public String Name { get; }
        //}
        #endregion

        internal static class ConditionalWeakTableDemo
        {
            public static void Main_()
            {
                Object o = new Object().GCWatch("My Object created at " + DateTime.Now);
                GC.Collect(); // Оповещение об отправке в мусор не выдается
                GC.KeepAlive(o); // Объект, на который ссылается o, должен существовать
                o = null; // Объект, на который ссылается o, можно уничтожать
                GC.Collect(); // Оповещение об отправке в мусор
                Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            // Создание объекта Timer, вызывающего метод TimerCallback каждые 2000 миллисекунд
            Timer t = new Timer(TimerCallback, null, 0, 2000);

            // Ждем, когда пользователь нажмет Enter
            Console.ReadLine();
            
            // Создание объекта Timer, вызывающего метод TimerCallback каждые 2000 мс
            Timer t2 = new Timer(TimerCallback, null, 0, 2000);
            // Ждем, когда пользователь нажмет Enter
            Console.ReadLine();
            // Создаем ссылку на t после ReadLine (в ходе оптимизации эта строка удаляется)
            t2 = null;

            // Создание объекта Timer, вызывающего метод TimerCallback каждые 2000 мс
            Timer t3 = new Timer(TimerCallback, null, 0, 2000);
            // Ждем, когда пользователь нажмет Enter
            Console.ReadLine();
            // Создаем ссылку на переменную t после ReadLine (t не удаляется уборщиком мусора до возвращения управления методом Dispose)
            t3.Dispose();

            Console.WriteLine("Application is running with server GC=" + GCSettings.IsServerGC);

            // Создание байтов для записи во временный файл
            Byte[] bytesToWrite = new Byte[] { 1, 2, 3, 4, 5 };
            // Создание временного файла
            FileStream fs = new FileStream("Temp.dat", FileMode.Create);
            // Запись байтов во временный файл
            fs.Write(bytesToWrite, 0, bytesToWrite.Length);
            // Удаление временного файла
            File.Delete("Temp.dat"); // Генерируется исключение IOException

            // Создание байтов для записи во временный файл
            Byte[] bytesToWrite2 = new Byte[] { 1, 2, 3, 4, 5 };
            // Создание временного файла
            FileStream fs2 = new FileStream("Temp.dat", FileMode.Create);
            // Запись байтов во временный файл
            fs2.Write(bytesToWrite2, 0, bytesToWrite2.Length);
            // Явное закрытие файла после записи
            fs2.Dispose();
            // Удаление временного файла
            File.Delete("Temp.dat"); // Теперь эта инструкция всегда остается работоспособной

            // Создание байтов для записи во временный файл
            Byte[] bytesToWrite3 = new Byte[] { 1, 2, 3, 4, 5 };
            // Создание временного файла
            FileStream fs3 = new FileStream("Temp.dat", FileMode.Create);
            // Запись байтов во временный файл
            fs3.Write(bytesToWrite, 0, bytesToWrite.Length);
            // Явное закрытие файла после записи
            fs3.Dispose();
            // Попытка записи в файл после закрытия
            fs3.Write(bytesToWrite, 0, bytesToWrite.Length); // Исключение ObjectDisposedException
            // Удаление временного файла
            File.Delete("Temp.dat");

            // Создание байтов для записи во временный файл
            Byte[] bytesToWrite4 = new Byte[] { 1, 2, 3, 4, 5 };
            // Создание временного файла
            FileStream fs4 = new FileStream("Temp.dat", FileMode.Create);
            try
            {
                // Запись байтов во временный файл
                fs4.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
            finally
            {
                // Явное закрытие файла после записи
                if (fs4 != null)
                    fs4.Dispose();
            }
            // Удаление временного файла
            File.Delete("Temp.dat");

            // Создание байтов для записи во временный файл
            Byte[] bytesToWrite5 = new Byte[] { 1, 2, 3, 4, 5 };
            // Создание временного файла
            using (FileStream fs5 = new FileStream("Temp.dat", FileMode.Create))
            {
                // Запись байтов во временный файл
                fs5.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
            // Удаление временного файла
            File.Delete("Temp.dat");

            {
                FileStream fs5 = new FileStream("DataFile.dat", FileMode.Create);
                StreamWriter sw5 = new StreamWriter(fs);
                sw5.Write("Hi there");
                // Следующий вызов метода Dispose обязателен
                sw5.Dispose();
                // ПРИМЕЧАНИЕ. Метод StreamWriter.Dispose закрывает объект FileStream. Вручную закрывать объект FileStream не нужно
            }

            MemoryPressureDemo(0); // 0 вызывает нечастую уборку мусора
            MemoryPressureDemo(10 * 1024 * 1024); // 10 Mбайт вызывают частую уборку мусора
            HandleCollectorDemo();
            /*
                MemoryPressureDemo, size=0
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                MemoryPressureDemo, size=10485760
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource destroy.
                BigNativeResource destroy. продолжение 
                594 Глава 21. Автоматическое управление памятью (уборка мусора)
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource create.
                BigNativeResource create.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                BigNativeResource destroy.
                HandleCollectorDemo
                LimitedResource create. Count=1
                LimitedResource create. Count=2
                LimitedResource create. Count=3
                LimitedResource destroy. Count=3
                LimitedResource destroy. Count=2
                LimitedResource destroy. Count=1
                LimitedResource create. Count=1
                LimitedResource create. Count=2
                LimitedResource destroy. Count=2
                LimitedResource create. Count=2
                LimitedResource create. Count=3
                LimitedResource destroy. Count=3
                LimitedResource destroy. Count=2
                LimitedResource destroy. Count=1
                LimitedResource create. Count=1
                LimitedResource create. Count=2
                LimitedResource destroy. Count=2
                LimitedResource create. Count=2
                LimitedResource destroy. Count=1
                LimitedResource destroy. Count=0             
             */

            //unsafe public static void Go()
            unsafe static void Go()
            {
                // Выделение места под объекты, которые немедленно превращаются в мусор
                for (Int32 x = 0; x < 10000; x++)
                {
                    new Object();
                }
                IntPtr originalMemoryAddress;
                Byte[] bytes = new Byte[1000]; // Располагаем этот массив после мусорных объектов. Получаем адрес в памяти массива Byte[]
                fixed (Byte* pbytes = bytes) 
                { 
                    originalMemoryAddress = (IntPtr)pbytes; 
                }

                // Принудительная уборка мусора. Мусор исчезает, позволяя сжать массив Byte[]
                GC.Collect();

                // Повторное получение адреса массива Byte[] в памяти и сравнение двух адресов
                fixed (Byte* pbytes = bytes)
                {
                    Console.WriteLine("The Byte[] did{0} move during the GC",
                    (originalMemoryAddress == (IntPtr)pbytes) ? " not" : null);
                }
            }
        }

        private static void TimerCallback(Object o)
        {
            // Вывод даты/времени вызова этого метода
            Console.WriteLine("In TimerCallback: " + DateTime.Now);
            // Принудительный вызов уборщика мусора в этой программе
            GC.Collect();
        }

        private static void LowLatencyDemo()
        {
            GCLatencyMode oldMode = GCSettings.LatencyMode;
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;
                // Здесь выполняется код
            }
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }
        }
    }
}
