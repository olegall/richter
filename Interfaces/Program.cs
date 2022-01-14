using System;
using System.Collections.Generic;

namespace Interfaces
{
    public interface IDisposable
    {
        void Dispose();
    }

    public interface IEnumerable
    {
        IEnumerator GetEnumerator();
    }

    public interface IEnumerable<T> : IEnumerable
    {
        IEnumerator<T> GetEnumerator();
    }

    public interface ICollection<T> : IEnumerable<T>, IEnumerable
    {
        void Add(T item);
        void Clear();
        Boolean Contains(T item);
        void CopyTo(T[] array, Int32 arrayIndex);
        Boolean Remove(T item);
        Int32 Count { get; } // Свойство только для чтения
        Boolean IsReadOnly { get; } // Свойство только для чтения
    }

    public interface IComparable<T>
    {
        Int32 CompareTo(T other);
    }

    // Объект Point является производным от System.Object и реализует IComparable<T> в Point
    public sealed class Point : IComparable<Point>
    {
        private Int32 m_x, m_y;

        public Point(Int32 x, Int32 y)
        {
            m_x = x;
            m_y = y;
        }

        // Этот метод реализует IComparable<T> в Point
        public Int32 CompareTo(Point other)
        {
            return Math.Sign(Math.Sqrt(m_x * m_x + m_y * m_y) - Math.Sqrt(other.m_x * other.m_x + other.m_y * other.m_y));
        }

        public override String ToString()
        {
            return String.Format("({0}, {1})", m_x, m_y);
        }
    }

    // Этот класс является производным от Object и реализует IDisposable
    internal class Base : IDisposable
    {
        // Этот метод неявно запечатан и его нельзя переопределить
        public void Dispose()
        {
            Console.WriteLine("Base's Dispose");
        }
    }
    
    // Этот класс наследует от Base и повторно реализует IDisposable
    internal class Derived : Base, IDisposable
    {
        // Этот метод не может переопределить Dispose из Base.
        // Ключевое слово 'new' указывает на то, что этот метод повторно реализует метод Dispose интерфейса IDisposable
        new public void Dispose()
        {
            Console.WriteLine("Derived's Dispose");
            // ПРИМЕЧАНИЕ: следующая строка кода показывает, как вызвать реализацию базового класса (если нужно) base.Dispose();
        }
    }

    internal sealed class SimpleType : IDisposable
    {
        public void Dispose() 
        { 
            Console.WriteLine("Dispose"); 
        }
    }

    internal sealed class SimpleType : IDisposable
    {
        public void Dispose() 
        { 
            Console.WriteLine("public Dispose"); 
        }

        void IDisposable.Dispose() 
        { 
            Console.WriteLine("IDisposable Dispose"); 
        }
    }

    // Этот класс реализует обобщенный интерфейс IComparable<T> дважды
    public sealed class Number : IComparable<Int32>, IComparable<String>
    {
        private Int32 m_val = 5;

        // Этот метод реализует метод CompareTo интерфейса IComparable<Int32>
        public Int32 CompareTo(Int32 n)
        {
            return m_val.CompareTo(n);
        }

        // Этот метод реализует метод CompareTo интерфейса IComparable<String>
        public Int32 CompareTo(String s)
        {
            return m_val.CompareTo(Int32.Parse(s));
        }
    }
    
    //public sealed class Number<T> : IComparable<T> // я
    //{
    //    private Int32 m_val = 5;

    //    // Этот метод реализует метод CompareTo интерфейса IComparable<Int32>
    //    public Int32 CompareTo(Int32 n)
    //    {
    //        return m_val.CompareTo(n);
    //    }

    //    // Этот метод реализует метод CompareTo интерфейса IComparable<String>
    //    public Int32 CompareTo(String s)
    //    {
    //        return m_val.CompareTo(Int32.Parse(s));
    //    }
    //}

    public static class SomeType
    {
        private static void Test()
        {
            Int32 x = 5;
            Guid g = new Guid();

            // Компиляция этого вызова M выполняется без проблем, поскольку Int32 реализует и IComparable, и IConvertible
            M(x);

            // Компиляция этого вызова M приводит к ошибке, поскольку Guid реализует IComparable, но не реализует IConvertible
            M(g);
        }

        // Параметр T типа M ограничивается только теми типами, которые реализуют оба интерфейса: IComparable И IConvertible
        private static Int32 M<T>(T t) where T : IComparable, IConvertible
        {
            return 0;
        }
    }

    public interface IWindow
    {
        Object GetMenu();
    }

    public interface IRestaurant
    {
        Object GetMenu();
    }

    // Этот тип является производным от System.Object и реализует интерфейсы IWindow и IRestaurant
    public sealed class MarioPizzeria : IWindow, IRestaurant
    {
        // Реализация метода GetMenu интерфейса IWindow
        Object IWindow.GetMenu() { return null; }

        // Реализация метода GetMenu интерфейса IRestaurant
        Object IRestaurant.GetMenu() { return null; }

        // Метод GetMenu (необязательный), не имеющий отношения к интерфейсу
        public Object GetMenu() { return null; }
    }

    internal class Base : IComparable
    {
        // Явная реализация интерфейсного метода (EIMI)
        Int32 IComparable.CompareTo(Object o)
        {
            Console.WriteLine("Base's CompareTo");
            return 0;
        }
    }

    internal sealed class Derived : Base, IComparable
    {
        // Открытый метод, также являющийся реализацией интерфейса
        public Int32 CompareTo(Object o)
        {
            Console.WriteLine("Derived's CompareTo");

            // Эта попытка вызвать EIMI базового класса приводит к ошибке: "error CS0117: 'Base' does not contain a definition for 'CompareTo'"
            base.CompareTo(o);

            return 0;
        }

        // Открытый метод, который также является реализацией интерфейса
        public Int32 CompareTo(Object o)
        {
            Console.WriteLine("Derived's CompareTo");

            // Эта попытка вызова EIMI базового класса приводит к бесконечной рекурсии
            IComparable c = this;

            c.CompareTo(o);

            return 0;
        }
    }

    internal class Base : IComparable
    {
        // Явная реализация интерфейсного метода (EIMI)
        Int32 IComparable.CompareTo(Object o)
        {
            Console.WriteLine("Base's IComparable CompareTo");
            return CompareTo(o); // Теперь здесь вызывается виртуальный метод
        }
     
        // Виртуальный метод для производных классов (этот метод может иметь любое имя)
        public virtual Int32 CompareTo(Object o)
        {
            Console.WriteLine("Base's virtual CompareTo");
            return 0;
        }
    }

    internal sealed class Derived : Base, IComparable
    {
        // Открытый метод, который также является реализацией интерфейса
        public override Int32 CompareTo(Object o)
        {
            Console.WriteLine("Derived's CompareTo");

            // Теперь можно вызвать виртуальный метод класса Base
            return base.CompareTo(o);
        }
    }

    class Program
    {
        private void SomeMethod()
        {
            Int32 x = 1, y = 2;
            IComparable c = x;
            // CompareTo ожидает Object, но вполне допустимо передать переменную y типа Int32
            c.CompareTo(y); // Выполняется упаковка CompareTo ожидает Object, при передаче "2" (тип String) компиляция выполняется нормально,
                            // но во время выполнения генерируется исключение ArgumentException
            c.CompareTo("2");
        }

        private void SomeMethod()
        {
            Int32 x = 1, y = 2;
            IComparable<Int32> c = x;
            // CompareTo ожидает Object, но вполне допустимо передать переменную y типа Int32
            c.CompareTo(y); // Выполняется упаковка. CompareTo ожидает Int32, передача "2" (тип String) приводит к ошибке компиляции
                            // с сообщением о невозможности привести тип String к Int32

            c.CompareTo("2"); // Ошибка
        }

        //internal struct SomeValueType : IComparable
        //{
        //    private Int32 m_x;
        //    public SomeValueType(Int32 x) { m_x = x; }
        //    public Int32 CompareTo(Object other)
        //    {
        //        return (m_x _((SomeValueType)other).m_x);
        //    }
        //}

        internal struct SomeValueType : IComparable
        {
            private Int32 m_x;
            public SomeValueType(Int32 x) { m_x = x; }
            public Int32 CompareTo(SomeValueType other)
            {
                return (m_x _ other.m_x);
            }
            // ПРИМЕЧАНИЕ: в следующей строке не используется public/private
            Int32 IComparable.CompareTo(Object other)
            {
                return CompareTo((SomeValueType)other);
            }
        }

        static void Main(string[] args)
        {
            Point[] points = new Point[] {
                new Point(3, 3),
                new Point(1, 2),
            };

            // Вызов метода CompareTo интерфейса IComparable<T> объекта Point
            if (points[0].CompareTo(points[1]) > 0)
            {
                Point tempPoint = points[0];
                points[0] = points[1];
                points[1] = tempPoint;
            }

            Console.WriteLine("Points from closest to (0, 0) to farthest:");
            foreach (Point p in points)
                Console.WriteLine(p);

            /************************* Первый пример *************************/
            Base b = new Base();
            // Вызов реализации Dispose в типе b: "Dispose класса Base"
            b.Dispose();
            // Вызов реализации Dispose в типе объекта b: "Dispose класса Base"
            ((IDisposable)b).Dispose();

            /************************* Второй пример ************************/
            Derived d = new Derived();
            // Вызов реализации Dispose в типе d: "Dispose класса Derived"
            d.Dispose();
            // Вызов реализации Dispose в типе объекта d: "Dispose класса Derived"
            ((IDisposable)d).Dispose();

            /************************* Третий пример *************************/
            b = new Derived();
            // Вызов реализации Dispose в типе b: "Dispose класса Base"
            b.Dispose();
            // Вызов реализации Dispose в типе объекта b: "Dispose класса Derived"
            ((IDisposable)b).Dispose();

            // Переменная s ссылается на объект String
            String s = "Jeffrey";

            // Используя переменную s, можно вызывать любой метод, определенный в String, Object, IComparable, ICloneable, IConvertible, IEnumerable и т. д.
            // Переменная cloneable ссылается на тот же объект String
            ICloneable cloneable = s;

            // Используя переменную cloneable, я могу вызвать любой метод, объявленный только в интерфейсе ICloneable (или любой метод, определенный в типе Object)
            // Переменная comparable ссылается на тот же объект String
            IComparable comparable = s;

            // Используя переменную comparable, я могу вызвать любой метод, объявленный только в интерфейсе IComparable (или любой метод, определенный в типе Object)

            // Переменная enumerable ссылается на тот же объект String
            // Во время выполнения можно приводить интерфейсную переменную к интерфейсу другого типа, если тип объекта реализует оба интерфейса
            IEnumerable enumerable = (IEnumerable)comparable;

            // Используя переменную enumerable, я могу вызывать любой метод, объявленный только в интерфейсе IEnumerable (или любой метод, определенный только в типе Object)
            SimpleType st = new SimpleType();

            // Вызов реализации открытого метода Dispose
            st.Dispose();

            // Вызов реализации метода Dispose интерфейса IDisposable
            IDisposable d2 = st;
            d2.Dispose();

            // Разный SimpleType
            /*
               Dispose
               Dispose

               public Dispose
               IDisposable Dispose
            */

            Number n = new Number();
            // Значение n сравнивается со значением 5 типа Int32
            IComparable<Int32> cInt32 = n;
            Int32 result = cInt32.CompareTo(5);

            // Значение n сравнивается со значением "5" типа String
            IComparable<String> cString = n;
            result = cString.CompareTo("5");



            MarioPizzeria mp = new MarioPizzeria();
            // Эта строка вызывает открытый метод GetMenu класса MarioPizzeria
            mp.GetMenu();

            // Эти строки вызывают метод IWindow.GetMenu
            IWindow window = mp;
            window.GetMenu();

            // Эти строки вызывают метод IRestaurant.GetMenu
            IRestaurant restaurant = mp;
            restaurant.GetMenu();



            SomeValueType v = new SomeValueType(0);
            Object o = new Object();
            Int32 n1 = v.CompareTo(v); // Нежелательная упаковка
            n1 = v.CompareTo(o); // Исключение InvalidCastException

            SomeValueType v2 = new SomeValueType(0);
            Object o2 = new Object();
            Int32 n2 = v2.CompareTo(v2); // Без упаковки
            n2 = v2.CompareTo(o2); // Ошибка компиляции
            //n2 = v2.CompareTo((SomeValueType)o2); // ok

            SomeValueType v3 = new SomeValueType(0);
            IComparable c = v3; // Упаковка!
            Object o3 = new Object();
            Int32 n3 = c.CompareTo(v); // Нежелательная упаковка
            n3 = c.CompareTo(o3); // Исключение InvalidCastException

            Int32 x = 5;
            Single s = x.ToSingle(null); // Попытка вызвать метод интерфейса IConvertible

            Int32 x2 = 5;
            Single s2 = ((IConvertible)x).ToSingle(null);
        }
    }
}
