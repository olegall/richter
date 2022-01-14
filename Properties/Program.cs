using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Properties
{
    class Program
    {
        public sealed class Employee
        {
            private String m_Name;
            private Int32 m_Age;

            public String Name
            {
                get { return (m_Name); }
                set { m_Name = value; } // Ключевое слово value идентифицирует новое значение
            }

            public Int32 Age
            {
                get { return (m_Age); }
                set
                {
                    if (value < 0) // Ключевое слово value всегда идентифицирует новое значение
                        throw new ArgumentOutOfRangeException("value", value.ToString(), "The value must be greater than or equal to 0");
                    
                    m_Age = value;
                }
            }
        }

        public sealed class Employee2
        {
            // Это свойство является автоматически реализуемым
            public String Name { get; set; }

            private Int32 m_Age;
            public Int32 Age
            {
                get { return (m_Age); }
                set
                {
                    if (value < 0) // value всегда идентифицирует новое значение
                        throw new ArgumentOutOfRangeException("value", value.ToString(), "The value must be greater than or equal to 0");

                    m_Age = value;
                }
            }
        }

        
        private static String Name
        {
            get { return null; }
            set { }
        }

        static void MethodWithOutParam(out String n) { n = null; }

        public sealed class Classroom
        {
            private List<String> m_students = new List<String>();
            public List<String> Students { get { return m_students; } }

            public Classroom() { }
        }

        public static void M()
        {
            Classroom classroom = new Classroom
            {
                Students = { "Jeff", "Kristin", "Aidan", "Grant" }
            };

            // Вывести имена 4 студентов, находящихся в классе
            foreach (var student in classroom.Students)
                Console.WriteLine(student);
        }

        // Простая форма:
        [Serializable]
        public class Tuple<T1>
        {
            private T1 m_Item1;
            public Tuple(T1 item1) { m_Item1 = item1; }
            public T1 Item1 { get { return m_Item1; } }
        }

        // Сложная форма:
        [Serializable]
        public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>
        {
            private T1 m_Item1; 
            private T2 m_Item2;
            private T3 m_Item3; 
            private T4 m_Item4;
            private T5 m_Item5; 
            private T6 m_Item6;
            private T7 m_Item7; 
            private TRest m_Rest;
            
            public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest t)
            {
                m_Item1 = item1; 
                m_Item2 = item2; 
                m_Item3 = item3; 
                m_Item4 = item4; 
                m_Item5 = item5; 
                m_Item6 = item6; 
                m_Item7 = item7; 
                m_Rest = rest;
            }

            public T1 Item1 { get { return m_Item1; } }
            public T2 Item2 { get { return m_Item2; } }
            public T3 Item3 { get { return m_Item3; } }
            public T4 Item4 { get { return m_Item4; } }
            public T5 Item5 { get { return m_Item5; } }
            public T6 Item6 { get { return m_Item6; } }
            public T7 Item7 { get { return m_Item7; } }
            public TRest Rest { get { return m_Rest; } }
        }

        // Возвращает минимум в Item1 и максимум в Item2
        private static Tuple<Int32, Int32> MinMax(Int32 a, Int32 b)
        {
            return new Tuple<Int32, Int32>(Math.Min(a, b), Math.Max(a, b));
        }

        // Пример вызова метода и использования Tuple
        private static void TupleTypes()
        {
            var minmax = MinMax(6, 2);
            Console.WriteLine("Min={0}, Max={1}", minmax.Item1, minmax.Item2); // Min=2, Max=6
        }

        //// Возвращает минимум в Item1 и максимум в Item2
        //private static Tuple<Int32, Int32> MinMax(Int32 a, Int32 b)
        //{
        //    return Tuple.Create(Math.Min(a, b), Math.Max(a, b)); // Упрощенный синтаксис
        //}

        public sealed class SomeType
        {
            // Определяем метод доступа get_Item
            //[IndexerName("Foo")] // я
            public Int32 this[Boolean b]
            {
                get { return 0; }
            }

            // Определяем метод доступа get_Jeff
            [IndexerName("Jeff")]
            public String this[Boolean b]
            {
                get { return null; }
            }

            //public String this[int a]
            //{
            //    get { return string.Empty; }
            //}

            // error CS0111: Class 'SomeType' already defines a member called 'this' with the same parameter types
        }

        public class SomeType2
        {
            private String m_name;
            public String Name
            {
                get { return m_name; }
                protected set { m_name = value; }
            }
        }

        static void Main(string[] args)
        {
            Employee e = new Employee();
            e.Name = "Jeffrey Richter"; // "Задать" имя сотрудника
            String EmployeeName = e.Name; // "Получить" имя сотрудника
            e.Age = 41; // "Задать" возраст сотрудника
            e.Age = -5; // Вброс исключения ArgumentOutOfRangeException
            Int32 EmployeeAge = e.Age; // "Получить" возраст сотрудника

            // При попытке скомпилировать следующую строку компилятор вернет сообщение об ошибке:
            // error CS0206: A property or indexer may not be passed as an out or ref parameter.
            MethodWithOutParam(out Name);

            Employee e2 = new Employee() { Name = "Jeff", Age = 45 };

            Employee e3 = new Employee();
            e3.Name = "Jeff";
            e3.Age = 45;

            String s = new Employee() { Name = "Jeff", Age = 45 }.ToString().ToUpper();

            var table1 = new Dictionary<String, Int32> {
                { "Jeffrey", 1 }, { "Kristin", 2 }, { "Aidan", 3 }, { "Grant", 4 }
            };

            var table2 = new Dictionary<String, Int32>();
            table2.Add("Jeffrey", 1);
            table2.Add("Kristin", 2);
            table2.Add("Aidan", 3);
            table2.Add("Grant", 4);

            // Определение типа, создание сущности и инициализация свойств
            var o1 = new { Name = "Jeff", Year = 1964 };

            // Вывод свойств на консоль
            Console.WriteLine("Name={0}, Year={1}", o1.Name, o1.Year); // Выводит: Name=Jeff, Year=1964

            // Это работает, так как все объекты имею один анонимный тип
            var people = new[] {
                o1, // См. ранее в этом разделе
                new { Name = "Kristin", Year = 1970 },
                new { Name = "Aidan", Year = 2003 },
                new { Name = "Grant", Year = 2008 }
            };

            // Организация перебора массива анонимных типов (ключевое слово var обязательно).
            foreach (var person in people)
                Console.WriteLine("Person={0}, Year={1}", person.Name, person.Year);

            String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var query =
                from pathname in Directory.GetFiles(myDocuments)
                let LastWriteTime = File.GetLastWriteTime(pathname)
                where LastWriteTime > (DateTime.Now - TimeSpan.FromDays(7))
                orderby LastWriteTime
                select new { Path = pathname, LastWriteTime };

            foreach (var file in query)
                Console.WriteLine("LastWriteTime={0}, Path={1}", file.LastWriteTime, file.Path);

            var t = Tuple.Create(0, 1, 2, 3, 4, 5, 6, Tuple.Create(7, 8));
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", t.Item1, t.Item2, t.Item3, t.Item4, t.Item5);

            dynamic e4 = new System.Dynamic.ExpandoObject();
            e4.x = 6; // Добавление свойства 'x' типа Int32 со значением 6
            e4.y = "Jeff"; // Добавление свойства 'y' строкового типа со значением "Jeff"
            e4.z = null; // Добавление свойста 'z' объекта со значением null
                        
            // Просмотр всех свойств и других значений
            foreach (var v in (IDictionary<String, Object>)e4)
                Console.WriteLine("Key={0}, V={1}", v.Key, v.Value);

            // Удаление свойства 'x' и его значения
            var d = (IDictionary<String, Object>)e4;
            d.Remove("x");

            // Выделить массив BitArray, который может хранить 14 бит
            BitArray ba = new BitArray(14);

            // Установить все четные биты вызовом метода доступа set
            for (Int32 x = 0; x < 14; x++)
            {
                ba[x] = (x % 2 == 0);
            }

            // Вывести состояние всех битов вызовом метода доступа get
            for (Int32 x = 0; x < 14; x++)
            {
                Console.WriteLine("Bit " + x + " is " + (ba[x] ? "On" : "Off"));
            }
        }
    }
}
