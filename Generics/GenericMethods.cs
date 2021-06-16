using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics
{
    internal sealed class GenericType<T>
    {
        private T m_value;
        public GenericType(T value) { m_value = value; }
        public TOutput Converter<TOutput>()
        {
            TOutput result = (TOutput)Convert.ChangeType(m_value, typeof(TOutput));
            return result;
        }
    }

    public class GenericMethods
    {
        private static void Swap<T>(ref T o1, ref T o2)
        {
            T temp = o1;
            o1 = o2;
            o2 = temp;
        }

        public static void CallingSwap()
        {
            Int32 n1 = 1, n2 = 2;
            Console.WriteLine("n1={0}, n2={1}", n1, n2);
            Swap<Int32>(ref n1, ref n2);
            Console.WriteLine("n1={0}, n2={1}", n1, n2);

            String s1 = "Aidan", s2 = "Grant";
            Console.WriteLine("s1={0}, s2={1}", s1, s2);
            Swap<String>(ref s1, ref s2);
            Console.WriteLine("s1={0}, s2={1}", s1, s2);
        }

        public static void CallingSwapUsingInference()
        {
            Int32 n1 = 1, n2 = 2;
            Swap(ref n1, ref n2); // Вызывает Swap<Int32>
            String s1 = "Aidan";
            Object s2 = "Grant";
            //Swap(ref s1, ref s2); // Ошибка, невозможно вывести тип
        }

        private static void Display(String s)
        {
            Console.WriteLine(s);
        }

        private static void Display<T>(T o)
        {
            Display(o.ToString()); // Вызывает Display(String)
        }

        public static void RunDisplays()
        {
            Display("Jeff"); // Вызывает Display(String)
            Display(123); // Вызывает Display<T>(T)
            Display<String>("Aidan"); // Вызывает Display<T>(T)
        }
    }
}
