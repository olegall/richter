using System;

namespace Delegates
{
    public class Program
    {
        static void Main(string[] args) 
        {
            Console.WriteLine("*** \nДополнительные средства управления цепочками делегатов ***");
            FamiliarityWithDelegates.StaticDelegateDemo();
            FamiliarityWithDelegates.InstanceDelegateDemo();
            FamiliarityWithDelegates.ChainDelegateDemo1(new Program());
            FamiliarityWithDelegates.ChainDelegateDemo2(new Program());

            Console.WriteLine("*** \nДополнительные средства управления цепочками делегатов ***");
            new AdditionalMeans().Run();

            Console.WriteLine("*** \nДелегаты и отражение ***");
            DelegateReflection.Run();
            Console.ReadLine();
        }
    }
}
