using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    // Тип, неявно производный от Object
    class Employee
    {

    }

    // Тип, явно производный от Object
    class Employee : System.Object
    {

    }

    internal class Manager : Employee // базовый класс можно без internal
    {
 
    }

    // Вот как выглядит реализация метода Equals для Object:
    public class Object1 // почему конфликта с CLR-м object нет?
    {
        public virtual Boolean Equals(Object obj)
        {
            // Если обе ссылки указывают на один и тот же объект, значит, эти объекты равны
            if (this == obj)
                return true;

            // Предполагаем, что объекты не равны
            return false;
        }
    }

    // Учитывая это, компания Microsoft должна была бы реализовать метод Equals типа Object примерно так:
    public class Object2
    {
        public virtual Boolean Equals(Object obj)
        {
            // Сравниваемый объект не может быть равным null
            if (obj == null)
                return false;

            // Объекты разных типов не могут быть равны
            if (this.GetType() != obj.GetType())
                return false;

            // Если типы объектов совпадают, возвращаем true при условии, что все их поля попарно равны.
            // Так как в System.Object не определены поля, следует считать, что поля равны
            return true;
        }
    }

    public class Object3
    {
        public static Boolean ReferenceEquals(Object objA, Object objB)
        {
            return (objA == objB);
        }
    }

    // Открытый тип доступен из любой сборки
    public class ThisIsAPublicType {  }

    // Внутренний тип доступен только из собственной сборки
    internal class ThisIsAnInternalType {  }

    // Это внутренний тип, так как модификатор доступа не указан явно
    class ThisIsAlsoAnInternalType {  }

    public static class AStaticClass
    {
        public static void AStaticMethodQ() { } // здесь и далее - обязательно static

        public static String AStaticProperty
        {
            get { return s_AStaticField; }
            set { s_AStaticField = value; }
        }

        private static String s_AStaticField;

        public static event EventHandler AStaticEvent;
    }

    internal class Employee2
    {
        // Невиртуальный экземплярный метод
        public Int32 GetYearsEmployed() { return 0; }

        // Виртуальный метод (виртуальный - значит, экземплярный)
        public virtual String GetProgressReport() { return ""; }

        // Статический метод
        public static Employee Lookup(String name) { return new Employee(); }
    }

    internal class SomeClass
    {
        // ToString - виртуальный метод базового класса Object
        public override String ToString()
        {
            // Компилятор использует команду call для невиртуального вызова метода ToString класса Object
            // Если бы компилятор вместо call использовал callvirt, этот метод продолжал бы рекурсивно вызывать сам себя до переполнения стека
            return base.ToString();
        }
    }

    public class Set
    {
        private Int32 m_length = 0;
        
        // Этот перегруженный метод — невиртуальный
        public Int32 Find(Object value)
        {
            return Find(value, 0, m_length);
        }

        // Этот перегруженный метод — невиртуальный
        public Int32 Find(Object value, Int32 startIndex)
        {
            //return Find(value, startIndex, m_length startIndex);
            return Find(value, startIndex, startIndex);
        }

        // Наиболее функциональный метод сделан виртуальным и может быть переопределен
        public virtual Int32 Find(Object value, Int32 startIndex, Int32 endIndex)
        {
            // Здесь находится настоящая реализация, которую можно переопределить...
            return 0;
        }

        // Другие методы
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Employee eError = new Employee("ConstructorParam1");

            // Приведение типа не требуется, т. к. new возвращает объект Employee, а Object — это базовый тип для Employee.
            Object o = new Employee();

            // Приведение типа обязательно, т. к. Employee — производный от Object
            // В других языках (таких как Visual Basic) компилятор не потребует явного приведения
            Employee e = (Employee)o;

            // Создаем объект Manager и передаем его в PromoteEmployee
            // Manager ЯВЛЯЕТСЯ производным от Employee, поэтому PromoteEmployee работает
            Manager m = new Manager();
            PromoteEmployee(m);

            // Создаем объект DateTime и передаем его в PromoteEmployee
            // DateTime НЕ ЯВЛЯЕТСЯ производным от Employee, поэтому PromoteEmployee выбрасывает исключение System.InvalidCastException
            DateTime newYears = new DateTime(2013, 1, 1);
            PromoteEmployee(newYears);

            //System.Int32 a = new System.Int32();

            int a = 0; // Самый удобный синтаксис
            System.Int32 a2 = 0; // Удобный синтаксис
            int a3 = new int(); // Неудобный синтаксис
            System.Int32 a4 = new System.Int32(); // Самый неудобный синтаксис

            //Int32 i = 5; // 32-разрядное число
            //Int64 l = i; // Неявное приведение типа к 64-разрядному значению

            Int32 i = 5; // Неявное приведение Int32 к Int32
            Int64 l = i; // Неявное приведение Int32 к Int64
            Single s = i; // Неявное приведение Int32 к Single
            Byte b1 = (Byte)i; // Явное приведение Int32 к Byte
            Int16 v = (Int16)s; // Явное приведение Single к Int16

            Boolean found = false; // В готовом коде found присваивается 0
            Int32 x = 100 + 20 + 3; // В готовом коде x присваивается 123
            String s2 = "a " + "bc"; // В готовом коде s присваивается "a bc"

            Int32 x2 = 100; // Оператор присваивания
            Int32 y = x + 23; // Операторы суммирования и присваивания
            Boolean lessThanFifty = (y < 50); // Операторы "меньше чем" и присваивания

            

            //UInt32 invalid = unchecked((UInt32) - 1); // OK
            
            
            checked
            { 
                // Начало проверяемого блока
                Byte b = 100;
                b = (Byte)(b + 200); // Это выражение проверяется на переполнение
            } // Конец проверяемого блока
            
            checked
            {   // Начало проверяемого блока
                Byte b = 100;
                b += 200; // Это выражение проверяется на переполнение
            }

            //SomeVal v1 = new SomeVal(); // Размещается в стеке

            // Две следующие строки компилируются, так как C# считает, что поля в v1 инициализируются нулем
            SomeVal v1 = new SomeVal();
            Int32 a1 = v1.x;

            // Следующие строки вызовут ошибку компиляции, поскольку C# не считает, что поля в v1 инициализируются нулем
            SomeVal v1_2;
            //Int32 a2 = v1_2.x;
            // error CS0170: Use of possibly unassigned field 'x' (ошибка CS0170: Используется поле 'x', которому не присвоено значение)

            dynamic d = 123;
            //var result = M(d); // 'var result' - то же, что 'dynamic result'

            Object o1 = 123; // OK: Неявное приведение Int32 к Object (упаковка)
            //Int32 n1 = o1; // Ошибка: Нет неявного приведения Object к Int32
            Int32 n2 = (Int32)o1; // OK: Явное приведение Object к Int32 (распаковка)
            dynamic d1 = 123; // OK: Неявное приведение Int32 к dynamic (упаковка)
            Int32 n3 = d; // OK: Неявное приведение dynamic к Int32 (распаковка)

            //new DynamicObject(); // нельзя, т.к. конструктор protected
            new Object1(); // можно просто создать объект, не присваивая никуда

            dynamic stringType = new StaticMemberDynamicWrapper(typeof(String));
            var r = stringType.Concat("A", "B"); // Динамический вызов статического
                                                 // метода Concat класса String
            Console.WriteLine(r); // выводится "AB"

            new Virtual().Main();
        }

        // Ссылочный тип (поскольку 'class')
        class SomeRef { public Int32 x; }
        
        // Значимый тип (поскольку 'struct')
        struct SomeVal { public Int32 x; }

        // Для повышения производительности разрешим CLR установить порядок полей для этого типа
        [StructLayout(LayoutKind.Auto)]
        internal struct SomeValType
        {
            private readonly Byte m_b;
            private readonly Int16 m_x;
        }

        void _1()
        {
            Object o = new Object();
            Boolean b1 = (o is Object); // b1 равно true
            Boolean b2 = (o is Employee); // b2 равно false

            if (o is Employee)
            {
                //Employee e = (Employee)o; // конфликт с e, объявленным ниже
                // Используем e внутри инструкции if
            }

            Employee e = o as Employee;
            if (e != null)
            {
                // Используем e внутри инструкции if
            }
        }

        void _2()
        {
            System.Object o = new Object(); // Создание объекта Object
            Employee e = o as Employee; // Приведение o к типу Employee. Преобразование невыполнимо: исключение не возникло, но e равно null
            e.ToString(); // Обращение к e вызывает исключение NullReferenceException
        }

        void _3() 
        {
            Byte b = 100;
            b = (Byte)(b + 200); // После этого b равно 44 (2C в шестнадцатеричной записи) 
        }

        void _4() 
        {
            Byte b = 100; // Выдается исключение
            b = checked((Byte)(b + 200)); // OverflowException

            b = (Byte)checked(b + 200); // b содержит 44; нет OverflowException
        }

        public static void PromoteEmployee(Object o)
        {
            // В этом месте компилятор не знает точно, на какой тип объекта ссылается o, поэтому скомпилирует этот код
            // Однако в период выполнения CLR знает, на какой тип ссылается объект o (приведение типа выполняется каждый раз),
            // и проверяет, соответствует ли тип объекта типу Employee или другому типу, производному от Employee
            Employee e = (Employee)o;
        }

        static void ValueTypeDemo()
        {
            SomeRef r1 = new SomeRef(); // Размещается в куче
            SomeVal v1 = new SomeVal(); // Размещается в стеке
            r1.x = 5; // Разыменовывание указателя
            v1.x = 5; // Изменение в стеке
            Console.WriteLine(r1.x); // Отображается "5"
            Console.WriteLine(v1.x); // Также отображается "5"
                                        // В левой части рис. 5.2 показан результат
                                        // выполнения предыдущих строк
            SomeRef r2 = r1; // Копируется только ссылка (указатель)
            SomeVal v2 = v1; // Помещаем в стек и копируем члены
            r1.x = 8; // Изменяются r1.x и r2.x
            v1.x = 9; // Изменяется v1.x, но не v2.x
            Console.WriteLine(r1.x); // Отображается "8"
            Console.WriteLine(r2.x); // Отображается "8"
            Console.WriteLine(v1.x); // Отображается "9"
            Console.WriteLine(v2.x); // Отображается "5"
                                        // В правой части рис. 5.2 показан результат
                                        // выполнения ВСЕХ предыдущих строк
        }
    }
}