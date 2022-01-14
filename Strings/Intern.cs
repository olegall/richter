using System;

namespace Strings
{
    class Intern
    {
        void Main() 
        {
            String s1 = "Hello";
            String s2 = "Hello";
            Console.WriteLine(Object.ReferenceEquals(s1, s2)); // Должно быть 'False'

            s1 = String.Intern(s1);
            s2 = String.Intern(s2);
            Console.WriteLine(Object.ReferenceEquals(s1, s2)); // 'True'
        }

        private static Int32 NumTimesWordAppearsEquals(String word, String[] wordlist)
        {
            Int32 count = 0;
            for (Int32 wordnum = 0; wordnum < wordlist.Length; wordnum++)
            {
                if (word.Equals(wordlist[wordnum], StringComparison.Ordinal))
                    count++;
            }
            return count;
        }

        private static Int32 NumTimesWordAppearsIntern(String word, String[] wordlist)
        {
            // В этом методе предполагается, что все элементы в wordlist ссылаются на интернированные строки
            word = String.Intern(word);
            Int32 count = 0;
            for (Int32 wordnum = 0; wordnum < wordlist.Length; wordnum++)
            {
                if (Object.ReferenceEquals(word, wordlist[wordnum]))
                    count++;
            }
            return count;
        }
    }
}
