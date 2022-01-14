using System;

namespace Arrays
{
    class Program2
    {
        public static void Main()
        {
            Array a;

            // Создание одномерного массива с нулевым начальным индексом и без элементов
            a = new String[0];
            
            Console.WriteLine(a.GetType()); // "System.String[]". Создание одномерного массива с нулевым начальным индексом и без элементов
            a = Array.CreateInstance(typeof(String), new Int32[] { 0 }, new Int32[] { 0 });

            Console.WriteLine(a.GetType()); // "System.String[]". Создание одномерного массива с начальным индексом 1 и без элементов
            
            a = Array.CreateInstance(typeof(String), new Int32[] { 0 }, new Int32[] { 1 });
            Console.WriteLine(a.GetType()); // "System.String[*]" <-- ВНИМАНИЕ!
            Console.WriteLine();
            
            // Создание двухмерного массива с нулевым начальным индексом и без элементов
            a = new String[0, 0];
            Console.WriteLine(a.GetType()); // "System.String[,]" Создание двухмерного массива с нулевым начальным индексом и без элементов

            a = Array.CreateInstance(typeof(String), new Int32[] { 0, 0 }, new Int32[] { 0, 0 });
            Console.WriteLine(a.GetType()); // "System.String[,]" Создание двухмерного массива с начальным индексом 1 и без элементов
            
            a = Array.CreateInstance(typeof(String), new Int32[] { 0, 0 }, new Int32[] { 1, 1 });
            Console.WriteLine(a.GetType()); // "System.String[,]"
        }
    }
}
