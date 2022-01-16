using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;

// Пространство имен ДОЛЖНО соответствовать имени сборки и быть отличным от "Windows"
namespace WinRT
{
    // [Flags] // Не должно быть для int; обязательно для uint
    public enum WinRTEnum : int
    { // Перечисления должны базироваться
        None, // на типе int или uint
        NotNone
    }

    // Структуры могут содержать только основные типы данных, String и другие структуры. Конструкторы и методы запрещены.
    public struct WinRTStruct
    {
        public Int32 ANumber;
        public String AString;
        public WinRTEnum AEnum; // В действительности просто
    } // 32-разрядное целое

    // В сигнатуре делегатов должны содержаться WinRT-совместимые типы (без BeginInvoke/EndInvoke)
    public delegate String WinRTDelegate(Int32 x);

    // Интерфейсы могут содержать методы, свойства и события, но не могут быть обобщенными.
    public interface IWinRTInterface
    {
        // Nullable<T> продвигается как IReference<T>
        Int32? InterfaceProperty { get; set; }
    }

    // Члены без атрибута [Version(#)] по умолчанию используют версию класса (1) и являются частью одного нижележащего интерфейса COM, создаваемого программой WinMDExp.exe
    [Version(1)]
    // Классы должны быть производными от Object, запечатанными, не обобщенными, должны реализовать только интерфейсы WinRT, а открытые члены должны быть типами WinRT
    public sealed class WinRTClass : IWinRTInterface
    {
        // Открытые поля запрещены
        #region Класс может предоставлять статические методы, свойства и события
        public static String StaticMethod(String s) { return "Returning " + s; }
        public static WinRTStruct StaticProperty { get; set; }

        // В JavaScript параметры 'out' возвращаются в виде объектов; каждый параметр становится свойством наряду с возвращаемым значением
        public static String OutParameters(out WinRTStruct x, out Int32 year)
        {
            x = new WinRTStruct
            {
                AEnum = WinRTEnum.NotNone,
                ANumber = 333,
                AString = "Jeff"
            };
            year = DateTimeOffset.Now.Year;
            return "Grant";
        }
        #endregion

        // Конструктор может получать аргументы, кроме out/ref
        public WinRTClass(Int32? number) { InterfaceProperty = number; }
        public Int32? InterfaceProperty { get; set; }

        // Переопределяться может только метод ToString
        public override String ToString()
        {
            return String.Format("InterfaceProperty={0}",
            InterfaceProperty.HasValue ? InterfaceProperty.Value.ToString() : "(not set)");
        }

        public void ThrowingMethod()
        {
            throw new InvalidOperationException("My exception message");
            // Чтобы выдать исключение с конкретным кодом HRESULT, используйте COMException
            //const Int32 COR_E_INVALIDOPERATION = unchecked((Int32)0x80131509);
            //throw new COMException("Invalid Operation", COR_E_INVALIDOPERATION);
        }
        #region Массивы передаются, возвращаются ИЛИ заполняются; без комбинаций
        public Int32 PassArray([ReadOnlyArray] /* Подразумевается [In] */ Int32[] data)
        {
            return data.Sum();
        }

        public Int32 FillArray([WriteOnlyArray] /* Подразумевается [Out] */ Int32[] data)
        {
            for (Int32 n = 0; n < data.Length; n++) data[n] = n;
            return data.Length;
        }

        public Int32[] ReturnArray()
        {
            return new Int32[] { 1, 2, 3 };
        }
        #endregion

        // Коллекции передаются по ссылке
        public void PassAndModifyCollection(IDictionary<String, Object> collection)
        {
            collection["Key2"] = "Value2"; // Коллекция изменяется "на месте"
        }

        #region Перегрузка методов
        // Перегруженные версии с одинаковым количеством параметров JavaScript считает идентичными
        public void SomeMethod(Int32 x) { }

        [System.Windows.Foundation.Metadata.DefaultOverload] // Метод назначается
        public void SomeMethod(String s) { } // перегрузкой по умолчанию
        #endregion

        #region Автоматическая реализация события
        public event WinRTDelegate AutoEvent;
        public String RaiseAutoEvent(Int32 number)
        {
            WinRTDelegate d = AutoEvent;
            return (d == null) ? "No callbacks registered" : d(number);
        }
        #endregion

        #region Ручная реализация события
        // Закрытое поле для отслеживания зарегистрированных делегатов события
        private EventRegistrationTokenTable<WinRTDelegate> m_manualEvent = null;

        // Ручная реализация методов add и remove
        public event WinRTDelegate ManualEvent
        {
            add
            {
                // Получение существующей таблицы (или создание новой, если таблица еще не инициализирована)
                return EventRegistrationTokenTable<WinRTDelegate>.GetOrCreateEventRegistrationTokenTable(ref m_manualEvent).AddEventHandler(value);
            }
            remove
            {
                EventRegistrationTokenTable<WinRTDelegate>.GetOrCreateEventRegistrationTokenTable(ref m_manualEvent).RemoveEventHandler(value);
            }
        }

        public String RaiseManualEvent(Int32 number)
        {
            WinRTDelegate d = EventRegistrationTokenTable<WinRTDelegate>.GetOrCreateEventRegistrationTokenTable(ref m_manualEvent).InvocationList;
            return (d == null) ? "No callbacks registered" : d(number);
        }

        #endregion
        #region Asynchronous methods
        // Асинхронные методы ДОЛЖНЫ возвращать IAsync[Action|Operation](WithProgress)
        // ПРИМЕЧАНИЕ: другие языки видят DataTimeOffset как Windows.Foundation.DateTime
        public IAsyncOperationWithProgress<DateTimeOffset, Int32> DoSomethingAsync()
        {
            // Используйте методы Run класса System.Runtime.InteropServices.WindowsRuntime.AsyncInfo для вызова закрытого метода, написанного исключительно на управляемом коде.
            return AsyncInfo.Run<DateTimeOffset, Int32>(DoSomethingAsyncInternal);
        }

        // Реализация асинхронной операции на базе закрытого метода с использованием обычных технологий .NET
        private async Task<DateTimeOffset> DoSomethingAsyncInternal(CancellationToken ct, IProgress<Int32> progress)
        {
            for (Int32 x = 0; x < 10; x++)
            {
                // Поддержка отмены и оповещений о ходе выполнения
                ct.ThrowIfCancellationRequested();
                if (progress != null) 
                    progress.Report(x * 10);

                await Task.Delay(1000); // Имитация асинхронных операций
            }

             return DateTimeOffset.Now; // Итоговое возвращаемое значение
        }
        public IAsyncOperation<DateTimeOffset> DoSomethingAsync2()
        {
            // Если отмена и оповещения не нужны, используйте методы расширения AsAsync[Action|Operation] класса System.WindowsRuntimeSystemExtensions
            // (они вызывают AsyncInfo.Run в своей внутренней реализации)
            return DoSomethingAsyncInternal(default(CancellationToken), null).AsAsyncOperation();
        }
        #endregion

        // После распространения версии новые члены следует помечать атрибутом [Version(#)], чтобы программа WinMDExp.exe
        // помещала новые члены в другой интерфейс COM. Это необходимо, поскольку интерфейсы COM должны быть неизменными.
        [Version(2)]
        public void NewMethodAddedInV2() { }
    }
}
