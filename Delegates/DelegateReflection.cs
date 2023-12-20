using System;
using System.Linq;
using System.Reflection;

namespace Delegates
{
    // Несколько разных определений делегатов
    internal delegate Object TwoInt32s(Int32 n1, Int32 n2);
    internal delegate Object OneString(String s1);

    public static class DelegateReflection
    {
        //public static void Main(String[] args)
        public static void Main_()
        {
            //String[] args = (String[])new object();
            String[] args = new String[] { };

            if (args.Length < 2)
            {
                String usage =
                @"Usage:" +
                "{0} delType methodName [Arg1] [Arg2]" +
                "{0} where delType must be TwoInt32s or OneString" +
                "{0} if delType is TwoInt32s, methodName must be Add or Subtract" +
                "{0} if delType is OneString, methodName must be NumChars or Reverse"
               +
                "{0}" +
                "{0}Examples:" +
                "{0} TwoInt32s Add 123 321" +
                "{0} TwoInt32s Subtract 123 321" +
                "{0} OneString NumChars \"Hello there\"" +
                "{0} OneString Reverse \"Hello there\"";

                Console.WriteLine(usage, Environment.NewLine);
                return;
            }

            // Преобразование аргумента delType в тип делегата
            Type delType = Type.GetType(args[0]);
            if (delType == null)
            {
                Console.WriteLine("Invalid delType argument: " + args[0]);
                return;
            }

            Delegate d;
            try
            {
                // Преобразование аргумента Arg1 в метод
                MethodInfo mi = typeof(DelegateReflection).GetTypeInfo().GetDeclaredMethod(args[1]);

                // Создание делегата, служащего оберткой статического метода
                d = mi.CreateDelegate(delType);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid methodName argument: " + args[1]);
                return;
            }

            // Создание массива, содержащего аргументы, передаваемые методу через делегат
            Object[] callbackArgs = new Object[args.Length /*2*/];
            if (d.GetType() == typeof(TwoInt32s))
            {
                try
                {
                    // Преобразование аргументов типа String в тип Int32
                    for (Int32 a = 2; a < args.Length; a++) 
                    {
                        //callbackArgs[a 2] = Int32.Parse(args[a]); 
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Parameters must be integers.");
                    return;
                }
            }

            if (d.GetType() == typeof(OneString))
            {
                // Простое копирование аргумента типа String
                Array.Copy(args, 2, callbackArgs, 0, callbackArgs.Length);
            }
            try
            {
                // Вызов делегата и вывод результата
                Object result = d.DynamicInvoke(callbackArgs);
                Console.WriteLine("Result = " + result);
            }
            catch (TargetParameterCountException)
            {
                Console.WriteLine("Incorrect number of parameters specified.");
            }
        }

        // Метод обратного вызова, принимающий два аргумента типа Int32
        private static Object Add(Int32 n1, Int32 n2)
        {
            return n1 + n2;
        }
        
        // Метод обратного вызова, принимающий два аргумента типа Int32
        private static Object Subtract(Int32 n1, Int32 n2)
        {
            return n1 - n2;
        }

        // Метод обратного вызова, принимающий один аргумент типа String
        private static Object NumChars(String s1)
        {
            return s1.Length;
        }
        // Метод обратного вызова, принимающий один аргумент типа String
        private static Object Reverse(String s1)
        {
            return new String(s1.Reverse().ToArray());
        }
    }
}