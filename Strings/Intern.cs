using System;

namespace Strings
{
    class Intern
    {
        public Intern()
        {
            String s1 = "Hello";
            String s2 = "Hello";
            Console.WriteLine(Object.ReferenceEquals(s1, s2)); // Должно быть 'False'
            var a1 = Object.ReferenceEquals(s1, s2);
            
            s1 = String.Intern(s1);
            s2 = String.Intern(s2);
            Console.WriteLine(Object.ReferenceEquals(s1, s2)); // 'True'
            var a2 = Object.ReferenceEquals(s1, s2);

            NumTimesWordAppearsEquals(null, null);
            NumTimesWordAppearsIntern(null, null);
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