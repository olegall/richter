using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Exceptions
{
    [Serializable]
    public abstract class ExceptionArgs
    {
        public virtual String Message 
        { 
            get 
            { 
                return String.Empty; 
            } 
        }
    }

    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable where TExceptionArgs : ExceptionArgs
    {
        private const String c_args = "Args"; // Для (де)сериализации
        private readonly TExceptionArgs m_args;
        
        public TExceptionArgs Args 
        { 
            get 
            { 
                return m_args; 
            } 
        }
        
        public Exception(String message = null, Exception innerException = null) : this(null, message, innerException) 
        {
        }

        public Exception(TExceptionArgs args, String message = null, Exception innerException = null) : base(message, innerException)
        {
            m_args = args;
        }

        // Конструктор для десериализации; так как класс запечатан, конструктор закрыт. Для незапечатанного класса конструктор должен быть защищенным
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        private Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            m_args = (TExceptionArgs)info.GetValue(c_args, typeof(TExceptionArgs));
        }

        // Метод для сериализации; он открыт из-за интерфейса ISerializable
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(c_args, m_args);
            base.GetObjectData(info, context);
        }

        public override String Message
        {
            get
            {
                String baseMsg = base.Message;
                return (m_args == null) ? baseMsg : baseMsg + " (" + m_args.Message + ")";
            }
        }

        public override Boolean Equals(Object obj)
        {
            Exception<TExceptionArgs> other = obj as Exception<TExceptionArgs>;
            if (obj == null) return false;
            return Object.Equals(m_args, other.m_args) && base.Equals(obj);
        }

        public override int GetHashCode() 
        {
            return base.GetHashCode(); 
        }
    }
}