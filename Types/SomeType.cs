using System;

namespace Types
{
    public sealed class SomeType
    { // 1
      // Вложенный класс
        private class SomeNestedType { } 
        
        // 2
        // Константа, неизменяемое и статическое изменяемое поле
        // Constant, readonly, and static read/write field
        private const Int32 c_SomeConstant = 1; // 3

        private readonly String m_SomeReadOnlyField = "2"; // 4

        private static Int32 s_SomeReadWriteField = 3; // 5

        // Конструктор типа
        static SomeType() { } // 6

        // Конструкторы экземпляров
        public SomeType(Int32 x) { } // 7

        public SomeType() { } // 8
                              // Экземплярный и статический методы

        private String InstanceMethod() { return null; } // 9

        public static void Main_() { } // 10
                                      // Необобщенное экземплярное свойство
        public Int32 SomeProp
        { // 11
            get { return 0; } // 12

            set { } // 13
        }

        // Обобщенное экземплярное свойство
        public Int32 this[String s]
        { // 14
            get { return 0; } // 15

            set { } // 16
        }

        // Экземплярное событие
        public event EventHandler SomeEvent; // 17
    }
}