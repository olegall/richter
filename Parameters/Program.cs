using System;
using System.Collections.Generic;
using System.IO;

namespace Parameters
{
    class Program
    {
        private static Int32 s_n = 0;

        private static bool noMoreFilesToProcess;

        private static void M(Int32 x = 9, String s = "A",  DateTime dt = default(DateTime), Guid guid = new Guid())
        //private static void M(Int32 x = 9, String s = "A",  DateTime dt = default, Guid guid = new Guid())
        {
            Console.WriteLine("x={0}, s={1}, dt={2}, guid={3}", x, s, dt, guid);
        }

        // Не делайте так:
        private static String MakePath(String filename = "Untitled")
        {
            return String.Format(@"C:\{0}.txt", filename);
        }

        // Используйте следующее решение:
        //private static String MakePath(String filename = null)
        //{
        //    // Здесь применяется оператор, поддерживающий значение null (??); см. главу 19
        //    return String.Format(@"C:\{0}.txt", filename ?? "Untitled");
        //}

        // Объявление метода:
        private static void M(ref Int32 x) {  }

        private static void ImplicitlyTypedLocalVariables()
        {
            var name = "Jeff";
            ShowVariableType(name); // Вывод: System.String. var n = null; // Ошибка

            var x = (String)null; // Допустимо, хотя и бесполезно. При наведении на var - String
            ShowVariableType(x); // Вывод: System.String

            var numbers = new Int32[] { 1, 2, 3, 4 };
            ShowVariableType(numbers); // Вывод: System.Int32[] Меньше символов при вводе сложных типов

            var collection = new Dictionary<String, Single>() { { "Grant", 4.0f } };

            // Вывод: System.Collections.Generic.Dictionary`2[System.String,System.Single]
            ShowVariableType(collection);

            foreach (var item in collection)
            {
                // Вывод: System.Collections.Generic.KeyValuePair`2
                //[System.String, System.Single]
                ShowVariableType(item);
            }
        }

        private static void ShowVariableType<T>(T t)
        {
            Console.WriteLine(typeof(T));
        }

        private static void GetVal(out Int32 v)
        {
            v = 10; // Этот метод должен инициализировать переменную V
        }

        private static void AddVal(ref Int32 v)
        {
            v += 10; // Этот метод может использовать инициализированный параметр v
        }

        public sealed class Point
        {
            static void Add(Point p) {  }

            static void Add(ref Point p) {  } // в чём отличие? aleek
        }

        //static void Add(out Point p) { } // ошибка. фикс: { p = null; }

        static void StartProcessingFiles(out FileStream fs)
        {
            //fs = new FileStream(...); // в этом методе объект fs должен инициализироваться
            fs = new FileStream(null, FileMode.Open); // в этом методе объект fs должен инициализироваться
        }

        static void ContinueProcessingFiles(ref FileStream fs)
        {
            fs.Close(); // Закрытие последнего обрабатываемого файла. Открыть следующий файл или вернуть null, если файлов больше нет

            if (noMoreFilesToProcess)
                fs = null;
            else
                //fs = new FileStream(...);
                fs = new FileStream(null, FileMode.Open);
        }

        private static void ProcessFiles(ref FileStream fs)
        {
            // Если предыдущий файл открыт, закрываем его
            if (fs != null) 
                fs.Close(); // Закрыть последний обрабатываемый файл. Открыть следующий файл или вернуть null, если файлов больше нет

            if (noMoreFilesToProcess)
                fs = null;
            else
                //fs = new FileStream(...);
                fs = new FileStream(null, FileMode.Open);
        }

        public static void Swap(ref Object a, ref Object b)
        //public static void Swap(Object a, Object b)
        {
            Object t = b;
            b = a;
            a = t;
        }

        internal sealed class SomeType
        {
            public Int32 m_val;
        }

        private static void GetAnObject(out Object o)
        {
            o = new String('X', 100);
        }

        public static void Swap<T>(ref T a, ref T b)
        //public static void Swap<T>(T a, T b)
        {
            T t = b;
            b = a;
            a = t;
        }

        static Int32 Add(params Int32[] values)
        {
            // ПРИМЕЧАНИЕ: при необходимости этот массив можно передать другим методам
            Int32 sum = 0;

            if (values != null)
            {
                for (Int32 x = 0; x < values.Length; x++)
                    sum += values[x];
            }

            return sum;
        }

        private static void DisplayTypes(params Object[] objects)
        {
            if (objects != null)
            {
                foreach (Object o in objects)
                    Console.WriteLine(o.GetType());
            }
        }

        // Рекомендуется в этом методе использовать параметр слабого типа
        public void ManipulateItems<T>(IEnumerable<T> collection) {  }

        // Не рекомендуется в этом методе использовать параметр сильного типа
        public void ManipulateItems<T>(List<T> collection) {  }

        // Рекомендуется в этом методе использовать параметр мягкого типа
        public void ProcessBytes(Stream someStream) {  }

        // Не рекомендуется в этом методе использовать параметр сильного типа
        public void ProcessBytes(FileStream fileStream) {  }

        // Рекомендуется в этом методе использовать сильный тип возвращаемого объекта
        public FileStream OpenFile() { return null; }

        // Не рекомендуется в этом методе использовать слабый тип возвращаемого объекта
        //public Stream OpenFile() { return null; }

        // Гибкий вариант: в этом методе используется мягкий тип возвращаемого объекта
        public IList<String> GetStringCollection() { return null; }

        // Негибкий вариант: в этом методе используется сильный тип возвращаемого объекта
        public List<String> GetStringCollection2() { return null; }

        static void Main(string[] args)
        {
            // 1. Аналогично: M(9, "A", default(DateTime), new Guid());
            M();

            // 2. Аналогично: M(8, "X", default(DateTime), new Guid());
            M(8, "X");

            // 3. Аналогично: M(5, "A", DateTime.Now, Guid.NewGuid());
            M(5, guid: Guid.NewGuid(), dt: DateTime.Now);

            // 4. Аналогично: M(0, "1", default(DateTime), new Guid());
            M(s_n++, s_n++.ToString());

            // 5. Аналогично: String t1 = "2"; Int32 t2 = 3;
            // M(t2, t1, default(DateTime), new Guid());
            M(s: (s_n++).ToString(), x: s_n++);

            /*
                x=9, s=A, dt=1/1/0001 12:00:00 AM, guid=00000000-0000-0000-0000-000000000000
                x=8, s=X, dt=1/1/0001 12:00:00 AM, guid=00000000-0000-0000-0000-000000000000
                x=5, s=A, dt=8/16/2012 10:14:25 PM, guid=d24a59da-6009-4aae-9295-839155811309
                x=0, s=1, dt=1/1/0001 12:00:00 AM, guid=00000000-0000-0000-0000-000000000000
                x=3, s=2, dt=1/1/0001 12:00:00 AM, guid=00000000-0000-0000-0000-000000000000
             */

            // Вызов метода:
            Int32 a = 5;
            M(x: ref a);

            {
                Int32 x; // Инициализация x
                GetVal(out x); // Инициализация x не обязательна
                Console.WriteLine(x); // Выводится 10
            }
            {
                Int32 x = 5; // Инициализация x
                //Int32 x; // ошибка
                AddVal(ref x); // x требуется инициализировать
                Console.WriteLine(x); // Выводится 15
            }
            {
                Int32 x; // x не инициализируется. Следующая строка не компилируется, а выводится сообщение: error CS0165: Use of unassigned local variable 'x'
                //AddVal(ref x);
                //Console.WriteLine(x);
            }

            FileStream fs; // Объект fs не инициализирован. Первый файл открывается для обработки
            
            StartProcessingFiles(out fs);

            // Продолжаем, пока остаются файлы для обработки
            for (; fs != null; ContinueProcessingFiles(ref fs))
            {
                // Обработка файла
                //fs.Read(...);
                fs.Read(null, 0, 0);
            }

            FileStream fs2 = null; // Обязательное присвоение начального значения null. Открытие первого файла для обработки
            ProcessFiles(ref fs2);

            // Продолжаем, пока остаются необработанные файлы
            for (; fs2 != null; ProcessFiles(ref fs2))
            {
                // Обработка файла
                //fs2.Read(...);
                fs.Read(null, 0, 0);
            }

            {
                String s1 = "Jeffrey";
                String s2 = "Richter";
                Swap(ref s1, ref s2);
                Console.WriteLine(s1); // Выводит "Richter"
                Console.WriteLine(s2); // Выводит "Jeffrey"
            }

            {
                String s1 = "Jeffrey";
                String s2 = "Richter";
                // Тип передаваемых по ссылке переменных должен соответствовать ожидаемому
                Object o1 = s1, o2 = s2;
                Swap(ref o1, ref o2);
            
                // Приведение объектов к строковому типу
                s1 = (String)o1;
                s2 = (String)o2;
                Console.WriteLine(s1); // Выводит "Richter"
                Console.WriteLine(s2); // Выводит "Jeffrey"
            }

            SomeType st;
            // Следующая строка выдает ошибку CS1503: Argument '1': cannot convert from 'ref SomeType' to 'ref object'.
            //GetAnObject(out st);
            //Console.WriteLine(st.m_val);

            // Выводит "15"
            Console.WriteLine(Add(new Int32[] { 1, 2, 3, 4, 5 }));
            // Обе строчки выводят "0"
            Console.WriteLine(Add()); // передает новый элемент Int32[0] методу Add
            Console.WriteLine(Add(null)); // передает методу Add значение null, что более эффективно (не выделяется память под массив)

            DisplayTypes(new Object(), new Random(), "Jeff", 5);
        }
    }
}