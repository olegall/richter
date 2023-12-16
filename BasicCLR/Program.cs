using System;

namespace BasicCLR
{
    public sealed class SomeLibraryType
    {
        // Предупреждение: возвращаемый тип 'SomeLibrary.SomeLibraryType.Abc()' не является CLS-совместимым
        public UInt32 Abc() { return 0; }

        // Предупреждение: идентификаторы 'SomeLibrary.SomeLibraryType.abc()', отличающиеся только регистром символов, не являются CLS-совместимыми
        public void abc() { }
        //public void ABC() { } // ok

        // Предупреждения нет: закрытый метод
        private UInt32 ABC() { return 0; }
    }

    internal sealed class Test
    {
        // Конструктор
        public Test() { }

        // Финализатор
        ~Test() { }

        // Перегрузка оператора
        public static Boolean operator ==(Test t1, Test t2)
        {
            return true;
        }

        public static Boolean operator !=(Test t1, Test t2)
        {
            return false;
        }

        // Перегрузка оператора
        public static Test operator +(Test t1, Test t2) 
        { 
            return null; 
        }

        // Свойство
        public String AProperty
        {
            get { return null; }
            set { }
        }

        // Индексатор
        public String this[Int32 x]
        {
            get { return null; }
            set { }
        }

        // Событие
        public event EventHandler AnEvent;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}