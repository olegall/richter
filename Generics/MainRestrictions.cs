using System.IO;

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

    internal sealed class PrimaryConstraintOfClass<T> where T : class
    {
        public void M()
        {
            T temp = null;// Допустимо, потому что тип T должен быть ссылочным
        }
    }
    
    internal sealed class PrimaryConstraintOfClass2<T> where T : struct
    {
        public void M()
        {
            //T temp = null;
        }
    }
    
    internal sealed class PrimaryConstraintOfClass3<T> where T : struct
    {
        public void M()
        {
            //T temp = null;
        }
    }

    internal sealed class PrimaryConstraintOfStruct<T> where T : struct
    {
        public static T Factory()
        {
            // Допускается, потому что у каждого значимого типа неявно
            // есть открытый конструктор без параметров
            return new T();
        }
    }
    
    internal sealed class PrimaryConstraintOfClass4<T> where T : class
    {
        /*public static T Factory()
        {
            return new T();
        }*/
    }

    /// <summary>
    /// Основные ограничения
    /// </summary>
    class MainRestrictions
    {

    }
}