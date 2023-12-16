using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace Reflection
{
    internal sealed class Dictionary<TKey, TValue> // не конфликтует с System.Collections.Generic.Dictionary aleek
    {
    }

    

    class Program
    {
        static void LoadAssemAndShowPublicTypes(String assemId)
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

        public static void SomeMethod(Object o)
        {
            // GetType возвращает тип объекта во время выполнения (позднее связывание)
            // typeof возвращает тип указанного класса (раннее связывание)

            if (o.GetType() == typeof(FileInfo)) { }

            if (o.GetType() == typeof(DirectoryInfo)) { }

            var false1 = new DirectoryInfo("C:/").GetType() == typeof(FileInfo);
        }

        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
        {
            string dllName = new AssemblyName(args.Name).Name + ".dll";

            var assem = Assembly.GetExecutingAssembly();

            string resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));

            if (resourceName == null)
                return null;

            // Not found, maybe another handler will find it
            using (Stream stream = assem.GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        private static Boolean AreObjectsTheSameType(Object o1, Object o2)
        {
            return o1.GetType() == o2.GetType();
        }

        static Type typeReference /*...*/ ; // Например: o.GetType() или typeof(Object)

        TypeInfo typeDefinition = typeReference.GetTypeInfo();

        private const BindingFlags c_bf = 
            BindingFlags.FlattenHierarchy | 
            BindingFlags.Instance | 
            BindingFlags.Static | 
            BindingFlags.Public | 
            BindingFlags.NonPublic;

        static void Main(string[] args)
        {
            ExceptionTree.Go();

            string dataAssembly = "System.Data, version=4.0.0.0, culture=neutral, PublicKeyToken=b77a5c561934e089";
            LoadAssemAndShowPublicTypes(dataAssembly);
            SomeMethod(new object());
            
            #region aleek
            //new Dictionary<,>();
            var res2 = new Dictionary<object, object>();
            var res3 = new Dictionary<string, string>();
            var res4 = typeof(Dictionary<,>);
            var res5 = typeof(Dictionary<object, object>);
            #endregion

            // Получаем ссылку на объект Type обобщенного типа
            // если закомментировать объявленный здесь Dictionary - перейдёт на System.Collections.Generic.Dictionary. Почему?
            Type openType = typeof(Dictionary<,>);

            // Закрываем обобщенный тип, используя TKey=String, TValue=Int32
            Type closedType = openType.MakeGenericType(new Type[] { typeof(String), typeof(Int32) });

            // Создаем экземпляр закрытого типа
            Object o = Activator.CreateInstance(closedType);

            // Проверяем, работает ли наше решение
            Console.WriteLine(o.GetType());

            // Перебор всех сборок, загруженных в домене
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            //foreach (Assembly a in assemblies.Where(x => x.FullName.Contains("System.Data"))) // aleek
            {
                Show(0, "Assembly: {0}", a);

                // Поиск типов в сборке
                foreach (Type t in a.ExportedTypes)
                {
                    Show(1, "Type: {0}", t);
                    // Получение информации о членах типа
                    foreach (MemberInfo mi in t.GetTypeInfo().DeclaredMembers)
                    {
                        String typeName = String.Empty;
                        if (mi is Type) typeName = "(Nested) Type";
                        if (mi is FieldInfo) typeName = "FieldInfo";
                        if (mi is MethodInfo) typeName = "MethodInfo";
                        if (mi is ConstructorInfo) typeName = "ConstructoInfo";
                        if (mi is PropertyInfo) typeName = "PropertyInfo";
                        if (mi is EventInfo) typeName = "EventInfo";
                        Show(2, "{0}: {1}", typeName, mi);
                    }
                }
            }

            Type someTypeRefTest = typeof(SomeTypeRefTest);
            RefTest(someTypeRefTest);

            Type someType = typeof(SomeType);
            BindToMemberThenInvokeTheMember(someType);

            BindToMemberCreateDelegateToMemberThenInvokeTheMember(someType);

            UseDynamicToBindAndInvokeTheMember(someType);

            // Выводим размер кучи до отражения
            // Show("Before doing anything");
            // Создаем кэш объектов MethodInfo для всех методов из MSCorlib.dll
            List<MethodBase> methodInfos = new List<MethodBase>();
            foreach (Type t in typeof(Object).Assembly.GetExportedTypes())
            {
                // Игнорируем обобщенные типы
                if (t.IsGenericTypeDefinition) 
                    continue; 

                MethodBase[] mb = t.GetMethods(c_bf);
                methodInfos.AddRange(mb);
            }

            // Выводим количество методов и размер кучи после привязки всех методов
            Console.WriteLine("# of methods={0:N0}", methodInfos.Count);
            Show("After building cache of MethodInfo objects");

            // Создаем кэш дескрипторов RuntimeMethodHandles для всех объектов MethodInfo
            List<RuntimeMethodHandle> methodHandles = methodInfos.ConvertAll<RuntimeMethodHandle>(mb => mb.MethodHandle);
            Show("Holding MethodInfo and RuntimeMethodHandle cache");

            GC.KeepAlive(methodInfos); // Запрещаем уборку мусора в кэше
            methodInfos = null; // Разрешаем уборку мусора в кэше
            Show("After freeing MethodInfo objects");

            methodInfos = methodHandles.ConvertAll<MethodBase>(rmh => MethodBase.GetMethodFromHandle(rmh));
            Show("Size of heap after re-creating MethodInfo objects");

            GC.KeepAlive(methodHandles); // Запрещаем уборку мусора в кэше
            GC.KeepAlive(methodInfos); // Запрещаем уборку мусора в кэше
            methodHandles = null; // Разрешаем уборку мусора в кэше
            methodInfos = null; // Разрешаем уборку мусора в кэше
            Show("After freeing MethodInfos and RuntimeMethodHandles");

            Console.ReadLine();
        }

        private static void Show(Int32 indent, String format, params Object[] args)
        {
            Console.WriteLine(new String(' ', 3 * indent) + format, args);
        }

        private static void Show(String format) // aleek
        {
            //Console.WriteLine(new String(' ', 3 * indent) + format, args);
        }

        private static void RefTest(Type type)
        {
            // Создание экземпляра
            Type ctorArgument = Type.GetType("Reflection.Foo");

            ConstructorInfo ctor = type.GetTypeInfo().DeclaredConstructors.First(c => c.GetParameters()[0].ParameterType == ctorArgument);

            Object[] args = new Object[] { new Foo { FooProp = 12 } };

            var x1 = args[0]; // x before constructor called
            Console.WriteLine(((Foo)args[0]).FooProp);
            Object obj = ctor.Invoke(args); // навести мышью на args

            var x2 = args[0]; // x after constructor called
            Console.WriteLine(((Foo)args[0]).FooProp);
        }

        private static void BindToMemberThenInvokeTheMember(Type type)
        {
            Console.WriteLine("BindToMemberThenInvokeTheMember");

            // Создание экземпляра
            Type ctorArgument = Type.GetType("System.Int32&"); // что такое System.Int32&?

            // или typeof(Int32).MakeByRefType();
            ConstructorInfo ctor = type.GetTypeInfo().DeclaredConstructors.First(c => c.GetParameters()[0].ParameterType == ctorArgument);
            Object[] args = new Object[] { 12 }; // Аргументы конструктора
            Console.WriteLine("x before constructor called: " + args[0]); // Откуда watch знает что int? (object {int})
            
            Object obj = ctor.Invoke(args);
            Console.WriteLine("Type: " + obj.GetType());
            Console.WriteLine("x after constructor returns: " + args[0]);

            // Чтение и запись в поле
            FieldInfo fi = obj.GetType().GetTypeInfo().GetDeclaredField("m_someField");
            fi.SetValue(obj, 33);
            Console.WriteLine("someField: " + fi.GetValue(obj));

            // Вызов метода
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            String s = (String)mi.Invoke(obj, null);
            Console.WriteLine("ToString: " + s);

            // Чтение и запись свойства
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");

            try
            {
                pi.SetValue(obj, 0, null);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException.GetType() != typeof(ArgumentOutOfRangeException))
                    throw;

                Console.WriteLine("Property set catch.");
            }

            pi.SetValue(obj, 2, null);
            Console.WriteLine("SomeProp: " + pi.GetValue(obj, null));
            
            // Добавление и удаление делегата для события
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            EventHandler eh = new EventHandler(EventCallback); // См. ei.EventHandlerType
            ei.AddEventHandler(obj, eh);
            ei.RemoveEventHandler(obj, eh);
        }

        private static void BindToMemberCreateDelegateToMemberThenInvokeTheMember(Type type)
        {
            Console.WriteLine("BindToMemberCreateDelegateToMemberThenInvokeTheMember");

            // Создание экземпляра (нельзя создать делегата для конструктора)
            Object[] args = new Object[] { 12 }; // Аргументы конструктора
            Console.WriteLine("x before constructor called: " + args[0]);

            Object someTypeObj = Activator.CreateInstance(type, args);
            Console.WriteLine("Type: " + someTypeObj.GetType().ToString());
            Console.WriteLine("x after constructor returns: " + args[0]);

            // ВНИМАНИЕ: нельзя создать делегата для поля. Вызов метода
            MethodInfo mi = someTypeObj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            var toString = mi.CreateDelegate<Func<String>>(someTypeObj);
            String s = toString();
            Console.WriteLine("ToString: " + s);

            // Чтение и запись свойства
            PropertyInfo pi = someTypeObj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            var setSomeProp = pi.SetMethod.CreateDelegate<Action<Int32>>(someTypeObj);
            try
            {
                setSomeProp(0);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Property set catch.");
            }

            setSomeProp(2);

            var getSomeProp = pi.GetMethod.CreateDelegate<Func<Int32>>(someTypeObj);
            Console.WriteLine("SomeProp: " + getSomeProp());

            // Добавление и удаление делегата для события
            EventInfo eventInfo = someTypeObj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");

            Action<EventHandler> addSomeEvent = eventInfo.AddMethod.CreateDelegate<Action<EventHandler>>(someTypeObj);
            addSomeEvent(EventCallback);

            Action<EventHandler> removeSomeEvent = eventInfo.RemoveMethod.CreateDelegate<Action<EventHandler>>(someTypeObj);
            removeSomeEvent(EventCallback);
        }

        private static void UseDynamicToBindAndInvokeTheMember(Type type)
        {
            Console.WriteLine("UseDynamicToBindAndInvokeTheMember");

            // Создание экземпляра (dynamic нельзя использовать для вызова конструктора)
            Object[] args = new Object[] { 12 }; // Аргументы конструктора
            Console.WriteLine("x before constructor called: " + args[0]);

            dynamic someTypeObj = Activator.CreateInstance(type, args);
            Console.WriteLine("Type: " + someTypeObj.GetType().ToString());
            Console.WriteLine("x after constructor returns: " + args[0]);

            // Чтение и запись поля
            try
            {
                someTypeObj.m_someField = 5;
                Int32 v = (Int32)someTypeObj.m_someField;
                Console.WriteLine("someField: " + v);
            }
            catch (RuntimeBinderException e)
            {
                // Получает управление, потому что поле является приватным
                Console.WriteLine("Failed to access field: " + e.Message);
            }

            // Вызов метода
            String s = (String)someTypeObj.ToString();
            Console.WriteLine("ToString: " + s);

            // Чтение и запись свойства
            try
            {
                someTypeObj.SomeProp = 0; // если поставить точку останова в SomeType напротив throw new ArgumentOutOfRangeException("value");, то остановится, если нет - то нет
            }
            catch (ArgumentOutOfRangeException) 
            {
                Console.WriteLine("Property set catch.");
            }

            someTypeObj.SomeProp = 2; // можно навесить любое св-во, т.к. dynamic
            Int32 val = (Int32)someTypeObj.SomeProp;
            Console.WriteLine("SomeProp: " + val);

            // Добавление и удаление делегата для события
            someTypeObj.SomeEvent += new EventHandler(EventCallback);
            //obj.SomeEvent = new EventHandler(EventCallback); // ошибка
        }

        // Добавление метода обратного вызова для события
        private static void EventCallback(Object sender, EventArgs e) { }


    }
}