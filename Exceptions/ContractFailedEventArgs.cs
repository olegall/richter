using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public sealed class ContractFailedEventArgs : EventArgs
    {
        //public ContractFailedEventArgs(ContractFailureKind failureKind, String message, String condition, Exception originalException);
        public ContractFailureKind FailureKind { get; }
        public String Message { get; }
        public String Condition { get; }
        public Exception OriginalException { get; }
        public Boolean Handled { get; } // Верно, если хоть один обработчик
      
        // вызвал SetHhandled
        //public void SetHandled(); // Присваивает Handled значение true,
                                  // позволяя игнорировать нарушение
        public Boolean Unwind { get; } // Верно, если хоть один обработчик
                                       // вызвал SetUnwind или threw
        //public void SetUnwind(); // Присваивает Unwind значение true,
                                 // принудительно генерируя ContractException
    }
}
