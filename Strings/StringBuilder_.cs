using System;
using System.Globalization;
using System.Text;

namespace Strings
{
    class StringBuilder_
    {
        public StringBuilder_()
        {
            StringBuilder sb = new StringBuilder();
            { 
                String s = sb.AppendFormat("{0} {1}", "Jeffrey", "Richter").Replace(' ', '-').Remove(4, 3).ToString(); 
                Console.WriteLine(s); // "Jeff-Richter"
            }
            IFormatProvider formatProvider = null; // aleek
            NumberFormatInfo nfi = (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
            DateTimeFormatInfo dtfi = (DateTimeFormatInfo)formatProvider.GetFormat(typeof(DateTimeFormatInfo));

            { String s = String.Format("On {0}, {1} is {2} years old.", new DateTime(2012, 4, 22, 14, 35, 5), "Aidan", 9); }
        }

        void _2() 
        {
            // Создаем StringBuilder для операций со строками
            StringBuilder sb = new StringBuilder();

            // Выполняем ряд действий со строками, используя StringBuilder
            sb.AppendFormat("{0} {1}", "Jeffrey", "Richter").Replace(" ", "-");

            // Преобразуем StringBuilder в String, чтобы сделать все символы прописными
            String s = sb.ToString().ToUpper();

            // Очищаем StringBuilder (выделяется память под новый массив Char)
            sb.Length = 0;

            // Загружаем строку с прописными String в StringBuilder и выполняем остальные операции
            sb.Append(s).Insert(8, "Marc-");

            // Преобразуем StringBuilder обратно в String
            s = sb.ToString();

            // Выводим String на экран для пользователя
            Console.WriteLine(s); // "JEFFREY-Marc-RICHTER"
        }
    }
}
