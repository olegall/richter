using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Strings
{
    class Protected
    {
        void Main() 
        {
            using (SecureString ss = new SecureString())
            {
                Console.Write("Please enter password: ");
                while (true)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    if (cki.Key == ConsoleKey.Enter) 
                        break;

                    // Присоединить символы пароля в конец SecureString
                    ss.AppendChar(cki.KeyChar);
                    Console.Write("*");
                }
                Console.WriteLine();

                // Пароль введен, отобразим его для демонстрационных целей
                DisplaySecureString(ss);
            }

            // После 'using' SecureString обрабатывается методом Disposed,
            // поэтому никаких конфиденциальных данных в памяти нет
        }

        // Этот метод небезопасен, потому что обращается к неуправляемой памяти
        private unsafe static void DisplaySecureString(SecureString ss)
        {
            Char* pc = null;
            try
            {
                // Дешифрование SecureString в буфер неуправляемой памяти
                pc = (Char*)Marshal.SecureStringToCoTaskMemUnicode(ss);
                // Доступ к буферу неуправляемой памяти, который хранит дешифрованную версию SecureString
                for (Int32 index = 0; pc[index] != 0; index++)
                    Console.Write(pc[index]);
            }
            finally
            {
                // Обеспечиваем обнуление и освобождение буфера неуправляемой памяти, который хранит расшифрованные символы SecureString
                if (pc != null)
                    Marshal.ZeroFreeCoTaskMemUnicode((IntPtr)pc);
            }
        }
    }
}
