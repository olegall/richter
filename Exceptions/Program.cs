using System;

namespace Exceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            new Wrapper().Main_();
            // throw vs throw e
            
            var s = new Sentence(null);
            s.GetFirstCharacter();
            Sentence.GetFirstCharacterNullable();

            ConstrainedExecutionRegion.Demo1();
            ConstrainedExecutionRegion.Demo2();

            new ShoppingCart().AddItem(new Item());
        }
    }
}
