using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics
{
    public interface IEnumerator<T> : IDisposable//, IEnumerator
    {
        T Current { get; }
    }

    class Point { }

    internal sealed class Triangle : IEnumerator<Point>
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

    class GenericInterfaces
    {
    }
}
