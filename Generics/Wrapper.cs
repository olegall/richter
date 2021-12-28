using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Generics
{
    //[Serializable]
    //public class List<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    //{
    //    public List();
    //    public void Add(T item);
    //    public Int32 BinarySearch(T item);
    //    public void Clear();
    //    public Boolean Contains(T item);
    //    public Int32 IndexOf(T item);
    //    public Boolean Remove(T item);
    //    public void Sort();
    //    public void Sort(IComparer<T> comparer);
    //    public void Sort(Comparison<T> comparison);
    //    public T[] ToArray();
    //    public Int32 Count { get; }
    //    public T this[Int32 index] { get; set; }
    //}

    class Wrapper
    {
        private static void SomeMethod()
        {
            // Создание списка (List), работающего с объектами DateTime
            List<DateTime> dtList = new List<DateTime>();
            
            // Добавление объекта DateTime в список
            dtList.Add(DateTime.Now); // Без упаковки. Добавление еще одного объекта DateTime в список
            dtList.Add(DateTime.MinValue); // Без упаковки. Попытка добавить объект типа String в список
            //dtList.Add("1/1/2004"); // Ошибка компиляции. Извлечение объекта DateTime из списка
            
            DateTime dt = dtList[0]; // Приведение типов не требуется
        }

        public static void ValueTypePerfTest()
        {
            const Int32 count = 10000000;

            using (new OperationTimer("List<Int32>"))
            {
                List<Int32> list = new List<Int32>();

                for (Int32 n = 0; n < count; n++)
                {
                    list.Add(n); // Без упаковки
                    Int32 x = list[n]; // Без распаковки
                }

                list = null; // Для удаления в процессе уборки мусора
            }

            using (new OperationTimer("ArrayList of Int32"))
            {
                ArrayList arrayList = new ArrayList();

                for (Int32 n = 0; n < count; n++)
                {
                    arrayList.Add(n); // Упаковка
                    Int32 x = (Int32)arrayList[n]; // Распаковка
                }

                arrayList = null; // Для удаления в процессе уборки мусора
            }
        }

        public static void ReferenceTypePerfTest()
        {
            const Int32 count = 10000000;

            using (new OperationTimer("List<String>"))
            {
                List<String> list = new List<String>();

                for (Int32 n = 0; n < count; n++)
                {
                    list.Add("X"); // Копирование ссылки
                    String x = list[n]; // Копирование ссылки
                }

                list = null; // Для удаления в процессе уборки мусора
            }

            using (new OperationTimer("ArrayList of String"))
            {
                ArrayList arrayList = new ArrayList();

                for (Int32 n = 0; n < count; n++)
                {
                    arrayList.Add("X"); // Копирование ссылки
                    String x = (String)arrayList[n]; // Проверка преобразования
                } // и копирование ссылки

                arrayList = null; // Для удаления в процессе уборки мусора
            }
        }

        public void GenericDelegates() 
        {
        }
    }
}
