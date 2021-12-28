using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reflection
{
    class ExceptionTree
    {
        public static void Go()
        {
            // Явная загрузка сборок для отражения
            LoadAssemblies();

            // Фильтрация и сортировка всех типов
            var allTypes = (from a in AppDomain.CurrentDomain.GetAssemblies() from t in a.ExportedTypes
                            where typeof(Exception).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()) orderby t.Name
                            select t).ToArray();

            // Построение и вывод иерархического дерева наследования
            Console.WriteLine(WalkInheritanceHierarchy(new StringBuilder(), 0, typeof(Exception), allTypes));
        }

        private static StringBuilder WalkInheritanceHierarchy(StringBuilder sb, Int32 indent, Type baseType, IEnumerable<Type> allTypes)
        {
            String spaces = new String(' ', indent * 3);
            sb.AppendLine(spaces + baseType.FullName);

            foreach (var t in allTypes)
            {
                if (t.GetTypeInfo().BaseType != baseType)
                    continue;

                WalkInheritanceHierarchy(sb, indent + 1, t, allTypes);
            }
            return sb;
        }

        private static void LoadAssemblies()
        {
            String[] assemblies = {
                 "System, PublicKeyToken={0}",
                 "System.Core, PublicKeyToken={0}",
                 "System.Data, PublicKeyToken={0}",
                 "System.Design, PublicKeyToken={1}",
                 "System.DirectoryServices, PublicKeyToken={1}",
                 "System.Drawing, PublicKeyToken={1}",
                 "System.Drawing.Design, PublicKeyToken={1}",
                 "System.Management, PublicKeyToken={1}",
                 "System.Messaging, PublicKeyToken={1}",
                 "System.Runtime.Remoting, PublicKeyToken={0}",
                 "System.Security, PublicKeyToken={1}",
                 "System.ServiceProcess, PublicKeyToken={1}",
                 "System.Web, PublicKeyToken={1}",
                 "System.Web.RegularExpressions, PublicKeyToken={1}",
                 "System.Web.Services, PublicKeyToken={1}",
                 "System.Xml, PublicKeyToken={0}",
             };

            String EcmaPublicKeyToken = "b77a5c561934e089";
            String MSPublicKeyToken = "b03f5f7f11d50a3a";
            // Получение версии сборки, содержащей System.Object.
            
            // Этот же номер версии предполагается для всех остальных сборок.
            Version version = typeof(Object).Assembly.GetName().Version;

            // Явная загрузка сборок
            foreach (String a in assemblies)
            {
                String AssemblyIdentity = String.Format(a, EcmaPublicKeyToken, MSPublicKeyToken) + ", Culture=neutral, Version=" + version;
                Assembly.Load(AssemblyIdentity);
            }
        }
    }
}
