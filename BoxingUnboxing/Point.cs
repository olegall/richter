using System;

namespace BoxingUnboxingPoint
{
    internal struct Point : IComparable
    {
        private Int32 m_x, m_y;

        // Конструктор, просто инициализирующий поля
        public Point(Int32 x, Int32 y)
        {
            m_x = x;
            m_y = y;
        }

        // Переопределяем метод ToString, унаследованный от System.ValueType
        public override String ToString()
        {
            // Возвращаем Point как строку (вызов ToString предотвращает упаковку)
            return String.Format("({0}, {1})", m_x.ToString(), m_y.ToString()); // состояние структуры возвращаем?
        }

        // Безопасная в отношении типов реализация метода CompareTo - почему "Безопасная в отношении типов"? aleek
        public Int32 CompareTo(Point other)
        {
            // Используем теорему Пифагора для определения точки, наиболее удаленной от начала координат (0, 0)
            return Math.Sign(Math.Sqrt(m_x * m_x + m_y * m_y) - Math.Sqrt(other.m_x * other.m_x + other.m_y * other.m_y));
        }

        // Реализация метода CompareTo интерфейса IComparable
        public Int32 CompareTo(Object o)
        {
            if (GetType() != o.GetType())
            {
                throw new ArgumentException("o is not a Point");
            }

            // Вызов безопасного в отношении типов метода CompareTo
            return CompareTo((Point)o);
        }
    }
}
