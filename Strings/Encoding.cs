using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strings
{
    class Encoding
    {
        void Main() 
        {
            // Кодируемая строка
            String s = "Hi there.";
            
            // Получаем объект, производный от Encoding, который "умеет" выполнять кодирование и декодирование с использованием UTF-8
            Encoding encodingUTF8 = Encoding.UTF8;
            
            // Выполняем кодирование строки в массив байтов
            Byte[] encodedBytes = encodingUTF8.GetBytes(s);
            
            // Показываем значение закодированных байтов
            Console.WriteLine("Encoded bytes: " + BitConverter.ToString(encodedBytes));
            
            // Выполняем декодирование массива байтов обратно в строку
            String decodedString = encodingUTF8.GetString(encodedBytes);
            
            // Показываем декодированную строку
            Console.WriteLine("Decoded string: " + decodedString);

            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                Encoding e = ei.GetEncoding();
                Console.WriteLine("{1}{0}" +
                "\tCodePage={2}, WindowsCodePage={3}{0}" +
                "\tWebName={4}, HeaderName={5}, BodyName={6}{0}" +
                "\tIsBrowserDisplay={7}, IsBrowserSave={8}{0}" +
                "\tIsMailNewsDisplay={9}, IsMailNewsSave={10}{0}",
                Environment.NewLine,
                e.EncodingName, e.CodePage, e.WindowsCodePage,
                e.WebName, e.HeaderName, e.BodyName,
                e.IsBrowserDisplay, e.IsBrowserSave,
                e.IsMailNewsDisplay, e.IsMailNewsSave);
            }

            /*
                IBM EBCDIC (US-Canada)
                 CodePage=37, WindowsCodePage=1252
                 WebName=IBM037, HeaderName=IBM037, BodyName=IBM037
                 IsBrowserDisplay=False, IsBrowserSave=False
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                OEM United States
                 CodePage=437, WindowsCodePage=1252
                 WebName=IBM437, HeaderName=IBM437, BodyName=IBM437
                 IsBrowserDisplay=False, IsBrowserSave=False
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                IBM EBCDIC (International)
                 CodePage=500, WindowsCodePage=1252
                 WebName=IBM500, HeaderName=IBM500, BodyName=IBM500
                 IsBrowserDisplay=False, IsBrowserSave=False
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                Arabic (ASMO 708)
                 CodePage=708, WindowsCodePage=1256
                 WebName=ASMO-708, HeaderName=ASMO-708, BodyName=ASMO-708
                 IsBrowserDisplay=True, IsBrowserSave=True
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                Unicode
                 CodePage=1200, WindowsCodePage=1200
                 WebName=utf-16, HeaderName=utf-16, BodyName=utf-16
                 IsBrowserDisplay=False, IsBrowserSave=True
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                Unicode (Big-Endian)
                 CodePage=1201, WindowsCodePage=1200
                 WebName=unicodeFFFE, HeaderName=unicodeFFFE, BodyName=unicodeFFFE
                 IsBrowserDisplay=False, IsBrowserSave=False
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                Western European (DOS)
                 CodePage=850, WindowsCodePage=1252
                 WebName=ibm850, HeaderName=ibm850, BodyName=ibm850
                 IsBrowserDisplay=False, IsBrowserSave=False
                 IsMailNewsDisplay=False, IsMailNewsSave=False

                Unicode (UTF-8)
                 CodePage=65001, WindowsCodePage=1200
                 WebName=utf-8, HeaderName=utf-8, BodyName=utf-8
                 IsBrowserDisplay=True, IsBrowserSave=True
                 IsMailNewsDisplay=True, IsMailNewsSave=True             
             */
        }
    }
}
