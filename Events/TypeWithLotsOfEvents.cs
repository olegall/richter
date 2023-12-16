using System;

namespace Events
{
    // Определение типа, унаследованного от EventArgs для этого события
    public class FooEventArgs : EventArgs 
    {
    }

    public class TypeWithLotsOfEvents
    {
        // Определение закрытого экземплярного поля, ссылающегося на коллекцию. Коллекция управляет множеством пар "Event/Delegate"
        // Примечание: Тип EventSet не входит в FCL, это мой собственный тип
        private readonly EventSet m_eventSet = new EventSet();

        // Защищенное свойство позволяет производным типам работать с коллекцией
        protected EventSet EventSet 
        { 
            get 
            { 
                return m_eventSet;
            } 
        }

        #region Code to support the Foo event (repeat this pattern for additional events)
        // Определение членов, необходимых для события Foo.

        // 2a. Создайте статический, доступный только для чтения объект для идентификации события.
        // Каждый объект имеет свой хеш-код для нахождения связанного списка делегатов события в коллекции.
        protected static readonly EventKey s_fooEventKey = new EventKey();

        // 2b. Определение для события методов доступа для добавления или удаления делегата из коллекции. когда сработает? aleek
        public event EventHandler<FooEventArgs> Foo
        {
            // обязательно и add и remove
            add
            {
                m_eventSet.Add(s_fooEventKey, value); 
            }

            remove
            {
                m_eventSet.Remove(s_fooEventKey, value); 
            }
        }

        // 2c. Определение защищенного виртуального метода On для этого события.
        protected virtual void OnFoo(FooEventArgs e)
        {
            m_eventSet.Raise(s_fooEventKey, this, e);
        }

        // 2d. Определение метода, преобразующего входные данные этого события
        public void SimulateFoo()
        {
            OnFoo(new FooEventArgs());
        }
        #endregion
    }
}