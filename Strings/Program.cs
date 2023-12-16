using System;
using System.Globalization;
using System.Threading;

namespace Strings
{
    class Program
    {
        static void Main(string[] args)
        {
            Double d; // '\u0033' – это "цифра 3"

            d = Char.GetNumericValue('\u0033'); // Параметр '3' даст тот же результат
            Console.WriteLine(d.ToString()); // Выводится "3" '\u00bc' — это "простая дробь одна четвертая ('1/4')"

            d = Char.GetNumericValue('\u00bc');
            Console.WriteLine(d.ToString()); // Выводится "0.25" 'A' — это "Латинская прописная буква A"

            d = Char.GetNumericValue('A');
            Console.WriteLine(d.ToString()); // Выводится "-1"

            Char c;
            Int32 n;
            // Преобразование "число - символ" посредством приведения типов C#
            c = (Char)65;
            Console.WriteLine(c); // Выводится "A"

            n = (Int32)c;
            Console.WriteLine(n); // Выводится "65"

            c = unchecked((Char)(65536 + 65));
            Console.WriteLine(c); // Выводится "A". Преобразование "число - символ" с помощью типа Convert
            
            c = Convert.ToChar(65);
            Console.WriteLine(c); // Выводится "A"

            n = Convert.ToInt32(c);
            Console.WriteLine(n); // Выводится "65". Демонстрация проверки диапазона для Convert

            try
            {
                c = Convert.ToChar(70000); // Слишком много для 16 разрядов
                Console.WriteLine(c); // Этот вызов выполняться НЕ будет
            }
            catch (OverflowException)
            {
                Console.WriteLine("Can't convert 70000 to a Char.");
            }

            // Преобразование "число - символ" с помощью интерфейса IConvertible
            c = ((IConvertible)65).ToChar(null);
            Console.WriteLine(c); // Выводится "A"

            n = ((IConvertible)c).ToInt32(null);
            Console.WriteLine(n); // Выводится "65"

            //String s = new String("Hi there."); // Ошибка

            new Encoding_();
            new Base64();
            new Format();
            new Protected();
            new StringBuilder_();
            new Symbols();
        }

        void Compare1()
        {
            String s1 = "Strasse";
            String s2 = "Straße";
            Boolean eq;
            
            // CompareOrdinal возвращает ненулевое значение
            eq = String.Compare(s1, s2, StringComparison.Ordinal) == 0;
            Console.WriteLine("Ordinal comparison: '{0}' {2} '{1}'", s1, s2, eq ? "==" : "!=");
            
            // Сортировка строк для немецкого языка (de) в Германии (DE)
            CultureInfo ci = new CultureInfo("de-DE");
            
            // Compare возвращает нуль
            eq = String.Compare(s1, s2, true, ci) == 0;
            Console.WriteLine("Cultural comparison: '{0}' {2} '{1}'", s1, s2, eq ? "==" : "!=");
        }
        
        void Compare2()
        {
            String output = String.Empty;
            String[] symbol = new String[] { "<", "=", ">" };
            Int32 x;
            CultureInfo ci;
            
            // Следующий код демонстрирует, насколько отличается результат сравнения строк для различных региональных стандартов
            String s1 = "coté";
            String s2 = "côte";

            // Сортировка строк для французского языка (Франция)
            ci = new CultureInfo("fr-FR");
            x = Math.Sign(ci.CompareInfo.Compare(s1, s2));
            output += String.Format("{0} Compare: {1} {3} {2}", ci.Name, s1, s2, symbol[x + 1]);
            output += Environment.NewLine;

            // Сортировка строк для японского языка (Япония)
            ci = new CultureInfo("ja-JP");
            x = Math.Sign(ci.CompareInfo.Compare(s1, s2));

            output += String.Format("{0} Compare: {1} {3} {2}", ci.Name, s1, s2, symbol[x + 1]);
            output += Environment.NewLine;

            // Сортировка строк по региональным стандартам потока
            ci = Thread.CurrentThread.CurrentCulture;
            x = Math.Sign(ci.CompareInfo.Compare(s1, s2));
            output += String.Format("{0} Compare: {1} {3} {2}", ci.Name, s1, s2, symbol[x + 1]);
            output += Environment.NewLine + Environment.NewLine;

            // Следующий код демонстрирует использование дополнительных возможностей метода CompareInfo.Compare при работе с двумя строками на японском языке
            // Эти строки представляют слово "shinkansen" (название высокоскоростного поезда) в разных вариантах письма: хирагане и катакане
            s1 = " "; // ("\u3057\u3093\u304b\u3093\u305b\u3093")
            s2 = " "; // ("\u30b7\u30f3\u30ab\u30f3\u30bb\u30f3")

                      // Результат сравнения по умолчанию
            ci = new CultureInfo("ja-JP");
            x = Math.Sign(String.Compare(s1, s2, true, ci));
            output += String.Format("Simple {0} Compare: {1} {3} {2}", ci.Name, s1, s2, symbol[x + 1]);
            output += Environment.NewLine;

            // Результат сравнения, который игнорирует тип каны
            CompareInfo compareInfo = CompareInfo.GetCompareInfo("ja-JP");
            x = Math.Sign(compareInfo.Compare(s1, s2, CompareOptions.IgnoreKanaType));
            output += String.Format("Advanced {0} Compare: {1} {3} {2}", ci.Name, s1, s2, symbol[x + 1]);
        }
    }
}
