using System;
using System.Runtime.InteropServices;

namespace GarbageCollector
{
    // Тип определяется в пространстве имен System.Runtime.InteropServices
    public struct GCHandle
    {
        // Статические методы, создающие элементы таблицы
        public static GCHandle Alloc(object value);
        public static GCHandle Alloc(object value, GCHandleType type);
        // Статические методы, преобразующие GCHandle в IntPtr
        public static explicit operator IntPtr(GCHandle value);
        public static IntPtr ToIntPtr(GCHandle value);
        // Статические методы, преобразующие IntPtr в GCHandle
        public static explicit operator GCHandle(IntPtr value);
        public static GCHandle FromIntPtr(IntPtr value);
        // Статические методы, сравнивающие два типа GCHandles
        public static Boolean operator ==(GCHandle a, GCHandle b);
        public static Boolean operator !=(GCHandle a, GCHandle b);
        // Экземплярный метод, освобождающий элемент таблицы (индекс равен 0)
        public void Free();
        // Экземплярное свойство, извлекающее/назначающее
        // для элемента ссылку на объект
        public object Target { get; set; }
        // Экземплярное свойство, равное true при отличном от 0 индексе
        public Boolean IsAllocated { get; }
        // Для элементов с флагом pinned возвращается адрес объекта
        public IntPtr AddrOfPinnedObject();
        public override Int32 GetHashCode();
        public override Boolean Equals(object o);
    }
}
