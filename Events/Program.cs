﻿using System;
using WrapTwoInterfaceEvents;

namespace Events
{
    class Program
    {
        static void Main(string[] args)
        {
            TypeWithLotsOfEvents twle = new TypeWithLotsOfEvents();

            // Добавление обратного вызова
            // на событие завязываем обратный вызов. есть событие - д.б. коллбэк
                        // не сработает. позже
            twle.Foo += HandleFooEvent;

            // Проверяем работоспособность
            twle.SimulateFoo();

            Shape shape = new Shape();
            Subscriber1 sub = new Subscriber1(shape);
            Subscriber2 sub2 = new Subscriber2(shape);
            shape.Draw();

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            Console.ReadLine();
            /* Output:
                Sub1 receives the IDrawingObject event.
                Drawing a shape.
                Sub2 receives the IShape event.
            */
        }

        // коллбэк сработает после вызова DynamicInvoke
        private static void HandleFooEvent(object sender, FooEventArgs e)
        {
            Console.WriteLine("Handling Foo Event here...");
        }
    }
}
