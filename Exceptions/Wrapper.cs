using System;
using System.IO;
using System.Runtime.Serialization;

namespace Exceptions
{
    public sealed class ReliabilityContractAttribute : Attribute
    {
        //public ReliabilityContractAttribute(Consistency consistencyGuarantee, Cer cer);
        public Cer Cer { get; }
        public Consistency ConsistencyGuarantee { get; }
    }

    public enum Consistency { MayCorruptProcess, MayCorruptAppDomain, MayCorruptInstance, WillNotCorruptState  }

    public enum Cer { None, MayFail, Success }

    public class Wrapper
    {
        private void SomeMethod()
        {
            try
            {
                // Код, требующий корректного восстановления или очистки ресурсов
            }
            catch (InvalidOperationException)
            {
                // Код восстановления работоспособности после исключения InvalidOperationException
            }
            catch (IOException)
            {
                // Код восстановления работоспособности после исключения IOException
            }
            catch
            {
                // Код восстановления работоспособности после остальных исключений.
                // После перехвата исключений их обычно генерируют повторно (зачем?)
                // Эта тема будет рассмотрена позже
                throw;
            }
            finally
            {
                // Здесь находится код, выполняющий очистку ресурсов после операций, начатых в блоке try. 
                // Этот код выполняется ВСЕГДА вне зависимости от наличия исключения
            }
            
            // Код, следующий за блоком finally, выполняется, если в блоке try
            // не генерировалось исключение или если исключение было перехвачено
            // блоком catch, а новое не генерировалось
        }

        //private void SomeMethod()
        //{
        //    try
        //    {
        //        // Внутрь блока try помещают код, требующий корректного
        //        // восстановления работоспособности или очистки ресурсов
        //    }
        //    catch (Exception e)
        //    {
        //        // До C# 2.0 этот блок перехватывал только CLS-совместимые исключения
        //        // В C# 2.0 этот блок научился перехватывать также
        //        // CLS-несовместимые исключения
        //        throw; // Повторная генерация перехваченного исключения
        //    }
        //    catch
        //    {
        //        // Во всех версиях C# этот блок перехватывает
        //        // и совместимые, и несовместимые с CLS исключения
        //        throw; // Повторная генерация перехваченного исключения
        //    }
        //}

        //private void SomeMethod()
        //{
        //    try { 
        //        //... 
        //    }
        //    catch (Exception e)
        //    {
        //        //...
        //         throw e; // CLR считает, что исключение возникло тут
        //                  // FxCop сообщает об ошибке
        //    }
        //}
        
        //private void SomeMethod()
        //{
        //    try 
        //    { 
        //        //...
        //    }
        //    catch (Exception e)
        //    {
        //         //...
        //         throw; // CLR не меняет информацию о начальной точке исключения.
        //                // FxCop НЕ сообщает об ошибке
        //    }
        //}

        //private void SomeMethod()
        //{
        //    Boolean trySucceeds = false;

        //    try
        //    {
        //        //...
        //        trySucceeds = true;
        //    }
        //    finally
        //    {
        //        if (!trySucceeds) 
        //        { 
        //            /* код перехвата исключения */ 
        //        }
        //    }
        //}

        private void ReadData(String pathname)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(pathname, FileMode.Open);
                // Обработка данных в файле
            }
            catch (IOException)
            {
                // Код восстановления после исключения IOException
            }
            finally
            {
                // Файл обязательно следует закрыть
                if (fs != null)
                    fs.Close();
            }
        }

        #region aleek
        // throw new Exception() в разных частях программы (конструктор, try, catch, ...)

        private void ExceptionWithoutTry()
        { 
            throw new Exception();

            // следующий код будет недостижимым
            var a1 = 0;

            if (a1 == 0)
            {
            }
        }

        private void MultipleThrows()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                // почему можно 3 throw? если поменять порядок?
                throw;
                throw e;
                throw new Exception();
            }
        }

        // не важно какой возвращаемый тип. почему?
        private int NoMatterType()
        //private void NoMatterType()
        //private string NoMatterType()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int GenericException<T>() where T : Exception, new()
        {
            try
            {
                throw new T();
            }
            catch (T e)
            {
                throw e;
            }
        }

        private void TextGenericException()
        {
            try
            {
                throw new Exception<DiskFullExceptionArgs>(new DiskFullExceptionArgs(@"C:\"), "The disk is full");
            }
            catch (Exception<DiskFullExceptionArgs> e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e) // не вызовется раз сработал предыдущий
            {
                Console.WriteLine(e.Message);
            }

            //try
            //{
            //    throw new Exception<DiskFullExceptionArgs>(new DiskFullExceptionArgs(@"C:\"), "The disk is full");
            //}
            //catch (Exception e)
            //{
            //}
            //catch (Exception<DiskFullExceptionArgs> e)
            //{
            //}

            //try { throw new Exception(); }
            //catch (Exception e) { }
            //catch (Exception e) { }// повторно нельзя

            //try { throw new Exception(); }
            //catch (Exception e) { }
            //catch (ArgumentNullException e) { } // нельзя - Exception всё перехватил

            //try { throw new Exception(); }
            //catch (ArgumentNullException e) { }
            //catch (FileNotFoundException e) { } // обязательно производные?
            //catch (Exception e) { }

            try
            {
                throw new Exception(); 
            }
            catch (Exception<DiskFullExceptionArgs> e)// не сработает - Exception<DiskFullExceptionArgs> более специфично чем Exception
            {
                Console.WriteLine(e.Message);
            }
            
            try
            {
                //throw new Exception(); 
            }
            catch (ArgumentException e)// не сработает - ArgumentException более специфична чем Exception
            {
            }
            
            try
            {
                throw new ArgumentException();
            }
            catch (Exception e) // сработает
            {
            }

            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                try
                {
                    //var a1 = throw new Exception(); // нельзя приравнять
                    throw new Exception(); // сработает catch
                    new Exception(); // в catch не попадёт
                }
                catch (Exception e2)
                {
                    var a1 = e.Message;
                    var a2 = e2.Message;
                    var a3 = e;

                    /* 
                     * когда создаёшь объект, сразу исключение бросается? пытаюсь увидеть просто объект new Exception(). 
                     * Скорее всего да - это объект. Что происходит с программой после new Exception()? 
                     * Какая разница - бросить в try или без try? в try - обратоется в catch, программа продолжит выполняться, без try - программа упадёт
                     */

                    var a4 = new Exception();
                    //var a5 = throw new Exception(); // в catch нельзя бросать исключения. Почему?
                    //throw e; // а так можно. хотя объект тот же. "Выдано исключение типа "System.Exception"."
                    // программа аварийно завершается. дальнейший код не выполняется. Как завершается? С ошибкой?
                    //throw; // "Выдано исключение типа "System.Exception"."
                }
            }

            try
            {
                throw new Exception();
            }
            catch (Exception)
            {
            }

            try
            {
                throw new Exception();
            }
            catch
            {
            }

            try
            {
                throw new Exception();
            }
            catch
            {
                throw;
            }

            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                throw e;
                throw e.InnerException;
                throw new Exception();
                throw new Exception();
                throw;
                var res1 = e.Message;
            }
        }
        #endregion

        public String CalculateSpreadsheetCell(Int32 row, Int32 column)
        {
            String result = null;

            try
            {
                //result = /* Код для расчета значения ячейки электронной таблицы */
            }
            catch (DivideByZeroException)
            {
                result = "Нельзя отобразить значение: деление на ноль";
            }
            catch (OverflowException)
            {
                result = "Нельзя отобразить значение: оно слишком большое";
            }

            return result;
        }

        public void SerializeObjectGraph(FileStream fs, IFormatter formatter, Object rootObj)
        {
            // Сохранение текущей позиции в файле
            Int64 beforeSerialization = fs.Position;

            try
            {
                // Попытка сериализовать граф объекта и записать его в файл
                formatter.Serialize(fs, rootObj);
            }
            catch
            { // Перехват всех исключений
              // При ЛЮБОМ повреждении файл возвращается в нормальное состояние
                fs.Position = beforeSerialization;
                // Обрезаем файл
                fs.SetLength(fs.Position);
                // ПРИМЕЧАНИЕ: предыдущий код не помещен в блок finally,
                // так как сброс потока требуется только при сбое сериализации
                // Уведомляем вызывающий код о происходящем,
                // снова генерируя ТО ЖЕ САМОЕ исключение
                throw;
            }
        }

        private static void SomeMethod(String filename)
        {
            try
            {
            }
            catch (IOException e)
            {
                // Добавление имени файла к объекту IOException
                e.Data.Add("Filename", filename);
                throw; // повторное генерирование того же исключения
            }
        }

        private static void Reflection(Object o)
        {
            try
            {
                // Вызов метода DoSomething для этого объекта
                System.Reflection.MethodInfo mi = o.GetType().GetMethod("DoSomething");
                mi.Invoke(o, null); // Метод DoSomething может сгенерировать исключение
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                // CLR преобразует его в TargetInvocationException
                throw e.InnerException; // Повторная генерация исходного исключения
            }
        }

        private static void DisplayFirstNumber(string[] args)
        {
            string arg = args.Length >= 1 ? args[0] : throw new ArgumentException("You must supply an argument");

            if (Int64.TryParse(arg, out var number))
            //if (Int64.TryParse(arg, out long number))
                Console.WriteLine($"You entered {number:F0}");
            else
                Console.WriteLine($"{arg} is not a number.");
        }

        public void Main_() 
        {
            SomeMethod();

            ExceptionWithoutTry();
            MultipleThrows();
            NoMatterType();
            //GenericException<Object>();
            TextGenericException();
            CalculateSpreadsheetCell(0, 0);
            SerializeObjectGraph(null, null, null);
            SomeMethod(null);
            Reflection(null);
            DisplayFirstNumber(null);
        }
    }
}