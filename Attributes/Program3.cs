using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Attributes
{
    [assembly: CLSCompliant(true)]
    [Serializable]
    [DefaultMemberAttribute("Main")]
    [DebuggerDisplayAttribute("Richter", Name = "Jeff", Target = typeof(Program))]
    public sealed class Program3
    {
        [Conditional("Debug")]
        [Conditional("Release")]
        public void DoSomething() { }

        public Program3()
        {
        }

        [CLSCompliant(true)]
        [STAThread]
        public static void Main()
        {
            // Вывод атрибутов, примененных к данному типу
            ShowAttributes(typeof(Program));

            // Получение набора связанных с типом методов
            MemberInfo[] members = typeof(Program).FindMembers(MemberTypes.Constructor | MemberTypes.Method, 
                                                               BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static, 
                                                               Type.FilterName, "*");
            foreach (MemberInfo member in members)
            {
                // Вывод атрибутов, примененных к данному члену
                ShowAttributes(member);
            }
        }

        private static void ShowAttributes(MemberInfo attributeTarget)
        {
            IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(attributeTarget);
            Console.WriteLine("Attributes applied to {0}: {1}", attributeTarget.Name, (attributes.Count == 0 ? "None" : String.Empty));
            foreach (CustomAttributeData attribute in attributes)
            {
                // Вывод типа каждого примененного атрибута
                Type t = attribute.Constructor.DeclaringType;
                Console.WriteLine(" {0}", t.ToString());
                Console.WriteLine(" Constructor called={0}", attribute.Constructor);
                IList<CustomAttributeTypedArgument> posArgs = attribute.ConstructorArguments;
                Console.WriteLine(" Positional arguments passed to constructor:" + ((posArgs.Count == 0) ? " None" : String.Empty));
                foreach (CustomAttributeTypedArgument pa in posArgs)
                {
                    Console.WriteLine(" Type={0}, Value={1}", pa.ArgumentType, pa.Value);
                }

                IList<CustomAttributeNamedArgument> namedArgs = attribute.NamedArguments;
                Console.WriteLine(" Named arguments set after construction:" + ((namedArgs.Count == 0) ? " None" : String.Empty));
                foreach (CustomAttributeNamedArgument na in namedArgs)
                {
                    Console.WriteLine(" Name={0}, Type={1}, Value={2}", na.MemberInfo.Name, na.TypedValue.ArgumentType, na.TypedValue.Value);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /*
        Компоновка и запуск этого приложения приведут к следующему результату:
        Attributes applied to Program:
         System.SerializableAttribute
         Constructor called=Void .ctor()
         Positional arguments passed to constructor: None
         Named arguments set after construction: None
         System.Diagnostics.DebuggerDisplayAttribute
         Constructor called=Void .ctor(System.String)
         Positional arguments passed to constructor:
         Type=System.String, Value=Richter
         Named arguments set after construction:
         Name=Name, Type=System.String, Value=Jeff
         Name=Target, Type=System.Type, Value=Program
         System.Reflection.DefaultMemberAttribute
         Constructor called=Void .ctor(System.String)
         Positional arguments passed to constructor:
         Type=System.String, Value=Main
         Named arguments set after construction: None
        Attributes applied to DoSomething:
         System.Diagnostics.ConditionalAttribute
         Constructor called=Void .ctor(System.String)
         Positional arguments passed to constructor:
         Type=System.String, Value=Release
         Named arguments set after construction: None
         System.Diagnostics.ConditionalAttribute
         Constructor called=Void .ctor(System.String)
         Positional arguments passed to constructor:
         Type=System.String, Value=Debug
         Named arguments set after construction: None
        Attributes applied to Main:
         System.CLSCompliantAttribute
         Constructor called=Void .ctor(Boolean)
         Positional arguments passed to constructor:
         Type=System.Boolean, Value=True
         Named arguments set after construction: None
         System.STAThreadAttribute
         Constructor called=Void .ctor()
         Positional arguments passed to constructor: None
         Named arguments set after construction: None
        Attributes applied to .ctor: None         
         */
    }
}
