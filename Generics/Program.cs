using System;

namespace Generics
{
    class Program
    {
        static void Main(string[] args)
        {
            AdditionalRestrictions.CallingConvertIList();

            Console.WriteLine("Initial");
            Wrapper.ValueTypePerfTest();
            Wrapper.ReferenceTypePerfTest();

            Console.WriteLine("\n*** Обобщения в библиотеке FCL ***");
            new GenericsInFCLLibrary();
            
            Console.WriteLine("\n*** Открытые и закрытые типы ***");
            new OpenedAndClosedTypes();
            
            Console.WriteLine("\n*** Обобщенные типы и наследование ***");
            GenericsAndInheritance.SameDataLinkedList();
            GenericsAndInheritance.DifferentDataLinkedList();

            Console.WriteLine("\n*** Обобщенные методы ***");
            GenericMethods.CallingSwap();
            GenericMethods.CallingSwapUsingInference();
            GenericMethods.Main_();
            //Console.ReadLine();

            new IdentificationOfGenerics();
            new VerificationAndRestrictions();
        }
    }
}
