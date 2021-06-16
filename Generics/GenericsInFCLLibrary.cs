using System;

namespace Generics
{
    /// <summary>
    /// Открытые и закрытые типы
    /// </summary>
    class GenericsInFCLLibrary : IRunnable
    {
        public void Run() 
        {
            // Создание и инициализация массива байтов
            Byte[] byteArray = new Byte[] { 5, 1, 4, 2, 3 };
            // Вызов алгоритма сортировки Byte[]
            Array.Sort<Byte>(byteArray);
            // Вызов алгоритма двоичного поиска Byte[]
            Int32 i = Array.BinarySearch<Byte>(byteArray, 1);
            Console.WriteLine(i); // Выводит "0"
        }
    }
}
