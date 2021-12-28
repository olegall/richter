using System;
using System.Runtime.CompilerServices;

namespace Exceptions
{
    public class ConstrainedExecutionRegion
    {
        public static void Demo1()
        {
            try
            {
                Console.WriteLine("In try");
            }
            finally
            {
                // Неявный вызов статического конструктора Type1
                Type1.M();
            }
        }

        private sealed class Type1
        {
            static Type1()
            {
                // В случае исключения M не вызывается
                Console.WriteLine("Type1's static ctor called");
            }

            public static void M() 
            {
            }
        }

        /*  Вот результат работы этого кода:
        In try
        Type1's static ctor called*/



        public static void Demo2()
        {
            // Подготавливаем код в блоке finally
            RuntimeHelpers.PrepareConstrainedRegions(); // Пространство имен System.Runtime.CompilerServices

            try
            {
                Console.WriteLine("In try");
            }
            finally
            {
                // Неявный вызов статического конструктора Type2
                Type2.M();
            }
        }

        public class Type2
        {
            static Type2()
            {
                Console.WriteLine("Type2's static ctor called");
            }

            // Используем атрибут, определенный в пространстве имен System.Runtime.ConstrainedExecution
            //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public static void M() 
            {
            }
        }
        /*После запуска этой версии кода получаем:
        Type2's static ctor called
        In try
        - наоборот
         */
    }
}