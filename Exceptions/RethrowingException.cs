using System;

namespace Exceptions
{
    public class Sentence
    {
        public Sentence(string s)
        {
            Value = s;
        }

        public string Value { get; set; }

        public char GetFirstCharacter()
        {
            try
            {
                return Value[0]; // останавливается, а не проходит в catch
            }
            catch (NullReferenceException e)
            {
                //return Value[0]; // в catch - не все пути к коду возвращают значение, в try норм.
                
                // закомментировать -  не все пути к коду возвращают значение. если вовращает void - норм
                throw; // какой смысл? видимо надо обработать в try возможную искл. ситуацию, а в catch уже осознанно бросить
                throw e;
                throw new Exception();
            }
        }
        
        // нет перегрузки из-за char?
        //public static char? GetFirstCharacterNullable()
        public static Nullable<char> GetFirstCharacterNullable()
        {
            try
            {
                return null; // в catch не попадёт, т.к. char? - допускает null
            }
            catch (NullReferenceException e)
            {
                throw;
            }
        }
    }
}
