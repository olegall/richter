using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Events
{
    /// <summary>
    /// Реализация событий компилятором
    /// </summary>
    class RealizationOfEventsByCompiler
    {
        // 1. ЗАКРЫТОЕ поле делегата, инициализированное значением null
        private EventHandler<NewMailEventArgs> NewMail = null;
        // 2. ОТКРЫТЫЙ метод add_Xxx (где Xxx – это имя события)
        // Позволяет объектам регистрироваться для получения уведомлений о событии
        public void add_NewMail(EventHandler<NewMailEventArgs> value)
        {
            // Цикл и вызов CompareExchange – хитроумный способ добавления
            // делегата способом, безопасным в отношении потоков
            EventHandler<NewMailEventArgs> prevHandler;
            EventHandler<NewMailEventArgs> newMail = this.NewMail;
            do
            {
                prevHandler = newMail;
                EventHandler<NewMailEventArgs> newHandler =
                (EventHandler<NewMailEventArgs>)Delegate.Combine(prevHandler, value);
                newMail = Interlocked.CompareExchange<EventHandler<NewMailEventArgs>>(
                ref this.NewMail, newHandler, prevHandler);
            } while (newMail != prevHandler);
        }
        // 3. ОТКРЫТЫЙ метод remove_Xxx (где Xxx – это имя события)
        // Позволяет объектам отменять регистрацию в качестве
        // получателей уведомлений о cобытии
        public void remove_NewMail(EventHandler<NewMailEventArgs> value)
        {
            // Цикл и вызов CompareExchange – хитроумный способ
            // удаления делегата способом, безопасным в отношении потоков
            EventHandler<NewMailEventArgs> prevHandler;
            EventHandler<NewMailEventArgs> newMail = this.NewMail;
            do
            {
                prevHandler = newMail;
                EventHandler<NewMailEventArgs> newHandler =
                (EventHandler<NewMailEventArgs>)Delegate.Remove(prevHandler, value);
                newMail = Interlocked.CompareExchange<EventHandler<NewMailEventArgs>>(
                ref this.NewMail, newHandler, prevHandler);
            } while (newMail != prevHandler);
        }
    }
}
