﻿using System;

namespace Exceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            var wrapper = new Wrapper();
            // throw vs throw e
            wrapper.TextException();
            wrapper.ExceptionWithoutTry();
            var s = new Sentence(null);
            Console.WriteLine($"The first character is {s.GetFirstCharacter()}");
            Sentence.GetFirstCharacterNullable();

            wrapper.MultipleThrows();
        }
    }
}
