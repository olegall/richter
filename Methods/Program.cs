using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Methods
{
    class Program
    {
        //public class SomeType { }
        // Это определение идентично определению:

        //public class SomeType
        //{
        //    public SomeType() : base() { }
        //    //public SomeType(){}
        //}

        //internal sealed class SomeType
        //{
        //    private Int32 m_x = 5;
        //    private String m_s = "Hi there";
        //    private Double m_d = 3.14159;
        //    private Byte m_b;

        //    // Это конструкторы
        //    public SomeType() {  }
        //    public SomeType(Int32 x) {  }
        //    public SomeType(String s) { m_d = 10; }
        //}

        internal sealed class SomeType
        {
            // Здесь нет кода, явно инициализирующего поля
            private Int32 m_x;
            private String m_s;
            private Double m_d;
            private Byte m_b;

            // Код этого конструктора инициализирует поля значениями по умолчанию
            // Этот конструктор должен вызываться всеми остальными конструкторами
            public SomeType()
            {
                m_x = 5;
                m_s = "Hi there";
                m_d = 3.14159;
                m_b = 0xff;
            }

            // Этот конструктор инициализирует поля значениями по умолчанию, а затем изменяет значение m_x
            public SomeType(Int32 x) : this()
            {
                m_x = x;
            }
            
            // Этот конструктор инициализирует поля значениями по умолчанию, а затем изменяет значение m_s
            public SomeType(String s) : this()
            {
                m_s = s;
            }

            // Этот конструктор инициализирует поля значениями по умолчанию, а затем изменяет значения m_x и m_s
            public SomeType(Int32 x, String s) : this()
            {
                m_x = x;
                m_s = s;
            }
        }

        internal struct Point
        {
            public Int32 m_x, m_y;
            public Point(Int32 x, Int32 y)
            {
                m_x = x;
                m_y = y;
            }
        }

        internal sealed class Rectangle
        {
            public Point m_topLeft, m_bottomRight;

            public Rectangle()
            {
                // В C# оператор new, использованный для создания экземпляра значимого типа, вызывает конструктор для инициализации полей значимого типа
                m_topLeft = new Point(1, 2);
                m_bottomRight = new Point(100, 200);
            }
        }

        //internal struct SomeValType
        //{
        //    // В значимый тип нельзя подставлять инициализацию экземплярных полей
        //    private Int32 m_x = 5;
        //}

        internal struct SomeValType
        {
            private Int32 m_x, m_y;

            //// C# допускает наличие у значимых типов конструкторов с параметрами
            //public SomeValType(Int32 x)
            //{
            //    m_x = x;
            //    // Обратите внимание: поле m_y здесь не инициализируется
            //}

            // C# позволяет значимым типам иметь конструкторы с параметрами
            public SomeValType(Int32 x)
            {
                // Выглядит необычно, но компилируется прекрасно, и все поля инициализируются значениями 0 или null
                this = new SomeValType();
                var res1 = this;

                m_x = x; // Присваивает m_x значение x. Обратите внимание, что поле m_y было инициализировано нулем
            }
        }

        internal sealed class SomeRefType
        {
            static SomeRefType()
            {
                // Исполняется при первом обращении к ссылочному типу SomeRefType
            }
        }

        internal struct SomeValTypeStatic
        {
            // C# на самом деле допускает определять для значимых типов конструкторы без параметров
            static SomeValTypeStatic()
            {
                // Исполняется при первом обращении к значимому типу SomeValType
            }
        }

        /* Хотя конструктор типа можно определить в значимом типе, этого никогда не следует делать, 
           так как иногда CLR не вызывает статический конструктор значимого типа.
           Например:*/
        internal struct SomeValType2
        {
            static SomeValType2()
            {
                Console.WriteLine("This never gets displayed");
            }
            
            public Int32 m_x;
        }

        internal sealed class SomeType3
        {
            private static Int32 s_x = 5;
        }

        internal sealed class SomeType4
        {
            private static Int32 s_x;
            static SomeType4() { s_x = 5; }
        }

        public sealed class Complex
        {
            public static Complex operator +(Complex c1, Complex c2) { return null; }
        }

        public sealed class Rational
        {
            // Создает Rational из Int32
            public Rational(Int32 num) { }

            // Создает Rational из Single
            public Rational(Single num) {  }

            // Преобразует Rational в Int32
            public Int32 ToInt32() { return 0; }

            // Преобразует Rational в Single
            public Single ToSingle() { return 0; }

            // Неявно создает Rational из Int32 и возвращает полученный объект
            public static implicit operator Rational(Int32 num)
            {
                return new Rational(num);
            }

            // Неявно создает Rational из Single и возвращает полученный объект
            public static implicit operator Rational(Single num)
            {
                return new Rational(num);
            }

            // Явно возвращает объект типа Int32, полученный из Rational
            public static explicit operator Int32(Rational r)
            {
                return r.ToInt32();
            }

            // Явно возвращает объект типа Single, полученный из Rational
            public static explicit operator Single(Rational r)
            {
                return r.ToSingle();
            }
        }

        static void Main(string[] args)
        {
            SomeValType2[] a = new SomeValType2[10];
            a[0].m_x = 123;
            Console.WriteLine(a[0].m_x); // Выводится 123

            Rational r1 = 5; // Неявное приведение Int32 к Rational
            Rational r2 = 2.5F; // Неявное приведение Single к Rational
            Int32 x = (Int32)r1; // Явное приведение Rational к Int32
            Single s = (Single)r2; // Явное приведение Rational к Single
        }
    }
}
