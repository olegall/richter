using System;

namespace GarbageCollector
{
    public abstract class SafeHandle : CriticalFinalizerObject, IDisposable
    {
        // Это дескриптор системного ресурса
        protected IntPtr handle;

        protected SafeHandle(IntPtr invalidHandleValue, Boolean ownsHandle)
        {
            this.handle = invalidHandleValue;
            // Если значение ownsHandle равно true, то системный ресурс закрывается при уничтожении объекта, производного от SafeHandle, уборщиком мусора
        }

        protected void SetHandle(IntPtr handle)
        {
            this.handle = handle;
        }

        // Явное освобождение ресурса выполняется вызовом метода Dispose
        public void Dispose() { Dispose(true); }

        // Здесь подойдет стандартная реализация метода Dispose. Настоятельно не рекомендуется переопределять этот метод!
        protected virtual void Dispose(Boolean disposing)
        {
            // В стандартной реализации аргумент, вызывающий метод Dispose, игнорируется
            // Если ресурс уже освобожден, управление возвращается коду
            // Если значение ownsHandle равно false, управление возвращается
            // Установка флага, означающего, что этот ресурс был освобожден
           
            // Вызов виртуального метода ReleaseHandle
            // Вызов GC.SuppressFinalize(this), отменяющий вызов метода финализации
            // Если значение ReleaseHandle равно true, управление возвращается коду
            // Если управление передано в эту точку, запускается ReleaseHandleFailed Managed Debugging Assistant (MDA)
        }

        // Здесь подходит стандартная реализация метода финализации. Настоятельно не рекомендуется переопределять этот метод!
        ~SafeHandle() 
        {
            Dispose(false); 
        }

        // Производный класс переопределяет этот метод, чтобы реализовать код освобождения ресурса
        protected abstract Boolean ReleaseHandle();
        
        public void SetHandleAsInvalid()
        {
            // Установка флага, означающего, что этот ресурс был освобожден
            // Вызов GC.SuppressFinalize(this), отменяющий вызов метода финализации
        }
        
        public Boolean IsClosed
        {
            get
            {
                // Возвращение флага, показывающего, был ли ресурс освобожден
            }
        }

        public abstract Boolean IsInvalid
        {
            // Производный класс переопределяет это свойство
            // Реализация должна вернуть значение true, если значение дескриптора не представляет ресурс (обычно это значит, что дескриптор равен 0 или @1)
            get
        }

        // Эти три метода имеют отношение к безопасности и подсчету ссылок. Подробнее о них рассказывается в конце этого раздела
        public void DangerousAddRef(ref Boolean success) { }
        public IntPtr DangerousGetHandle() { return new IntPtr(); }
        public void DangerousRelease() { }
    }
}