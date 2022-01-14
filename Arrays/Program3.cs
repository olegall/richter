using System;

namespace Arrays
{
    public static class Program3
    {
        private const Int32 c_numElements = 10000;

        public static void Main()
        {
            // Объявление двухмерного массива
            Int32[,] a2Dim = new Int32[c_numElements, c_numElements];

            // Объявление нерегулярного двухмерного массива (вектор векторов)
            Int32[][] aJagged = new Int32[c_numElements][];

            for (Int32 x = 0; x < c_numElements; x++)
                aJagged[x] = new Int32[c_numElements];

            // 1: Обращение к элементам стандартным, безопасным способом
            Safe2DimArrayAccess(a2Dim);

            // 2: Обращение к элементам с использованием нерегулярного массива
            SafeJaggedArrayAccess(aJagged);

            // 3: Обращение к элементам небезопасным методом
            Unsafe2DimArrayAccess(a2Dim);
        }

        private static Int32 Safe2DimArrayAccess(Int32[,] a)
        {
            Int32 sum = 0;
            for (Int32 x = 0; x < c_numElements; x++)
            {
                for (Int32 y = 0; y < c_numElements; y++)
                {
                    sum += a[x, y];
                }
            }
            return sum;
        }

        private static Int32 SafeJaggedArrayAccess(Int32[][] a)
        {
            Int32 sum = 0;
            for (Int32 x = 0; x < c_numElements; x++)
            {
                for (Int32 y = 0; y < c_numElements; y++)
                {
                    sum += a[x][y];
                }
            }
            return sum;
        }

        private static unsafe Int32 Unsafe2DimArrayAccess(Int32[,] a)
        {
            Int32 sum = 0;
            fixed (Int32* pi = a)
            {
                for (Int32 x = 0; x < c_numElements; x++)
                {
                    Int32 baseOfDim = x * c_numElements;
                    for (Int32 y = 0; y < c_numElements; y++)
                    {
                        sum += pi[baseOfDim + y];
                    }
                }
            }
            return sum;
        }
    }
}
