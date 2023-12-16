using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security;
using System.Security.Permissions;

namespace Serialization
{
    class Customer { }

    class Order { }

    internal struct Point 
    { 
        public Int32 x, y; 
    }

    [Serializable]
    internal class Circle
    {
        private Double m_radius;

        [NonSerialized]
        private Double m_area;

        public Circle(Double radius)
        {
            m_radius = radius;

            m_area = Math.PI * m_radius * m_radius;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            m_area = Math.PI * m_radius * m_radius;
        }
    }

    [Serializable]
    public class MyType
    {
        Int32 x, y;

        [NonSerialized] Int32 sum;

        public MyType(Int32 x, Int32 y)
        {
            this.x = x; 
            this.y = y;

            sum = x + y;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            // Пример. Присвоение полям значений по умолчанию в новой версии типа
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Пример. Инициализация временного состояния полей
            sum = x + y;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            // Пример. Модификация состояния перед сериализацией
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            // Пример. Восстановление любого состояния после сериализации
        }
    }

    class Common
    {
        private static List<Customer> s_customers = new List<Customer>();
        private static List<Order> s_pendingOrders = new List<Order>();
        private static List<Order> s_processedOrders = new List<Order>();

        private static void SaveApplicationState(Stream stream)
        {
            // Конструирование модуля форматирования для сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            // Сериализация всего состояния приложения
            formatter.Serialize(stream, s_customers);
            formatter.Serialize(stream, s_pendingOrders);
            formatter.Serialize(stream, s_processedOrders);
        }

        private static void RestoreApplicationState(Stream stream)
        {
            // Конструирование модуля форматирования сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            // Десериализация состояния приложения (выполняется в том же порядке, что и сериализация)
            s_customers = (List<Customer>)formatter.Deserialize(stream);
            s_pendingOrders = (List<Order>)formatter.Deserialize(stream);
            s_processedOrders = (List<Order>)formatter.Deserialize(stream);
        }

        private static Object DeepClone(Object original)
        {
            // Создание временного потока в памяти
            using (MemoryStream stream = new MemoryStream())
            {
                // Создания модуля форматирования для сериализации
                BinaryFormatter formatter = new BinaryFormatter();

                // Эта строка описывается в разделе "Контексты потока ввода-вывода"
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);

                // Сериализация графа объекта в поток в памяти
                formatter.Serialize(stream, original);

                // Возвращение к началу потока в памяти перед десериализацей
                stream.Position = 0;

                // Десериализация графа в новый набор объектов и возвращение корня графа (детальной копии) вызывающему методу
                return formatter.Deserialize(stream);
            }
        }

        private static void OptInSerialization()
        {
            Point pt = new Point { x = 1, y = 2 };

            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, pt); // исключение SerializationException
            }
        }

        private static void SingletonSerializationTest()
        {
            // Создание массива с несколькими ссылками на один объект Singleton
            Singleton[] a1 = { Singleton.GetSingleton(), Singleton.GetSingleton() };

            Console.WriteLine("Do both elements refer to the same object? " + (a1[0] == a1[1])); // "True"

            using (var stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Сериализация и десериализация элементов массива
                formatter.Serialize(stream, a1);
                stream.Position = 0;
                Singleton[] a2 = (Singleton[])formatter.Deserialize(stream);

                // Проверяем, что все работает, как нужно:
                Console.WriteLine("Do both elements refer to the same object? " + (a2[0] == a2[1])); // "True"
                Console.WriteLine("Do all elements refer to the same object? " + (a1[0] == a2[0])); // "True"
            }
        }

        private static void SerializationSurrogateDemo()
        {
            using (var stream = new MemoryStream())
            {
                // 1. Создание желаемого модуля форматирования
                IFormatter formatter = new SoapFormatter();

                // 2. Создание объекта SurrogateSelector
                SurrogateSelector ss = new SurrogateSelector();

                // 3. Селектор выбирает наш суррогат для объекта DateTime
                ss.AddSurrogate(typeof(DateTime), formatter.Context, new UniversalToLocalTimeSerializationSurrogate());
                // ПРИМЕЧАНИЕ. AddSurrogate можно вызывать более одного раза для регистрации нескольких суррогатов

                // 4. Модуль форматирования использует наш селектор
                formatter.SurrogateSelector = ss;

                // Создание объекта DateTime с локальным временем машины и его сериализация
                DateTime localTimeBeforeSerialize = DateTime.Now;
                formatter.Serialize(stream, localTimeBeforeSerialize);

                // Поток выводит универсальное время в виде строки, проверяя, что все работает
                stream.Position = 0;
                Console.WriteLine(new StreamReader(stream).ReadToEnd());

                // Десериализация универсального времени и преобразование объекта DateTime в локальное время
                stream.Position = 0;
                DateTime localTimeAfterDeserialize = (DateTime)formatter.Deserialize(stream);

                // Проверка корректности работы
                Console.WriteLine("LocalTimeBeforeSerialize ={0}", localTimeBeforeSerialize);
                Console.WriteLine("LocalTimeAfterDeserialize={0}", localTimeAfterDeserialize);
            }
        }
    }

    [Serializable]
    public class Dictionary<TKey, TValue> : ISerializable, IDeserializationCallback
    {
        //private IEqualityComparer<TKey> m_comparer = new EqualityComparer<TKey>();
        private IEqualityComparer<TKey> m_comparer;
        private object m_version = new object();
        private Int32[] m_buckets = new Int32[] { };
        private const int Count = 0;
        private int m_freeList = 0;
        IEnumerable<Entry<TKey, TValue>> m_entries;

        // Здесь закрытые поля (не показанные)
        private SerializationInfo m_siInfo; // Только для десериализации
                                            // Специальный конструктор (необходимый интерфейсу ISerializable) для управления десериализацией
        
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected Dictionary(SerializationInfo info, StreamingContext context)
        {
            // Во время десериализации сохраним SerializationInfo для OnDeserialization
            m_siInfo = info;
        }

        // Метод управления сериализацией
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", m_version);
            info.AddValue("Comparer", m_comparer, typeof(IEqualityComparer<TKey>));
            info.AddValue("HashSize", (m_buckets == null) ? 0 : m_buckets.Length);
            if (m_buckets != null)
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Count];
                //CopyTo(array, 0);
                info.AddValue("KeyValuePairs", array, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        // Метод, вызываемый после десериализации всех ключей/значений объектов
        //public virtual void IDeserializationCallback.OnDeserialization(Object sender)
        void IDeserializationCallback.OnDeserialization(Object sender) // фикс ошибки Рихтера
        {
            if (m_siInfo == null) 
                return; // Никогда не присваивается, возвращение управления

            Int32 num = m_siInfo.GetInt32("Version");
            Int32 num2 = m_siInfo.GetInt32("HashSize");

            m_comparer = (IEqualityComparer<TKey>)
            m_siInfo.GetValue("Comparer", typeof(IEqualityComparer<TKey>));

            if (num2 != 0)
            {
                m_buckets = new Int32[num2];

                for (Int32 i = 0; i < m_buckets.Length; i++) 
                {
                    m_buckets[i] = -1; 
                }

                m_entries = new Entry<TKey, TValue>[num2];
                m_freeList = -1;

                KeyValuePair<TKey, TValue>[] pairArray = (KeyValuePair<TKey, TValue>[])m_siInfo.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
                
                //if (pairArray == null)
                //    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);

                for (Int32 j = 0; j < pairArray.Length; j++)
                {
                    //if (pairArray[j].Key == null)
                    //    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_NullKey);

                    Insert(pairArray[j].Key, pairArray[j].Value, true);
                }
            }
            else 
            { 
                m_buckets = null; 
            }

            m_version = num;
            m_siInfo = null;
        }

        #region aleek
        private void Insert(TKey key, TValue value, bool v)
        {
            throw new NotImplementedException();
        }

        private class Entry<TKey, TValue>
        {
        }
        #endregion
    }

    [Serializable]
    internal class Base
    {
        protected String m_name = "Jeff";
        
        public Base() { /* Наделяем тип способностью создавать экземпляры */ }
    }

    [Serializable]
    internal class Derived : Base, ISerializable
    {
        private DateTime m_date = DateTime.Now;
        
        public Derived() { /* Наделяем тип способностью создавать экземпляры */ }
        
        // Если конструктор не существует, выдается SerializationException. Если класс не запечатан, конструктор должен быть защищенным.
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        private Derived(SerializationInfo info, StreamingContext context)
        {
            // Получение набора сериализуемых членов для нашего и базовых классов
            Type baseType = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(baseType, context);
            
            // Десериализация полей базового класса из объекта данных
            for (Int32 i = 0; i < mi.Length; i++)
            {
                // Получение поля и присвоение ему десериализованного значения
                FieldInfo fi = (FieldInfo)mi[i];
                fi.SetValue(this, info.GetValue(baseType.FullName + "+" + fi.Name, fi.FieldType));
            }
            
            // Десериализация значений, сериализованных для этого класса
            m_date = info.GetDateTime("Date");
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Сериализация нужных значений для этого класса
            info.AddValue("Date", m_date);

            // Получение набора сериализуемых членов для нашего и базовых классов
            Type baseType = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(baseType, context);

            // Сериализация полей базового класса в объект данных
            for (Int32 i = 0; i < mi.Length; i++)
            {
                // Полное имя базового типа ставим в префикс имени поля
                info.AddValue(baseType.FullName + "+" + mi[i].Name, ((FieldInfo)mi[i]).GetValue(this));
            }
        }

        public override String ToString()
        {
            return String.Format("Name={0}, Date={1}", m_name, m_date);
        }
    }

    // Разрешен только один экземпляр типа на домен
    [Serializable]
    public sealed class Singleton : ISerializable
    {
        // Единственный экземпляр этого типа
        private static readonly Singleton theOneObject = new Singleton();

        // Поля экземпляра
        public String Name = "Jeff";
        public DateTime Date = DateTime.Now;

        // Закрытый конструктор для создания однокомпонентного типа
        private Singleton() { }

        // Метод, возвращающий ссылку на одноэлементный тип
        public static Singleton GetSingleton() { return theOneObject; }

        // Метод, вызываемый при сериализации объекта Singleton
        // Рекомендую использовать явную реализацию интерфейсного метода.
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(SingletonSerializationHelper));
            // Добавлять другие значения не нужно
        }

        [Serializable]
        private sealed class SingletonSerializationHelper : IObjectReference
        {
            // Метод, вызываемый после десериализации этого объекта (без полей)
            public Object GetRealObject(StreamingContext context)
            {
                return Singleton.GetSingleton();
            }
        }

        // ПРИМЕЧАНИЕ. Специальный конструктор НЕ НУЖЕН, потому что он нигде не вызывается
    }

    internal sealed class UniversalToLocalTimeSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context)
        {
            // Переход от локального к мировому времени
            info.AddValue("Date", ((DateTime)obj).ToUniversalTime().ToString("u"));
        }

        public Object SetObjectData(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            // Переход от мирового времени к локальному
            return DateTime.ParseExact(info.GetString("Date"), "u", null).ToLocalTime();
        }
    }

    internal sealed class Ver1ToVer2SerializationBinder : SerializationBinder
    {
        class Ver2 { };

        public override Type BindToType(String assemblyName, String typeName)
        {
            // Десериализация объекта Ver1 из версии 1.0.0.0 в объект Ver2. Вычисление имени сборки, определяющей тип Ver1
            AssemblyName assemVer1 = Assembly.GetExecutingAssembly().GetName();
            assemVer1.Version = new Version(1, 0, 0, 0);

            // При десериализации объекта Ver1 версии v1.0.0.0 превращаем его в Ver2
            if (assemblyName == assemVer1.ToString() && typeName == "Ver1")
                return typeof(Ver2);
            
            // В противном случае возвращаем запрошенный тип
            return Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
        }
    }
}