using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics
{
    public delegate TReturn CallMe<TReturn, TKey, TValue>(TKey key, TValue value);

    class GenericDelegates
    {
        //public sealed class CallMe<TReturn, TKey, TValue> : MulticastDelegate
        //{
        //    public CallMe(Object object, IntPtr method);
        //    public virtual TReturn Invoke(TKey key, TValue value);
        //    public virtual IAsyncResult BeginInvoke(TKey key, TValue value,
        //    AsyncCallback callback, Object object);
        //    public virtual TReturn EndInvoke(IAsyncResult result);
        //}
    }
}
