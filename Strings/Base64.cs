using System;

namespace Strings
{
    class Base64
    {
        public Base64()
        {
            // Получаем набор из 10 байт, сгенерированных случайным образом
            Byte[] bytes = new Byte[10];
            new Random().NextBytes(bytes);

            // Отображаем байты
            Console.WriteLine(BitConverter.ToString(bytes));

            // Декодируем байты в строку в кодировке base-64 и выводим эту строку
            String s = Convert.ToBase64String(bytes);
            Console.WriteLine(s);

            // Кодируем строку в кодировке base-64 обратно в байты и выводим их
            bytes = Convert.FromBase64String(s);

            Console.WriteLine(BitConverter.ToString(bytes));
        }
    }
}