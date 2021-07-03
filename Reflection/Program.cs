using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace Reflection
{
    internal sealed class Dictionary<TKey, TValue> // не конфликтует с System.Collections.Generic.Dictionary  
    {
    }
    
    class Program
    {
        private const BindingFlags c_bf = 
            BindingFlags.FlattenHierarchy | 
            BindingFlags.Instance | 
            BindingFlags.Static | 
            BindingFlags.Public | 
            BindingFlags.NonPublic;

        static void Main(string[] args)
        {
            Console.WriteLine("*** LoadAssemAndShowPublicTypes ***");
            string dataAssembly = "System.Data, version=4.0.0.0, culture=neutral, PublicKeyToken=b77a5c561934e089";
            Wrapper.LoadAssemAndShowPublicTypes(dataAssembly);

            //var a1 = new Dictionary<,>();
            var a2 = new Dictionary<object,object>();
            var a3 = new Dictionary<string,string>();
            var a4 = typeof(Dictionary<,>);
            var a5 = typeof(Dictionary<object,object>);

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
            //foreach (Assembly a in assemblies)
            foreach (Assembly a in assemblies.Where(x => x.FullName.Contains("System.Data")))
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

            Console.WriteLine();
            Type t_ = typeof(SomeType);
            BindToMemberThenInvokeTheMember(t_);
            Console.WriteLine();

            BindToMemberCreateDelegateToMemberThenInvokeTheMember(t_);
            Console.WriteLine();

            UseDynamicToBindAndInvokeTheMember(t_);
            Console.WriteLine();

            // Выводим размер кучи до отражения
            // Show("Before doing anything");
            // Создаем кэш объектов MethodInfo для всех методов из MSCorlib.dll
            List<MethodBase> methodInfos = new List<MethodBase>();
            foreach (Type t in typeof(Object).Assembly.GetExportedTypes())
            {
                // Игнорируем обобщенные типы
                if (t.IsGenericTypeDefinition) 
                { 
                    continue; 
                }
                MethodBase[] mb = t.GetMethods(c_bf);
                methodInfos.AddRange(mb);
            }

            // Выводим количество методов и размер кучи после привязки всех методов
            Console.WriteLine("# of methods={0:N0}", methodInfos.Count);
            Show("After building cache of MethodInfo objects");

            // Создаем кэш дескрипторов RuntimeMethodHandles
            // для всех объектов MethodInfo
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

        // моё
        private static void Show(String format)
        {
            //Console.WriteLine(new String(' ', 3 * indent) + format, args);
        }

        private static void BindToMemberThenInvokeTheMember(Type type)
        {
            Console.WriteLine("BindToMemberThenInvokeTheMember");

            // Создание экземпляра
            Type ctorArgument = Type.GetType("System.Int32&");

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
                { 
                    throw; 
                }
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

        // Добавление метода обратного вызова для события
        private static void EventCallback(Object sender, EventArgs e) 
        {
        }

        private static void BindToMemberCreateDelegateToMemberThenInvokeTheMember(Type t)
        {
            Console.WriteLine("BindToMemberCreateDelegateToMemberThenInvokeTheMember");
            // Создание экземпляра (нельзя создать делегата для конструктора)
            Object[] args = new Object[] { 12 }; // Аргументы конструктора
            Console.WriteLine("x before constructor called: " + args[0]);
            Object obj = Activator.CreateInstance(t, args);
            Console.WriteLine("Type: " + obj.GetType().ToString());
            Console.WriteLine("x after constructor returns: " + args[0]);
            // ВНИМАНИЕ: нельзя создать делегата для поля.
            // Вызов метода
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            var toString = mi.CreateDelegate<Func<String>>(obj);
            String s = toString();
            Console.WriteLine("ToString: " + s);
            // Чтение и запись свойства
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("SomeProp");
            var setSomeProp = pi.SetMethod.CreateDelegate<Action<Int32>>(obj);
            try
            {
                setSomeProp(0);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Property set catch.");
            }
            setSomeProp(2);
            var getSomeProp = pi.GetMethod.CreateDelegate<Func<Int32>>(obj);
            Console.WriteLine("SomeProp: " + getSomeProp());
            // Добавление и удаление делегата для события
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            var addSomeEvent = ei.AddMethod.CreateDelegate<Action<EventHandler>>(obj);
            addSomeEvent(EventCallback);
            var removeSomeEvent =
            ei.RemoveMethod.CreateDelegate<Action<EventHandler>>(obj);
            removeSomeEvent(EventCallback);
        }

        private static void UseDynamicToBindAndInvokeTheMember(Type t)
        {
            Console.WriteLine("UseDynamicToBindAndInvokeTheMember");

            // Создание экземпляра (dynamic нельзя использовать для вызова конструктора)
            Object[] args = new Object[] { 12 }; // Аргументы конструктора
            Console.WriteLine("x before constructor called: " + args[0]);
            dynamic obj = Activator.CreateInstance(t, args);
            Console.WriteLine("Type: " + obj.GetType().ToString());
            Console.WriteLine("x after constructor returns: " + args[0]);

            // Чтение и запись поля
            try
            {
                obj.m_someField = 5;
                Int32 v = (Int32)obj.m_someField;
                Console.WriteLine("someField: " + v);
            }
            catch (RuntimeBinderException e)
            {
                // Получает управление, потому что поле является приватным
                Console.WriteLine("Failed to access field: " + e.Message);
            }
            // Вызов метода
            String s = (String)obj.ToString();
            Console.WriteLine("ToString: " + s);
            // Чтение и запись свойства
            try
            {
                obj.SomeProp = 0;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Property set catch.");
            }
            obj.SomeProp = 2;
            Int32 val = (Int32)obj.SomeProp;
            Console.WriteLine("SomeProp: " + val);
            // Добавление и удаление делегата для события
            obj.SomeEvent += new EventHandler(EventCallback);
            //obj.SomeEvent = new EventHandler(EventCallback); // ошибка
        }
    }

}