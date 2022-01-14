using System;
using System.IO;

namespace Arrays
{
    // Определение значимого типа, реализующего интерфейс
    internal struct MyValueType : IComparable
    {
        public Int32 CompareTo(Object obj)
        {
            return 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Int32[] myIntegers; // Объявление ссылки на массив
            myIntegers = new Int32[100]; // Создание массива типа Int32 из 100 элементов

            Control[] myControls; // Объявление ссылки на массив. Что конкретно происходит? Создаётся переменная в стеке с адресом
            myControls = new Control[50]; // Создание массива из 50 ссылок на переменную Control. Что конкретно происходит? Создаётся массив в куче?
            myControls[1] = new Button();
            myControls[2] = new TextBox();
            myControls[3] = myControls[2]; // Два элемента ссылаются на один объект
            
            myControls[46] = new DataGrid();
            myControls[48] = new ComboBox();
            myControls[49] = new Button();

            // Создание двухмерного массива типа Doubles
            Double[,] myDoubles = new Double[10, 20];

            // Создание трехмерного массива ссылок на строки
            String[,,] myStrings = new String[5, 3, 10];
            
            // Создание одномерного массива из массивов типа Point
            Point[][] myPolygons = new Point[3][];

            // myPolygons[0] ссылается на массив из 10 экземпляров типа Point
            myPolygons[0] = new Point[10];

            // myPolygons[1] ссылается на массив из 20 экземпляров типа Point
            myPolygons[1] = new Point[20];

            // myPolygons[2] ссылается на массив из 30 экземпляров типа Point
            myPolygons[2] = new Point[30];

            // вывод точек первого многоугольника
            for (Int32 x = 0; x < myPolygons[0].Length; x++)
                Console.WriteLine(myPolygons[0][x]);

            String[] names = new String[] { "Aidan", "Grant" };

            // Использование локальной переменной неявного типа:
            var names = new String[] { "Aidan", "Grant" };

            // Задание типа массива с помощью локальной переменной неявного типа:
            var names = new[] { "Aidan", "Grant", null };

            // Ошибочное задание типа массива с помощью локальной переменной неявного типа. я - тк есть int
            var names = new[] { "Aidan", "Grant", 123 }; 
            
            String[] names = { "Aidan", "Grant" };

            // Ошибочное использование локальной переменной
            var names = { "Aidan", "Grant" };

            // Применение переменных и массивов неявно заданного типа, а также анонимного типа:
            var kids = new[] { new { Name = "Aidan" }, new { Name = "Grant" } };

            // Пример применения (с другой локальной переменной неявно заданного типа):
            foreach (var kid in kids)
                Console.WriteLine(kid.Name);

            // Создание двухмерного массива FileStream
            FileStream[,] fs2dim = new FileStream[5, 10];

            // Неявное приведение к массиву типа Object
            Object[,] o2dim = fs2dim;

            // Невозможно приведение двухмерного массива к одномерному. 
            // Ошибка компиляции CS0030: невозможно преобразовать тип 'object[*,*]' в 'System.IO.Stream[]'
            Stream[] s1dim = (Stream[])o2dim;

            // Явное приведение к двухмерному массиву Stream
            Stream[,] s2dim = (Stream[,])o2dim;

            // Явное приведение к двухмерному массиву String. Компилируется, но во время выполнения возникает исключение InvalidCastException
            String[,] st2dim = (String[,])o2dim;

            // Создание одномерного массива Int32 (значимый тип)
            Int32[] i1dim = new Int32[5];

            // Невозможно приведение массива значимого типа. Ошибка компиляции CS0030: невозможно преобразовать тип 'int[]' в 'object[]'
            Object[] o1dim = (Object[])i1dim;

            // я - ссылочный тип stream к object можно
            Object[] o1dim2 = (Object[])s1dim;

            // Создание нового массива и приведение элементов к нужному типу при помощи метода Array.Copy
            // Создаем массив ссылок на упакованные элементы типа Int32
            Object[] ob1dim = new Object[i1dim.Length];
            Array.Copy(i1dim, ob1dim, i1dim.Length);

            // Создание массива из 100 элементов значимого типа
            MyValueType[] src = new MyValueType[100];

            // Создание массива ссылок IComparable
            IComparable[] dest = new IComparable[src.Length];

            // Присваивание элементам массива IComparable ссылок на упакованные версии элементов исходного массива
            Array.Copy(src, dest, src.Length);

            String[] sa = new String[100];
            Object[] oa = sa; // oa ссылается на массив элементов типа String
            oa[5] = "Jeff"; // CLR проверяет принадлежность oa к типу String; Проверка проходит успешно
            oa[3] = 5; // CLR проверяет принадлежность oa к типу Int32; Генерируется исключение ArrayTypeMismatchException
        }
    }
}
