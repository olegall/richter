using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace CompanyA
//{
//    public class Phone
//    {
//        public void Dial()
//        {
//            Console.WriteLine("Phone.Dial");
//            // Выполнить действия по набору телефонного номера
//        }
//    }
//}

namespace CompanyA
{
    public class Phone
    {
        public void Dial()
        {
            Console.WriteLine("Phone.Dial");
            EstablishConnection();
            // Выполнить действия по набору телефонного номера
        }

        protected virtual void EstablishConnection()
        {
            Console.WriteLine("Phone.EstablishConnection");
            // Выполнить действия по установлению соединения
        }
    }
}

//namespace CompanyB
//{
//    public class BetterPhone : CompanyA.Phone
//    {
//        public void Dial()
//        {
//            Console.WriteLine("BetterPhone.Dial");
//            EstablishConnection();
//            base.Dial();
//        }

//        protected virtual void EstablishConnection()
//        {
//            Console.WriteLine("BetterPhone.EstablishConnection");
//            // Выполнить действия по набору телефонного номера
//        }
//    }
//}

//namespace CompanyB
//{
//    public class BetterPhone : CompanyA.Phone
//    {
//        // Этот метод Dial никак не связан с одноименным методом класса Phone
//        public new void Dial()
//        {
//            Console.WriteLine("BetterPhone.Dial");
//            EstablishConnection();
//            base.Dial();
//        }

//        protected virtual void EstablishConnection()
//        {
//            Console.WriteLine("BetterPhone.EstablishConnection");
//            // Выполнить действия по установлению соединения
//        }
//    }
//}

//namespace CompanyB
//{
//    public class BetterPhone : CompanyA.Phone
//    {
//        // Ключевое слово 'new' оставлено, чтобы указать, что этот метод не связан с методом Dial базового типа
//        public new void Dial()
//        {
//            Console.WriteLine("BetterPhone.Dial");
//            EstablishConnection();
//            base.Dial();
//        }

//        // Ключевое слово 'new' указывает, что этот метод не связан с методом EstablishConnection базового типа
//        protected new virtual void EstablishConnection()
//        {
//            Console.WriteLine("BetterPhone.EstablishConnection");
//            // Выполнить действия для установления соединения
//        }
//    }
//}

namespace CompanyB
{
    public class BetterPhone : CompanyA.Phone
    {
        // Метод Dial удален (так как он наследуется от базового типа)
        // Здесь ключевое слово new удалено, а модификатор virtual заменен на override, чтобы указать, что этот метод связан с методом EstablishConnection из базового типа
        protected override void EstablishConnection()
        {
            Console.WriteLine("BetterPhone.EstablishConnection");
            // Выполнить действия по установлению соединения
        }
    }
}

namespace Types
{
    public class Virtual
    {
        public void Main_()
        {
            CompanyB.BetterPhone phone = new CompanyB.BetterPhone();
            phone.Dial();

            /*
             BetterPhone.Dial
             BetterPhone.EstablishConnection
             Phone.Dial
             */

            /*
             BetterPhone.Dial
             BetterPhone.EstablishConnection
             Phone.Dial
             Phone.EstablishConnection
             */

            /*
             Phone.Dial
             BetterPhone.EstablishConnection
             */
        }
    }
}
