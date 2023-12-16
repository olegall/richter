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
                
                // если закомментировать все throw - не все пути к коду возвращают значение. если вовращает void - норм
                throw; // какой смысл? видимо надо обработать в try возможную искл. ситуацию, а в catch уже осознанно бросить
                       // CLR сама определит тип исключения и использует дефолтное сообщение. бросится искл. соотв типа
                throw e;
                // throw, throw e - результат 1 и тот же. когда throw e - информации по идее больше

                throw new Exception();
                throw new NullReferenceException("No character found");
            }
        }
        
        public static Nullable<char> GetFirstCharacterNullable()
        {
            try
            {
                return null;
            }
            catch (NullReferenceException e)
            {
                throw;
            }
        }
        
        // public static Nullable<char> GetFirstCharacterNullable() => null; // перегрузка по nullable не работает
    }
}
