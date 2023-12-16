using System;
using System.IO;
using System.Threading;

namespace Exceptions
{
    public static class SomeType
    {
        private static Object s_myLockObject = new Object();

        public static void SomeMethod()
        {
            // В случае исключения произойдет ли блокировка? Если да, то этот режим будет невозможно отключить!
            Monitor.Enter(s_myLockObject); // операции над объектом, который между Enter и Exit потокобезопасны

            try
            {
                // Безопасная в отношении потоков операция
            }
            finally
            {
                Monitor.Exit(s_myLockObject);
            }
        }
        // ...
    }

    public static class SomeType2
    {
        private static Object s_myLockObject = new Object();

        public static void SomeMethod()
        {
            Boolean lockTaken = false; // Предполагаем, что блокировки нет

            try
            {
                // Это работает вне зависимости от наличия исключения!
                Monitor.Enter(s_myLockObject, ref lockTaken); // раз передаём ref lockTaken, значит модифицируется?

                // Потокобезопасная операция
            }
            finally
            {
                // Если режим блокировки включен, отключаем его
                if (lockTaken) 
                { 
                    Monitor.Exit(s_myLockObject); 
                }
            }
        }
        // ...
    }

    public sealed class SomeType3
    {
        private void SomeMethod()
        {
            // Открытие файла
            FileStream fs = new FileStream(@"C:\Data.bin ", FileMode.Open);
            try
            {
                // Вывод частного от деления 100 на первый байт файла
                Console.WriteLine(100 / fs.ReadByte());
            }
            finally // не попадёт в finally при исключении в дебаге. в рантайме? aleek
            {
                // В блоке finally размещается код очистки, гарантирующий закрытие файла независимо от того, возникло исключение
                // (например, если первый байт файла равен 0) или нет
                fs.Close();
            }
        }
    }

    internal sealed class SomeType4
    {
        private void SomeMethod()
        {
            using (FileStream fs = new FileStream(@"C:\Data.bin", FileMode.Open))
            {
                // Вывод частного от деления 100 на первый байт файла
                Console.WriteLine(100 / fs.ReadByte());
            }
        }
    }
}