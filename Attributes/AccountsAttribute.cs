using System;
using System.Collections.Generic;
using System.Text;

namespace Attributes
{
    [Flags]
    internal enum Accounts
    {
        Savings = 0x0001,
        Checking = 0x0002,
        Brokerage = 0x0004
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class AccountsAttribute : Attribute
    {
        private Accounts m_accounts;
        public AccountsAttribute(Accounts accounts)
        {
            m_accounts = accounts;
        }
        public override Boolean Match(Object obj)
        {
            // Если в базовом классе реализован метод Match и это не
            // класс Attribute, раскомментируйте следующую строку
            // if (!base.Match(obj)) return false;
            // Так как 'this' не равен null, если obj равен null,
            // объекты не совпадают
            // ПРИМЕЧАНИЕ. Эту строку можно удалить, если вы считаете,
            // что базовый тип корректно реализует метод Match
            if (obj == null) return false;
            // Объекты разных типов не могут быть равны
            // ПРИМЕЧАНИЕ. Эту строку можно удалить, если вы считаете,
            // что базовый тип корректно реализует метод Match
            if (this.GetType() != obj.GetType()) return false;
            // Приведение obj к нашему типу для доступа к полям
            // ПРИМЕЧАНИЕ. Это приведение всегда работает,
            // так как объекты принадлежат к одному типу
            AccountsAttribute other = (AccountsAttribute)obj;
            // Сравнение полей
            // Проверка, является ли accounts 'this' подмножеством
            // accounts объекта others
            if ((other.m_accounts & m_accounts) != m_accounts)
                return false;

            return true; // Объекты совпадают
        }

        public override Boolean Equals(Object obj)
        {
            // Если в базовом классе реализован метод Equals и это
            // не класс Object, раскомментируйте следующую строку
            // if (!base.Equals(obj)) return false;
            // Так как 'this' не равен null, при obj равном null
            // объекты не совпадают
            // ПРИМЕЧАНИЕ. Эту строку можно удалить, если вы считаете,
            // что базовый тип корректно реализует метод Equals
            if (obj == null) return false;
            // Объекты разных типов не могут совпасть
            // ПРИМЕЧАНИЕ. Эту строку можно удалить, если вы считаете,
            // что базовый тип корректно реализует метод Equals
            if (this.GetType() != obj.GetType()) return false;
            // Приведение obj к нашему типу для получения доступа к полям
            // ПРИМЕЧАНИЕ. Это приведение работает всегда,
            // так как объекты принадлежат к одному типу
            AccountsAttribute other = (AccountsAttribute)obj;
            // Сравнение значений полей 'this' и other
            if (other.m_accounts != m_accounts)
                return false;
            return true; // Объекты совпадают
        }

        // Переопределяем GetHashCode, так как Equals уже переопределен
        public override Int32 GetHashCode()
        {
            return (Int32)m_accounts;
        }

        /*
         Построение и запуск этого приложения приводит к следующему результату:
            ChildAccount types can NOT write checks.
            AdultAccount types can write checks.
            Program types can NOT write checks.
         */
    }
}
