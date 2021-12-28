using System;

namespace Exceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            //var wrapper = new Wrapper();
            //// throw vs throw e
            //wrapper.TextException();
            //wrapper.ExceptionWithoutTry();
            //var s = new Sentence(null);
            //s.GetFirstCharacter();
            //Sentence.GetFirstCharacterNullable();

            //wrapper.MultipleThrows();

            //ConstrainedExecutionRegion.Demo1();
            ConstrainedExecutionRegion.Demo2();

            new ShoppingCart().AddItem(new Item());
        }
    }
}
