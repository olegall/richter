using System;

namespace Generics
{
    internal sealed class GenericType<T>
    {
        private T m_value;

        public GenericType(T value) 
        { 
            m_value = value; 
        }

        public TOutput Converter<TOutput>()
        {
            TOutput result = (TOutput)Convert.ChangeType(m_value, typeof(TOutput));
            return result;
        }
    }

    public class GenericMethods
    {
        public static void CallingSwap()
        {
            Int32 n1 = 1, n2 = 2;
            Swap<Int32>(ref n1, ref n2); // n1 = 2, n2 = 1 - значения поменялись
            //Swap<int>(ref n1, ref n2); // тип значения aleek

            String s1 = "Aidan", s2 = "Grant";
            Swap<String>(ref s1, ref s2); // s1 = Grant, s2 = Aidan - значения поменялись
        }

        public static void CallingSwapUsingInference()
        {
            Int32 n1 = 1, n2 = 2;
            Swap(ref n1, ref n2); // Вызывает Swap<Int32>
            
            String s1 = "Aidan";
            Object s2 = "Grant";
            //Swap(ref s1, ref s2); // Ошибка, ограничение дженерика - строка
        }

        private static void Swap<T>(ref T o1, ref T o2)
        {
            T temp = o1;
            o1 = o2;
            o2 = temp;
        }

        public static void RunDisplays()
        {
            Display("Jeff"); // Вызывает Display(String)
            Display(123); // Вызывает Display<T>(T)
            Display<int>(123); // Вызывает Display<T>(T)
            Display<String>("Aidan"); // Вызывает Display<T>(T). потому что типизируем, срабатывает по перегрузке дженерика
        }

        private static void Display<T>(T o)
        {
            Display(o.ToString()); // Вызывает Display(String)
        }

        private static void Display(String s)
        {
            Console.WriteLine(s);
        }
    }
}
