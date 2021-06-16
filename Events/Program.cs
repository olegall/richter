using System;

namespace Events
{
    class Program
    {
        static void Main(string[] args)
        {
            TypeWithLotsOfEvents twle = new TypeWithLotsOfEvents();

            // Добавление обратного вызова
            twle.Foo += HandleFooEvent;

            // Проверяем работоспособность
            twle.SimulateFoo();

            Console.ReadLine();
        }

        private static void HandleFooEvent(object sender, FooEventArgs e)
        {
            Console.WriteLine("Handling Foo Event here...");
        }
    }
}
