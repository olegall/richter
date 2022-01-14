using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Strings
{
    class OwnFormat
    {
        void Main()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(new BoldInt32s(), "{0} {1} {2:M}", "Jeff", 123, DateTime.Now);
            Console.WriteLine(sb);
        }

        internal sealed class BoldInt32s : IFormatProvider, ICustomFormatter
        {
            public Object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter)) return this;
                return Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
            }
            
            public String Format(String format, Object arg, IFormatProvider formatProvider)
            {
                String s;
                IFormattable formattable = arg as IFormattable;
                if (formattable == null) s = arg.ToString();
                else s = formattable.ToString(format, formatProvider);
                if (arg.GetType() == typeof(Int32))
                    return "<B>" + s + "</B>";
                return s;
            }
        }

        public StringBuilder AppendFormat(IFormatProvider formatProvider, String format, params Object[] args)
        {
            // Если параметр IFormatProvider передан, выясним, предоставляет ли он объект ICustomFormatter
            ICustomFormatter cf = null;
            if (formatProvider != null)
                cf = (ICustomFormatter)

            formatProvider.GetFormat(typeof(ICustomFormatter));
            // Продолжаем добавлять литеральные символы (не показанные в этом псевдокоде) и замещаемые параметры в массив символов объекта StringBuilder.
            Boolean MoreReplaceableArgumentsToAppend = true;
            while (MoreReplaceableArgumentsToAppend)
            {
            // argFormat ссылается на замещаемую строку форматирования, полученную из параметра format
                String argFormat = /* ... */;
                
                // argObj ссылается на соответствующий элемент параметра-массива args
                Object argObj = /* ... */;
                
                // argStr будет указывать на отформатированную строку, которая добавляется к результирующей строке
                String argStr = null;

                // Если есть специальный объект форматирования, используем его для форматирования аргумента
                if (cf != null)
                    argStr = cf.Format(argFormat, argObj, formatProvider);
                
                // Если специального объекта форматирования нет или он не выполнял форматирование аргумента, попробуем еще что-нибудь
                if (argStr == null)
                {
                    // Выясняем, поддерживает ли тип аргумента дополнительное форматирование
                    IFormattable formattable = argObj as IFormattable;
                    if (formattable != null)
                    {
                        // Да; передаем методу интерфейса для этого типа строку форматирования и класс-поставщик
                        argStr = formattable.ToString(argFormat, formatProvider);
                    }
                    else
                    {
                        // Нет; используем общий формат с учетом региональных стандартов потока
                        if (argObj != null) 
                            argStr = argObj.ToString();
                        else 
                            argStr = String.Empty;
                    }
                }

                // Добавляем символы из argStr в массив символов (поле - член класса)
                /* ... */
                // Проверяем, есть ли еще параметры, нуждающиеся в форматировании
                MoreReplaceableArgumentsToAppend = /* ... */;
            }

            return this;
        }
    }
}
