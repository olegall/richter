using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics
{
    internal sealed class PrimaryConstraintOfStream<T> where T : Stream
    {
        public void M(T stream)
        {
            stream.Close();// OK
        }
    }
    /*
        если в исходном тексте явно указать System.Object, компилятор C# выдаст ошибку
        (ошибка CS0702: в ограничении не может использоваться специальный класс object):
        error CS0702: Constraint cannot be special class 'object'     
     */

    class MainRestrictions
    {

    }
}
