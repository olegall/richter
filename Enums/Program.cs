using System;
using System.IO;
using System.Reflection;

namespace Enums
{
    class Program
    {
        internal enum Color
        {
            White, // Присваивается значение 0
            Red, // Присваивается значение 1
            Green, // Присваивается значение 2
            Blue, // Присваивается значение 3
            Orange // Присваивается значение 4
        }


        // CS0527.cs  
        // compile with: /target:library  
        //public struct clx : int { }   // CS0527 int not an interface  

        interface I1 { }
        interface I2 : I1 { } // Интерфейс может наследовать только от интерфейса

        //internal struct ColorWrong : System.Enum // структура наследует только от интерфейса. это нормально, что ошибка
        //{
        //    // Далее перечислены открытые константы, определяющие символьные имена и значения
        //    public const Color White = (Color)0; // это нормально, что ошибка
        //    public const Color Red = (Color)1;
        //    public const Color Green = (Color)2;
        //    public const Color Blue = (Color)3;
        //    public const Color Orange = (Color)4;

        //    // Далее находится открытое поле экземпляра со значением переменной Color. Код с прямой ссылкой на этот экземпляр невозможен
        //    public Int32 value__;
        //}

        [Flags, Serializable]
        public enum FileAttributes
        {
            ReadOnly = 0x0001,
            Hidden = 0x0002,
            System = 0x0004,
            Directory = 0x0010,
            Archive = 0x0020,
            Device = 0x0040,
            Normal = 0x0080,
            Temporary = 0x0100,
            SparseFile = 0x0200,
            ReparsePoint = 0x0400,
            Compressed = 0x0800,
            Offline = 0x1000,
            NotContentIndexed = 0x2000,
            Encrypted = 0x4000
        }

        [Flags] // Компилятор C# допускает значение "Flags" или "FlagsAttribute"
        internal enum Actions
        {
            None = 0,
            Read = 0x0001,
            Write = 0x0002,
            ReadWrite = Actions.Read | Actions.Write,
            Delete = 0x0004,
            Query = 0x0008,
            Sync = 0x0010
        }

        static void Main(string[] args)
        {
            Color color0 = (Color)0; // white. не обязательно (Color1), если только 0.
            Color color1 = (Color)1; // red
            Color color100 = (Color)100; // 100. ошибки нет, хоть и вышли за пределв enum-а
            //int int1 = (Color1)100;

            Color c = Color.Blue;
            Color[] colors = (Color[])Enum.GetValues(typeof(Color));
            Console.WriteLine("Number of symbols defined: " + colors.Length);
            Console.WriteLine("Value\tSymbol\n-----\t------");

            foreach (Color c2 in colors)
            {
                // Выводим каждый идентификатор в десятичном и общем форматах
                Console.WriteLine("{0,5:D}\t{0:G}", c2);
            }

            _2();
        }

        static void _2()
        {
            // Так как Orange определен как 4, 'c' присваивается значение 4
            Color c = (Color)Enum.Parse(typeof(Color), "orange", true);

            // Так как Brown не определен, генерируется исключение ArgumentException
            //c = (Color)Enum.Parse(typeof(Color), "Brown", false);

            // Создается экземпляр перечисления Color со значением 1
            Enum.TryParse<Color>("1", false, out c); // объявленный c переопределился на red

            // Создается экземпляр перечисления Color со значение 23
            Enum.TryParse<Color>("23", false, out c);

            // Выводит "True", так как в перечислении Color идентификатор Red определен как 1
            Console.WriteLine(Enum.IsDefined(typeof(Color), 1));

            // Выводит "True", так как в перечислении Color идентификатор White определен как 0
            Console.WriteLine(Enum.IsDefined(typeof(Color), "White"));

            // Выводит "False", так как выполняется проверка с учетом регистра
            Console.WriteLine(Enum.IsDefined(typeof(Color), "white"));

            // Выводит "False", так как в перечислении Color отсутствует идентификатор со значением 10
            //Console.WriteLine(Enum.IsDefined(typeof(Color), (Byte)10));

            SetColor((Color)0); // ok
            SetColor((Color)1); // ok
            SetColor(Color.Blue); // ok
            //SetColor((Color)547); // исключение

            String file = Assembly.GetEntryAssembly().Location;
            //FileAttributes attributes = File.GetAttributes(file);
            //Console.WriteLine("Is {0} hidden? {1}", file, (attributes & FileAttributes.Hidden) != 0);

            Actions actions = Actions.Read | Actions.Delete; // 0x0005
            Console.WriteLine(actions.ToString()); // "Read, Delete"

            // Так как Query определяется как 8, 'a' получает начальное значение 8
            Actions a = (Actions)Enum.Parse(typeof(Actions), "Query", true);
            Console.WriteLine(a.ToString()); // "Query". Так как у нас определены и Query, и Read, 'a' получает начальное значение 9

            Enum.TryParse<Actions>("Query, Read", false, out a);
            Console.WriteLine(a.ToString()); // "Read, Query". Создаем экземпляр перечисления Actions enum со значением 28

            a = (Actions)Enum.Parse(typeof(Actions), "28", false);
            Console.WriteLine(a.ToString()); // "Delete, Query, Sync"
        }

        public static void SetColor(Color c)
        {
            if (!Enum.IsDefined(typeof(Color), c))
            {
                throw (new ArgumentOutOfRangeException("c", c, "Invalid Color value."));
            }
            // Задать цвет, как White, Red, Green, Blue или Orange
        }
    }
}
