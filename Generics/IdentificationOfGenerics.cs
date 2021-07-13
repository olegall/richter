﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DateTimeList = System.Collections.Generic.List<System.DateTime>;

namespace Generics
{
    internal sealed class DateTimeList : List<DateTime>
    {
        // Здесь никакой код добавлять не нужно!
    }

    class IdentificationOfGenerics : IRunnable
    {
        public void Run() 
        {
            List<DateTime> dt = new List<DateTime>();
            //DateTimeList dt = new DateTimeList();
            Boolean sameType = typeof(List<DateTime>) == typeof(DateTimeList);
            Boolean sameType2 = typeof(List<DateTime>) == typeof(DateTimeList);
        }
    }

    internal sealed class SomeType
    {
        private static void SomeMethod()
        {
            // Компилятор определяет, что dtl имеет тип
            // System.Collections.Generic.List<System.DateTime>
            var dtl = List<DateTime>();
        
        }

        // сгенерирован
        private static object List<T>()
        {
            throw new NotImplementedException();
        }
    }
}
