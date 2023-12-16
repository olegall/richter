using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Attributes
{
    
    //[assembly: CLSCompliant(true)]
    [Serializable]
    [DefaultMemberAttribute("Main")]
    [DebuggerDisplayAttribute("Richter", Name = "Jeff", Target = typeof(Program))]
    public sealed class Program2
    {
        [Conditional("Debug")]
        [Conditional("Release")]
        public void DoSomething() { }

        public Program2()
        {
        }

        [CLSCompliant(true)]
        [STAThread]
        public static void Main_()
        {
            // Вывод набора атрибутов, примененных к типу
            ShowAttributes(typeof(Program));

            // Получение и задание методов, связанных с типом
            var members =
                from m in typeof(Program).GetTypeInfo().DeclaredMembers.OfType<MethodBase>()
                where m.IsPublic
                select m;

            foreach (MemberInfo member in members)
            {
                // Вывод набора атрибутов, примененных к члену
                ShowAttributes(member);
            }
        }

        private static void ShowAttributes(MemberInfo attributeTarget)
        {
            var attributes = attributeTarget.GetCustomAttributes<Attribute>();
            Console.WriteLine("Attributes applied to {0}: {1}", attributeTarget.Name, (attributes.Count() == 0 ? "None" : String.Empty));

            foreach (Attribute attribute in attributes)
            {
                // Вывод типа всех примененных атрибутов
                Console.WriteLine(" {0}", attribute.GetType().ToString());
                if (attribute is DefaultMemberAttribute)
                    Console.WriteLine(" MemberName={0}", ((DefaultMemberAttribute)attribute).MemberName);

                if (attribute is ConditionalAttribute)
                    Console.WriteLine(" ConditionString={0}", ((ConditionalAttribute)attribute).ConditionString);

                if (attribute is CLSCompliantAttribute)
                    Console.WriteLine(" IsCompliant={0}", ((CLSCompliantAttribute)attribute).IsCompliant);
                
                DebuggerDisplayAttribute dda = attribute as DebuggerDisplayAttribute;
                if (dda != null)
                {
                    Console.WriteLine(" Value={0}, Name={1}, Target={2}",
                    dda.Value, dda.Name, dda.Target);
                }
            }
            Console.WriteLine();
        }

        /*
            Скомпоновав и запустив это приложение, мы получим следующий результат:
            Attributes applied to Program:
             System.SerializableAttribute
             System.Diagnostics.DebuggerDisplayAttribute
             Value=Richter, Name=Jeff, Target=Program
             System.Reflection.DefaultMemberAttribute
             MemberName=Main
            Attributes applied to DoSomething:
             System.Diagnostics.ConditionalAttribute
             ConditionString=Release
             System.Diagnostics.ConditionalAttribute
             ConditionString=Debug
            Attributes applied to Main:
             System.CLSCompliantAttribute
             IsCompliant=True
             System.STAThreadAttribute
            Attributes applied to .ctor: None         
        */
    }
}
