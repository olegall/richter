using System;
using System.Collections.Generic;
using System.Text;

namespace Methods
{
    public static class StringBuilderExtensions
    {
        public static Int32 IndexOf(this StringBuilder sb, Char value) // this - текущим классом расширяем StringBuilders
        {
            for (Int32 index = 0; index < sb.Length; index++)
                if (sb[index] == value)
                    return index;

            return -1;
        }
    }

    public static class ExtensionMethodsContainer // обязательно static для метода расширения
    {
        public static void ShowItems<T>(this IEnumerable<T> collection) // обязательно static для метода расширения
        {
            foreach (var item in collection)
                Console.WriteLine(item);
        }

        public static void InvokeAndCatch<TException>(this Action<Object> d, Object o) where TException : Exception // расширяет Action
        {
            try
            {
                d(o);
            }
            catch (TException)
            {
            }
        }
    }

    class ExtensionMethods
    {
        // Определен в пространстве имен System.Runtime.CompilerServices
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
        public sealed class ExtensionAttribute : Attribute
        {
        }

        public ExtensionMethods()
        {
            // Инициализирующая строка
            StringBuilder sb = new StringBuilder("Hello. My name is Jeff.");
            
            // Замена точки восклицательным знаком и получение номера символа в первом предложении (5)
            Int32 index = StringBuilderExtensions.IndexOf(sb.Replace('.', '!'), '!');

            // Замена точки восклицательным знаком
            sb.Replace('.', '!');

            // Получение номера символа в первом предложении (5)
            Int32 index2 = StringBuilderExtensions.IndexOf(sb, '!');

            // Замена точки восклицательным знаком и получение номера символа в первом предложении (5)
            Int32 index3 = sb.Replace('.', '!').IndexOf('!');

            Int32 index4 = sb.IndexOf('X');

            // sb равно null
            StringBuilder sb2 = null;

            // Вызов метода выражения: исключение NullReferenceException НЕ БУДЕТ выдано при вызове IndexOf
            // Исключение NullReferenceException будет вброшено внутри цикла IndexOf
            sb2.IndexOf('X');

            // Вызов экземплярного метода: исключение NullReferenceException БУДЕТ вброшено при вызове Replaces
            sb2.Replace('.', '!');

            // Показывает каждый символ в каждой строке консоли
            // вызывается без аргумента, в сигнатуре он был с this для метода расширения. aleek
            "Grant".ShowItems(); // String реализует IEnumerable, поэтому здесь доступен ShowItems() - здесь и далее

            // Показывает каждую строку в каждой строке консоли
            new[] { "Jeff", "Kristin" }.ShowItems();

            // Показывает каждый Int32 в каждой строчке консоли.
            new List<Int32>() { 1, 2, 3 }.ShowItems();

            Action<Object> action = o => Console.WriteLine(o.GetType()); // Выдает NullReferenceException

            action.InvokeAndCatch<NullReferenceException>(null); // Поглощает NullReferenceException

            // Cоздание делегата Action, ссылающегося на статический метод расширения ShowItems; первый аргумент инициализируется ссылкой на строку "Jeff"
            Action a = "Jeff".ShowItems;
            //Action a = "Jeff".ShowItems();
 
            // Вызов делегата, вызывающего ShowItems и передающего ссылку на строку "Jeff"
            a();
        }
    }
}
