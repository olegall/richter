using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Events
{
    // Этап 1. Определение типа для хранения информации,
    // которая передается получателям уведомления о событии
    internal class NewMailEventArgs : EventArgs
    {
        private readonly String m_from, m_to, m_subject;
        public NewMailEventArgs(String from, String to, String subject)
        {
            m_from = from; m_to = to; m_subject = subject;
        }
        public String From { get { return m_from; } }
        public String To { get { return m_to; } }
        public String Subject { get { return m_subject; } }
    }

    public static class EventArgExtensions
    {
        public static void Raise<TEventArgs>(this TEventArgs e,
        Object sender, ref EventHandler<TEventArgs> eventDelegate)
        {
            // Копирование ссылки на поле делегата во временное поле
            // для безопасности в отношении потоков
            EventHandler<TEventArgs> temp = Volatile.Read(ref eventDelegate);
            // Если зарегистрированный метод заинтересован в событии, уведомите его
            if (temp != null) temp(sender, e);
        }
    }

    class MailManager
    {
        // Этап 2. Определение члена-события
        public event EventHandler<NewMailEventArgs> NewMail;

        // Этап 3. Определение метода, ответственного за уведомление
        // зарегистрированных объектов о событии
        // Если этот класс изолированный, нужно сделать метод закрытым
        // или невиртуальным
        protected virtual void OnNewMail(NewMailEventArgs e)
        {
            // Сохранить ссылку на делегата во временной переменной
            // для обеспечения безопасности потоков
            EventHandler<NewMailEventArgs> temp = Volatile.Read(ref NewMail);
            // Если есть объекты, зарегистрированные для получения
            // уведомления о событии, уведомляем их
            if (temp != null) temp(this, e);
        }

        // Версия 2
        //protected void OnNewMail(NewMailEventArgs e)
        //{
        //    EventHandler<NewMailEventArgs> temp = NewMail;
        //    if (temp != null) temp(this, e);
        //}

        // Версия 3
        //protected void OnNewMail(NewMailEventArgs e)
        //{
        //    EventHandler<NewMailEventArgs> temp = Thread.VolatileRead(ref NewMail);
        //    if (temp != null) temp(this, e);
        //}

        //protected virtual void OnNewMail(NewMailEventArgs e)
        //{
        //    e.Raise(this, ref m_NewMail);
        //}

        // Этап 4. Определение метода, преобразующего входную
        // информацию в желаемое событие
        public void SimulateNewMail(String from, String to, String subject)
        {
            // Создать объект для хранения информации, которую
            // нужно передать получателям уведомления
            NewMailEventArgs e = new NewMailEventArgs(from, to, subject);
            // Вызвать виртуальный метод, уведомляющий объект о событии
            // Если ни один из производных типов не переопределяет этот метод,
            // объект уведомит всех зарегистрированных получателей уведомления
            OnNewMail(e);
        }
    }
}
