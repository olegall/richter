using System;

namespace Strings
{
    class Intern
    {
        public Intern()
        {
            String s1 = "Hello";
            String s2 = "Hello";
            var a1 = Object.ReferenceEquals(s1, s2); // Должно быть 'False'. true. видимо срабатывает интернирование строк - aleek
            
            s1 = String.Intern(s1);
            s2 = String.Intern(s2);
            var a2 = Object.ReferenceEquals(s1, s2); // 'True'

            #region #aleek
            String s3 = "Hello";
            String s4 = "Hello_";
            var a3 = Object.ReferenceEquals(s3, s4); // false

            s3 = s4; var a4 = Object.ReferenceEquals(s3, s4);
            //s4 = s3; var a5 = Object.ReferenceEquals(s3, s4);
            #endregion

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