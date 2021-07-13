using System;

namespace Generics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initial");
            Wrapper.ValueTypePerfTest();
            Wrapper.ReferenceTypePerfTest();

            Console.WriteLine("\n*** Обобщения в библиотеке FCL ***");
            new GenericsInFCLLibrary().Run();
            
            Console.WriteLine("\n*** Открытые и закрытые типы ***");
            new OpenedAndClosedTypes().Run();
            
            Console.WriteLine("\n*** Обобщенные типы и наследование ***");
            GenericsAndInheritance.SameDataLinkedList();
            GenericsAndInheritance.DifferentDataLinkedList();

            Console.WriteLine("\n*** Обобщенные методы ***");
            GenericMethods.CallingSwap();
            GenericMethods.CallingSwapUsingInference();
            GenericMethods.RunDisplays();
            Console.ReadLine();
        }
    }
}
