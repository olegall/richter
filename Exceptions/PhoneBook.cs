using System;
using System.IO;

namespace Exceptions
{
    internal sealed class PhoneBook
    {
        private String m_pathname; // Путь к файлу с телефонами
                                   // Выполнение других методов

        public String GetPhoneNumber(String name)
        {
            String phone = null;

            FileStream fs = null;

            try
            {
                fs = new FileStream(m_pathname, FileMode.Open);
                // Чтение переменной fs до обнаружения нужного имени phone = /* номер телефона найден */
            }
            catch (FileNotFoundException e)
            {
                // Генерирование другого исключения, содержащего имя абонента, с заданием исходного исключения в качестве внутреннего
                // throw new NameNotFoundException(name, e); // точно?
            }
            catch (IOException e)
            {
                // Генерирование другого исключения, содержащего имя абонента, с заданием исходного исключения в качестве внутреннего
                // throw new NameNotFoundException(name, e); // точно?
            }
            finally
            {
                if (fs != null) 
                    fs.Close();
            }

            return phone;
        }
    }
}