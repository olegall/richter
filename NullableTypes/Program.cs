using System;

namespace NullableTypes
{
    internal struct Point
    {
        private Int32 m_x, m_y;
        public Point(Int32 x, Int32 y) { m_x = x; m_y = y; }
        public static Boolean operator ==(Point p1, Point p2)
        {
            return (p1.m_x == p2.m_x) && (p1.m_y == p2.m_y);
        }

        public static Boolean operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Nullable<Int32> x = 5;
            Nullable<Int32> y = null;
            Console.WriteLine("x: HasValue={0}, Value={1}", x.HasValue, x.Value);
            Console.WriteLine("y: HasValue={0}, Value={1}", y.HasValue, y.GetValueOrDefault());
            Console.WriteLine("Hello World!");

            Int32? x2 = 5;
            Int32? y2 = null;

            Point? p1 = new Point(1, 1);
            Point? p2 = new Point(2, 2);
            Console.WriteLine("Are points equal? " + (p1 == p2).ToString());
            Console.WriteLine("Are points not equal? " + (p1 != p2).ToString());

            Func<String> f = () => SomeMethod() ?? "Untitled";

            Func<String> f2 = () => {
                var temp = SomeMethod();
                return temp != null ? temp : "Untitled";
            };
            
            String s_ = SomeMethod1() ?? SomeMethod2() ?? "Untitled";
            

            String s;
            var sm1 = SomeMethod1();
            if (sm1 != null) s = sm1;
            else
            {
                var sm2 = SomeMethod2();
                if (sm2 != null) s = sm2;
                else s = "Untitled";
            }

            // После упаковки Nullable<T> возвращается null или упакованный тип T
            Int32? n = null;
            Object o = n; // o равно null
            Console.WriteLine("o is null={0}", o == null); // "True"
            
            n = 5;
            o = n; // o ссылается на упакованный тип Int32
            Console.WriteLine("o's type={0}", o.GetType()); // "System.Int32"

            // Создание упакованного типа Int32
            Object o2 = 5;
            // Распаковка этого типа в Nullable<Int32> и в Int32
            Int32? a = (Int32?)o2; // a = 5
            Int32 b = (Int32)o2; // b = 5
                                // Создание ссылки, инициализированной значением null
            o = null;
            // "Распаковка" ее в Nullable<Int32> и в Int32
            a = (Int32?)o2; // a = null
            b = (Int32)o2; // NullReferenceException

            Int32? x3 = 5;
            // Эта строка выводит "System.Int32", а не "System.Nullable<Int32>"
            Console.WriteLine(x3.GetType());

            Int32? n2 = 5;
            Int32 result = ((IComparable)n2).CompareTo(5); // Компилируется и выполняется
            Console.WriteLine(result); // 0

            Int32 result2 = ((IComparable)(Int32)n2).CompareTo(5); // Громоздкий код
        }

        private static string SomeMethod() 
        {
            return string.Empty;
        }
        
        private static string SomeMethod1() 
        {
            return string.Empty;
        }
        
        private static string SomeMethod2()
        {
            return string.Empty;
        }

        private static void ConversionsAndCasting()
        {
            // Неявное преобразование из типа Int32 в Nullable<Int32>
            Int32? a = 5;

            // Неявное преобразование из 'null' в Nullable<Int32>
            Int32? b = null;

            // Явное преобразование Nullable<Int32> в Int32
            Int32 c = (Int32)a;

            // Прямое и обратное приведение примитивного типа в null-совместимый тип
            Double? d = 5; // Int32->Double? (d содержит 5.0 в виде double)
            Double? e = b; // Int32?->Double? (e содержит null)
        }

        private static void Operators()
        {
            Int32? a = 5;
            Int32? b = null;
            // Унарные операторы (+ ++ - -- ! ~)
            a++; // a = 6
            b = -b; // b = null
                    // Бинарные операторы (+ - * / % & | ^ << >>)
            a = a + 3; // a = 9
            b = b * 3; // b = null;
                       // Операторы равенства (== !=)
            if (a == null) { /* нет */ } else { /* да */ }
            if (b == null) { /* да */ } else { /* нет */ }
            if (a != b) { /* да */ } else { /* нет */ }
            // Операторы сравнения (<> <= >=)
            if (a < b) { /* нет */ } else { /* да */ }
        }

        private static Int32? NullableCodeSize(Int32? a, Int32? b)
        {
            return a + b;
        }

        private static Nullable<Int32> NullableCodeSize(Nullable<Int32> a, Nullable<Int32> b)
        {
            Nullable<Int32> nullable1 = a;
            Nullable<Int32> nullable2 = b;

            if (!(nullable1.HasValue & nullable2.HasValue))
            {
                return new Nullable<Int32>();
            }

            return new Nullable<Int32>(nullable1.GetValueOrDefault() + nullable2.GetValueOrDefault());
        }

        private static void NullCoalescingOperator()
        {
            Int32? b = null;
            // Приведенная далее инструкция эквивалентна следующей:
            // x = (b.HasValue) ? b.Value : 123
            Int32 x = b ?? 123;
            Console.WriteLine(x); // "123"
                                  // Приведенная далее в инструкции строка эквивалентна следующему коду: String temp = GetFilename();
         
            // filename = (temp != null) ? temp : "Untitled";
            String filename = GetFilename() ?? "Untitled";
        }
    }
}
