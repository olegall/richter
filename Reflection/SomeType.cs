using System;

namespace Reflection
{
    // Класс для демонстрации отражения. У него есть поле, конструктор, метод, свойство и событие
    internal sealed class SomeType
    {
        private Int32 m_someField;
        
        public SomeType(ref Int32 x)
        { 
            x *= 2;
        }
        
        public Int32 SomeProp
        {
            get 
            { 
                return m_someField; 
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                m_someField = value;
            }
        }

        // почему вызывается при запуске?
        public override String ToString()
        {
            return m_someField.ToString();
        }

        public event EventHandler SomeEvent;

        private void NoCompilerWarnings() 
        { 
            SomeEvent.ToString(); 
        }
    }
}