using System;

namespace Exceptions
{
    [Serializable]
    public sealed class DiskFullExceptionArgs : ExceptionArgs
    {
        private readonly String m_diskpath; // закрытое поле, задается во время создания
        public DiskFullExceptionArgs(String diskpath) 
        { 
            m_diskpath = diskpath; 
        }

        // Открытое предназначенное только для чтения свойство, которое возвращает поле
        public String DiskPath 
        { 
            get 
            { 
                return m_diskpath; 
            }
        }

        // Переопределение свойства Message для включения в него нашего поля
        public override String Message
        {
            get
            {
                return (m_diskpath == null) ? base.Message : "DiskPath=" + m_diskpath;
            }
        }
    }
}
