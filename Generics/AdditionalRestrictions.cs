using System;
using System.Collections.Generic;

namespace Generics
{
    /// <summary>
    /// Дополнительные ограничения
    /// </summary>
    public class AdditionalRestrictions
    {
        private static List<TBase> ConvertIList<T, TBase>(IList<T> list) where T : TBase
        {
            List<TBase> baseList = new List<TBase>(list.Count);

            for (Int32 index = 0; index < list.Count; index++)
            {
                baseList.Add(list[index]);
            }

            return baseList;
        }

        public static void CallingConvertIList()
        {
            // Создает и инициализирует тип List<String> (реализующий IList<String>)
            IList<String> ls = new List<String>();
            ls.Add("A String");

            // Преобразует IList<String> в IList<Object>
            IList<Object> lo = ConvertIList<String, Object>(ls);

            // Преобразует IList<String> в IList<IComparable>
            IList<IComparable> lc = ConvertIList<String, IComparable>(ls);

            // Преобразует IList<String> в IList<IComparable<String>>
            IList<IComparable<String>> lcs = ConvertIList<String, IComparable<String>>(ls);

            // Преобразует IList<String> в IList<String>
            IList<String> ls2 = ConvertIList<String, String>(ls);
            
            // Преобразует IList<String> в IList<Exception>
            //IList<Exception> le = ConvertIList<String, Exception>(ls); // Ошибка
        }
    }
}
