using System;

namespace Arrays
{
    public static class DynamicArrays // где динамика?
    {
        public static void Main_()
        //static DynamicArrays() // модификатор доступа нельзя aleek
        {
            // Требуется двухмерный массив [2005..2009][1..4]
            Int32[] lowerBounds = { 2005, 1 };
            Int32[] lengths = { 5, 4 };
            Decimal[,] quarterlyRevenue = (Decimal[,])Array.CreateInstance(typeof(Decimal), lengths, lowerBounds);
            Console.WriteLine("{0,4} {1,9} {2,9} {3,9} {4,9}", "Year", "Q1", "Q2", "Q3", "Q4");

            Int32 firstYear = quarterlyRevenue.GetLowerBound(0);
            Int32 lastYear = quarterlyRevenue.GetUpperBound(0);
            Int32 firstQuarter = quarterlyRevenue.GetLowerBound(1);
            Int32 lastQuarter = quarterlyRevenue.GetUpperBound(1);

            for (Int32 year = firstYear; year <= lastYear; year++)
            {
                Console.Write(year + " ");
                for (Int32 quarter = firstQuarter; quarter <= lastQuarter; quarter++)
                {
                    Console.Write("{0,9:C} ", quarterlyRevenue[year, quarter]);
                }
                Console.WriteLine();
            }

            /*
                Year Q1 Q2 Q3 Q4
                2005 $0.00 $0.00 $0.00 $0.00
                2006 $0.00 $0.00 $0.00 $0.00
                2007 $0.00 $0.00 $0.00 $0.00
                2008 $0.00 $0.00 $0.00 $0.00
                2009 $0.00 $0.00 $0.00 $0.00
            */
        }
    }
}
