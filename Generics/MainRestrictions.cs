using System.IO;

namespace Generics
{
    internal sealed class PrimaryConstraintOfStream<T> where T : Stream
    {
        public void M(T stream)
        {
            stream.Close(); // OK
        }
    }

    internal sealed class PrimaryConstraintOfStream_<Object> where Object : Stream // aleek
    {
        public void M(object stream)
        {
            //stream.Close();
        }
    }
    
    //если в исходном тексте явно указать System.Object, компилятор C# выдаст ошибку
    //(ошибка CS0702: в ограничении не может использоваться специальный класс object):
    //error CS0702: Constraint cannot be special class 'object'

    internal sealed class PrimaryConstraintOfClass1<T> where T : class
    {
        public void M()
        {
            T temp = null; // Допустимо, потому что тип T должен быть ссылочным
        }
    }

    internal sealed class PrimaryConstraintOfClass2<T> where T : class, new() // что значит new()? aleek
    {
        public static T Factory()
        {
            return new T();
        }
    }
    
    internal sealed class PrimaryConstraintOfStruct1<T> where T : struct
    {
        public void M()
        {
            //T temp = null; // ошибка, т.к. структура - тип значения. Ошибка	CS0403	Невозможно преобразовать Null к параметру типа "T", так как он может быть типом значения, не допускающим значения Null. Используйте вместо этого "default(T)"
            T temp = default(T); // так правильно
        }
    }

    internal sealed class PrimaryConstraintOfStruct2<T> where T : struct
    {
        public static T Factory()
        {
            // Допускается, потому что у каждого значимого типа неявно есть открытый конструктор без параметров
            return new T();
        }
    }

    /// <summary>
    /// Основные ограничения
    /// </summary>
    class MainRestrictions
    {

    }
}