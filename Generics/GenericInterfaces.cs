using System;
using System.Collections;

namespace Generics
{
                                      
    public interface IEnumerator<T> : IDisposable, IEnumerator
    {
        T Current { get; }
        //new T Current { get; }
        //override T Current { get; }
    }

    class Point { }

    //internal sealed class Triangle : IEnumerator<Point>
    internal sealed class Triangle
    {
        private Point[] m_vertices;

        // Тип свойства Current в IEnumerator<Point> - это Point
        public Point Current 
        { 
            get 
            { 
                return new Point(); 
            } 
        }

        public void Dispose()
        { 
        }
    }

    //internal sealed class ArrayEnumerator<T> : IEnumerator<T>
    //{
    //    private T[] m_array;

    //    // Тип свойства Current в IEnumerator<T> — T
    //    public T Current { get { ... } }
    //}

    class GenericInterfaces
    {
    }
}
