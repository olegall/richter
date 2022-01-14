using System;

namespace Methods
{
    class PartialMethods
    {
        // Сгенерированный код в некотором файле с исходным кодом:
        internal class Base
        {
            private String m_name;

            // Вызывается перед изменением поля m_name
            protected virtual void OnNameChanging(String value)
            {
            }

            public String Name
            {
                get { return m_name; }
                set
                {
                    // Информирует класс о возможных изменениях
                    OnNameChanging(value.ToUpper());

                    m_name = value; // Изменение поля
                }
            }
        }

        // Написанный программистом код из другого файла
        internal class Derived : Base
        {
            protected override void OnNameChanging(string value)
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
            }
        }

        // Сгенерированный при помощи инструмента программный код
        internal sealed partial class Base2
        {
            private String m_name;

            // Это объявление с определением частичного метода вызывается перед изменением поля m_name
            partial void OnNameChanging(String value);

            public String Name
            {
                get { return m_name; }
                set
                {
                    // Информирование класса о потенциальном изменении
                    OnNameChanging(value.ToUpper());
                    m_name = value; // Изменение поля
                }
            }
        }

        // Написанный программистом код, содержащийся в другом файле
        internal sealed partial class Base3
        {
            // Это объявление с реализацией частичного метода вызывается перед тем, как будет изменено поле m_name
            partial void OnNameChanging(String value) // чтобы не было ошибки - убрать partial либо реализацию (скобки)
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
            }
        }

        // Логический эквивалент сгенерированного инструментом кода в случае, когда нет объявления выполняемого частичного метода
        internal sealed class Base4
        {
            private String m_name;

            public String Name
            {
                get { return m_name; }
                set
                {
                    m_name = value; // Измените поле
                }
            }
        }


        public void Main() 
        {
            
        }
    }
}