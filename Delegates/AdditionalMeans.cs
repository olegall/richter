using System;
using System.Text;

namespace Delegates
{
    // Определение компонента Light
    internal sealed class Light
    {
        // Метод возвращает состояние объекта Light
        public String SwitchPosition()
        {
            return "The light is off";
        }
    }

    // Определение компонента Fan
    internal sealed class Fan
    {
        // Метод возвращает состояние объекта Fan
        public String Speed()
        {
            throw new InvalidOperationException("The fan broke due to overheating");
        }
    }

    // Определение компонента Speaker
    internal sealed class Speaker
    {
        // Метод возвращает состояние объекта Speaker
        public String Volume()
        {
            return "The volume is loud";
        }
    }

    // private delegate String GetStatusNamespace(); // нельзя объявить в пространстве

    public class AdditionalMeans
    {
        // объявляется только в классе. в методе нельзя
        // делегат - это класс. объявляется класс. не принимает параметров, возвращает string
        private delegate String GetStatus(); // GetStatus - микс класса и метода с сигнатурой aleek
        //private delegate Object GetStatus();
        //private Delegate Object GetStatus();

        private String Foo()
        {
            return "";
        }
        
        private String Foo2()
        {
            return "";
        }

        // Метод запрашивает состояние компонентов и возвращает информацию
        private static String GetComponentStatusReport(GetStatus status)
        {
            // Если цепочка пуста, ничего делать не нужно
            if (status == null) 
            { 
                return null; 
            }

            // Построение отчета о состоянии
            StringBuilder report = new StringBuilder();

            // Создание массива из делегатов цепочки
            Delegate[] arrayOfDelegates = status.GetInvocationList();

            // Циклическая обработка делегатов массива
            foreach (GetStatus getStatus in arrayOfDelegates)
            {
                try
                {
                    // Получение строки состояния компонента и добавление ее в отчет
                    report.AppendFormat("{0}{1}{1}", getStatus(), Environment.NewLine);
                }
                catch (InvalidOperationException e)
                {
                    // В отчете генерируется запись об ошибке для этого компонента
                    Object component = getStatus.Target;
                    report.AppendFormat(
                        "Failed to get status from {1}{2}{0} Error: {3}{0}{0}",
                        Environment.NewLine,
                        ((component == null) ? "" : component.GetType() + "."),
                        getStatus.Method.Name,
                        e.Message
                    );
                }
            }
            // Возвращение сводного отчета вызывающему коду
            return report.ToString();
        }

        public void CompareDelegates()
        {
            GetStatus del1 = Foo;
            GetStatus del2 = Foo2;
            var a1 = del1 == del2;
            var a2 = del1 != del2;
            var a3 = del1 + del2;
            var a4 = del1 - del2;
            //var a5 = del1 * del2;

            //del1.BeginInvoke();

            Delegate d1 = null;
            Delegate d2 = null;
            var b1 = d1 == d2;
            var b2 = d1 != d2;
            //var b3 = d1 + d2;
            //var b4 = d1 - d2;
            //var b5 = d1 * d2;
        }

        public void Main_()
        {
            // Объявление пустой цепочки делегатов
            GetStatus getStatus = null;

            // Создание трех компонентов и добавление в цепочку методов проверки их состояния
            getStatus += new GetStatus(new Light().SwitchPosition);
            getStatus += new GetStatus(new Fan().Speed);
            getStatus += new GetStatus(new Speaker().Volume);

            // Сводный отчет о состоянии трех компонентов
            Console.WriteLine(GetComponentStatusReport(getStatus));
            CompareDelegates();
        }
    }
}
