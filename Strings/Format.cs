using System;

namespace Strings
{
    class Format
    {
        public Format()
        {
            { String s = String.Format("On {0}, {1} is {2} years old.", new DateTime(2012, 4, 22, 14, 35, 5), "Aidan", 9); }
            { String s = String.Format("On {0:D}, {1} is {2:E} years old.", new DateTime(2012, 4, 22, 14, 35, 5), "Aidan", 9); }
        }
    }
}
