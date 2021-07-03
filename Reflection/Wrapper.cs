using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Reflection
{
    class Wrapper
    {
        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
        {
            string dllName = new AssemblyName(args.Name).Name + ".dll";
            var assem = Assembly.GetExecutingAssembly();
            string resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));
            if (resourceName == null) 
            { 
                return null; 
            }

            // Not found, maybe another handler will find it
            using (Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        public static void LoadAssemAndShowPublicTypes(String assemId)
        {
            // Явная загрузка сборки в домен приложений
            Assembly a = Assembly.Load(assemId);

            // Цикл выполняется для каждого типа, открыто экспортируемого загруженной сборкой
            foreach (Type t in a.ExportedTypes)
            {
                // Вывод полного имени типа
                Console.WriteLine(t.FullName);
            }
        }

        private static Boolean AreObjectsTheSameType(Object o1, Object o2)
        {
            return o1.GetType() == o2.GetType();
        }

        private static void SomeMethod(Object o)
        {
            // GetType возвращает тип объекта во время выполнения
            // (позднее связывание)
            // typeof возвращает тип указанного класса
            // (раннее связывание)
            if (o.GetType() == typeof(FileInfo)) {  }
            if (o.GetType() == typeof(DirectoryInfo)) {  }
        }

        static Type typeReference /*...*/ ; // Например: o.GetType() или typeof(Object)

        TypeInfo typeDefinition = typeReference.GetTypeInfo();
    }
}
