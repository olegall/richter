using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Attributes
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal sealed class OSVERSIONINFO
    {
        public OSVERSIONINFO()
        {
            OSVersionInfoSize = (UInt32)Marshal.SizeOf(this);
        }

        public UInt32 OSVersionInfoSize = 0;
        public UInt32 MajorVersion = 0;
        public UInt32 MinorVersion = 0;
        public UInt32 BuildNumber = 0;
        public UInt32 PlatformId = 0;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String CSDVersion = null;
    }

    internal sealed class MyClass
    {
        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean GetVersionEx([In, Out] OSVERSIONINFO ver);
    }

    //[assembly: SomeAttr] // Применяется к сборке
    //[module: SomeAttr] // Применяется к модулю
    //[type: SomeAttr] // Применяется к типу
    //internal sealed class SomeType<[typevar: SomeAttr] T>
    //{ // Применяется
    //  // к переменной обобщенного типа
    //    [field: SomeAttr] // Применяется к полю
    //    public Int32 SomeField = 0;
    //    [return: SomeAttr] // Применяется к возвращаемому значению
    //    [method: SomeAttr] // Применяется к методу
    //    public Int32 SomeMethod(
    //    [param: SomeAttr] // Применяется к параметру
    //    Int32 SomeParam)
    //    { return SomeParam; }
    //    [property: SomeAttr] // Применяется к свойству
    //    public String SomeProp
    //    {
    //        [method: SomeAttr] // Применяется к механизму доступа get
    //        get { return null; }
    //    }
    //    [event: SomeAttr] // Применяется к событиям
    //    [field: SomeAttr] // Применяется к полям, созданным компилятором
    //    [method: SomeAttr] // Применяется к созданным компилятором методам add и remove
    //    public event EventHandler SomeEvent;
    //}

    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]
    public class FlagsAttribute : System.Attribute
    {
        public FlagsAttribute()
        {
        }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class AttributeUsageAttribute : Attribute
    {
        internal static AttributeUsageAttribute Default = new AttributeUsageAttribute(AttributeTargets.All);
        internal Boolean m_allowMultiple = false;
        internal AttributeTargets m_attributeTarget = AttributeTargets.All;
        internal Boolean m_inherited = true;

        // Единственный открытый конструктор
        public AttributeUsageAttribute(AttributeTargets validOn)
        {
            m_attributeTarget = validOn;
        }

        internal AttributeUsageAttribute(AttributeTargets validOn, Boolean allowMultiple, Boolean inherited)
        {
            m_attributeTarget = validOn;
            m_allowMultiple = allowMultiple;
            m_inherited = inherited;
        }

        public Boolean AllowMultiple
        {
            get { return m_allowMultiple; }
            set { m_allowMultiple = value; }
        }

        public Boolean Inherited
        {
            get { return m_inherited; }
            set { m_inherited = value; }
        }

        public AttributeTargets ValidOn
        {
            get { return m_attributeTarget; }
        }
    }

    [Flags, Serializable]
    public enum AttributeTargets
    {
        Assembly = 0x0001,
        Module = 0x0002,
        Class = 0x0004,
        Struct = 0x0008,
        Enum = 0x0010,
        Constructor = 0x0020,
        Method = 0x0040,
        Property = 0x0080,
        Field = 0x0100,
        Event = 0x0200,
        Interface = 0x0400,
        Parameter = 0x0800,
        Delegate = 0x1000,
        ReturnValue = 0x2000,
        GenericParameter = 0x4000,
        All = Assembly | Module | Class | Struct | Enum |
        Constructor | Method | Property | Field | Event |
        Interface | Parameter | Delegate | ReturnValue |
        GenericParameter
    }

    [Flags]
    internal enum Color
    {
        Red
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    internal class TastyAttribute : Attribute
    {
    }

    [Tasty]
    [Serializable]
    internal class BaseType
    {
        [Tasty] protected virtual void DoSomething() { }
    }

    internal class DerivedType : BaseType
    {
        protected override void DoSomething() { }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SomeAttribute : Attribute
    {
        public SomeAttribute(String name, Object o, Type[] types)
        {
            // 'name' ссылается на String
            // 'o' ссылается на один из легальных типов (упаковка при необходимости)
            // 'types' ссылается на одномерный массив Types с нулевой нижней границей
        }
    }

    [Some("Jeff", Color.Red, new Type[] { typeof(Math), typeof(Console) })]
    internal sealed class SomeType
    {
    }

    [Conditional("TEST")]
    [Conditional("VERIFY")]
    public sealed class CondAttribute : Attribute
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("CondAttribute is {0}applied to Program type.", Attribute.IsDefined(typeof(Program), typeof(CondAttribute)) ? "" : "not ");
            Program2.Main_();
            Program3.Main_();
        }

        public override String ToString()
        {
            // Применяется ли к перечислимому типу экземпляр типа FlagsAttribute?
            if (this.GetType().IsDefined(typeof(FlagsAttribute), false))
            {
                // Да; выполняем код, интерпретирующий значение как перечислимый тип с битовыми флагами
            }
            else
            {
                // Нет; выполняем код, интерпретирующий значение как обычный перечислимый тип
            }
            return null; // aleek
        }
    }
}
