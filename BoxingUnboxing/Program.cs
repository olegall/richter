using BoxingUnboxingPoint3;
using System;
using System.Collections;

namespace BoxingUnboxing
{
    // Объявляем значимый тип
    struct Point
    {
        public Int32 x, y;
    }

    internal static class DynamicDemo
    {
        public static void Main_()
        {
            dynamic value;
            for (Int32 demo = 0; demo < 2; demo++)
            {
                value = (demo == 0) ? (dynamic)5 : (dynamic)"A";
                value = value + value;
                M(value);
            }
        }

        private static void M(Int32 n) { Console.WriteLine("M(Int32): " + n); } // M(Int32) : 10
        private static void M(String s) { Console.WriteLine("M(String): " + s); } // M(String) : AA
    }

    class Program
    {
        static void Main(string[] args)
        {
            ArrayList a = new ArrayList();
            Point p; // Выделяется память для Point (не в куче)

            for (Int32 i = 0; i < 10; i++)
            {
                p.x = p.y = i; // Инициализация членов в нашем значимом типе
                a.Add(p); // Упаковка значимого типа и добавление ссылки в ArrayList
            }

            Int32 x = 5;
            Object o = x; // Упаковка x; o указывает на упакованный объект
            //Int16 y = (Int16)o; // Генерируется InvalidCastException
            Int32 y = (Int32)o; // ok. aleek

            p = (Point)o; // Распаковка o и копирование полей из экземпляра в переменную в стеке
            p.x = 2; // Изменение состояния переменной в стеке
            o = p; // Упаковка p; o ссылается на новый упакованный экземпляр

            // Создаем в стеке два экземпляра Point
            BoxingUnboxingPoint.Point p1 = new BoxingUnboxingPoint.Point(10, 10);
            BoxingUnboxingPoint.Point p2 = new BoxingUnboxingPoint.Point(20, 20);

            // p1 НЕ пакуется для вызова ToString (виртуальный метод)
            Console.WriteLine(p1.ToString()); // "(10, 10)" p1 ПАКУЕТСЯ для вызова GetType (невиртуальный метод)
            Console.WriteLine(p1.GetType()); // "Point"
                                             // p1 НЕ пакуется для вызова CompareTo
                                             // p2 НЕ пакуется, потому что вызван CompareTo(Point)

            Console.WriteLine(p1.CompareTo(p2)); // "-1"
                                                 // p1 пакуется, а ссылка размещается в c
            IComparable c = p1;
            Console.WriteLine(c.GetType()); // "Point"
                                            // p1 НЕ пакуется для вызова CompareTo
                                            // Поскольку в CompareTo не передается переменная Point, вызывается CompareTo(Object), которому нужна ссылка на упакованный Point
                                            // c НЕ пакуется, потому что уже ссылается на упакованный Point

            Console.WriteLine(p1.CompareTo(c)); // "0". c НЕ пакуется, потому что уже ссылается на упакованный. Point p2 ПАКУЕТСЯ, потому что вызывается CompareTo(Object)

            Console.WriteLine(c.CompareTo(p2)); // "-1". c пакуется, а поля копируются в p2

            // все передаваемые параметры в CompareTo - структуры. Какой именно CompareTo вызывается - определяется объектом p1 или интерфейсной переменной c

            p2 = (BoxingUnboxingPoint.Point)c;

            // Убеждаемся, что поля скопированы в p2
            Console.WriteLine(p2.ToString());// "(10, 10)"

            DynamicDemo.Main_();
            _1();
            _2();
            _3();
            _4();
            _5();
            _6();
            _7();
            _8();
            _9();
        }

        static void _1()
        {
            Int32 x = 5;
            Object o = x; // Упаковка x; o указывает на упакованный объект
            Int16 y = (Int16)(Int32)o; // Распаковка, а затем приведение типа
        }

        static void _2()
        {
            Point p;
            p.x = p.y = 1;
            Object o = p; // Упаковка p; o указывает на упакованный объект
            p = (Point)o; // Распаковка o и копирование полей из экземпляра в стек
        }

        static void _3()
        {
            Point p;
            p.x = p.y = 1;
            Object o = p; // Упаковка p; o указывает на упакованный экземпляр
                          // Изменение поля x структуры Point (присвоение числа 2).
        }

        static void _4()
        {
            Int32 v = 5; // Создание неупакованной переменной значимого типа o
            Object o = v; // указывает на упакованное Int32, содержащее 5
            v = 123; // Изменяем неупакованное значение на 123
        }

        static void _5()
        {
            Int32 v = 5; // Создаем неупакованную переменную значимого типа
            Object o = v; // o указывает на упакованную версию v
            v = 123; // Изменяет неупакованный значимый тип на 123
            Console.WriteLine(v); // Отображает "123"
            v = (Int32)o; // Распаковывает и копирует o в v
            Console.WriteLine(v); // Отображает "5"
        }

        static void _6()
        {
            Int32 v = 5; // Создаем переменную упакованного значимого типа
            #if INEFFICIENT
                // При компиляции следующей строки v упакуется
                // три раза, расходуя и время, и память
                Console.WriteLine("{0}, {1}, {2}", v, v, v);
            #else
            // Следующие строки дают тот же результат, но выполняются намного быстрее и расходуют меньше памяти
            Object o = v; // Упакуем вручную v (только единожды) При компиляции следующей строки код упаковки не создается
            Console.WriteLine("{0}, {1}, {2}", o, o, o);
            #endif
        }

        static void _7()
        {
            BoxingUnboxingPoint2.Point p = new BoxingUnboxingPoint2.Point(1, 1);
            Console.WriteLine(p);
            p.Change(2, 2);
            Console.WriteLine(p);
            Object o = p;
            Console.WriteLine(o);
            ((BoxingUnboxingPoint2.Point)o).Change(3, 3);
            Console.WriteLine(o);
        }

        static void _8()
        {
            BoxingUnboxingPoint3.Point p = new BoxingUnboxingPoint3.Point(1, 1);
            Console.WriteLine(p);
            p.Change(2, 2);
            Console.WriteLine(p);
            Object o = p;
            Console.WriteLine(o);
            ((BoxingUnboxingPoint3.Point)o).Change(3, 3);
            Console.WriteLine(o);

            // p упаковывается, упакованный объект изменяется и освобождается
            ((IChangeBoxedPoint)p).Change(4, 4);
            Console.WriteLine(p);

            // Упакованный объект изменяется и выводится
            ((IChangeBoxedPoint)o).Change(5, 5);
            Console.WriteLine(o);
        }

        static void _9() 
        {
            dynamic d = 123;
            var x = (Int32)d; // Конвертация: 'var x' одинаково с 'Int32 x'
            var dt = new DateTime(d); // Создание: 'var dt' одинаково с 'DateTime dt'
        }
    }
}