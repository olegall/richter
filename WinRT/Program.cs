using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WinRT
{
    #region FCL aleek
    //public static class WindowsRuntimeSystemExtensions
    //{
    //    public static TaskAwaiter GetAwaiter(this IAsyncAction source);
    //    public static TaskAwaiter GetAwaiter<TProgress>(this IAsyncActionWithProgress<TProgress> source);
    //    public static TaskAwaiter<TResult> GetAwaiter<TResult>(this IAsyncOperation<TResult> source);
    //    public static TaskAwaiter<TResult> GetAwaiter<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> source);
    //}

    //public static class WindowsRuntimeSystemExtensions
    //{
    //    public static Task AsTask<TProgress>(this IAsyncActionWithProgress<TProgress> source, CancellationToken cancellationToken, IProgress<TProgress> progress);
    //    public static Task<TResult> AsTask<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> source, CancellationToken cancellationToken, IProgress<TProgress> progress);
    //    // Более простые перегруженные версии не показаны
    //}

    //public static class WindowsRuntimeStorageExtensions
    //{
    //    public static Task<Stream> OpenStreamForReadAsync(this IStorageFile file);
    //    public static Task<Stream> OpenStreamForWriteAsync(this IStorageFile file);
    //    public static Task<Stream> OpenStreamForReadAsync(this IStorageFolder rootDirectory, String relativePath);
    //    public static Task<Stream> OpenStreamForWriteAsync(this IStorageFolder rootDirectory, String relativePath, CreationCollisionOption creationCollisionOption);
    //}

    //public static class WindowsRuntimeStreamExtensions
    //{
    //    public static Stream AsStream(this IRandomAccessStream winRTStream);
    //    public static Stream AsStream(this IRandomAccessStream winRTStream, Int32 bufferSize);
    //    public static Stream AsStreamForRead(this IInputStream winRTStream);
    //    public static Stream AsStreamForRead(this IInputStream winRTStream, Int32 bufferSize);
    //    public static Stream AsStreamForWrite(this IOutputStream winRTStream);
    //    public static Stream AsStreamForWrite(this IOutputStream winRTStream, Int32 bufferSize);
    //    public static IInputStream AsInputStream(this Stream clrStream);
    //    public static IOutputStream AsOutputStream(this Stream clrStream);
    //}

    public interface IBuffer
    {
        UInt32 Capacity { get; } // Максимальный размер буфера (в байтах)
        UInt32 Length { get; set; } // Количество используемых байтов
    }

    //[Guid("905a0fefbc5311df8c49001e4fc686da")]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[ComImport]
    //internal interface IBufferByteAccess
    //{
    //    unsafe Byte* Buffer { get; }
    //}

    //public static class WindowsRuntimeBufferExtensions
    //{
    //    public static IBuffer AsBuffer(this Byte[] source);
    //    public static IBuffer AsBuffer(this Byte[] source, Int32 offset, Int32 length);
    //    public static IBuffer AsBuffer(this Byte[] source, Int32 offset, Int32 length, Int32 capacity);
    //    public static IBuffer GetWindowsRuntimeBuffer(this MemoryStream stream);
    //    public static IBuffer GetWindowsRuntimeBuffer(this MemoryStream stream, Int32 position, Int32 length);
    //}

    public interface IOutputStream : IDisposable
    {
        IAsyncOperationWithProgress<UInt32, UInt32> WriteAsync(IBuffer buffer);
    }

    //public static class WindowsRuntimeBufferExtensions
    //{
    //    public static Stream AsStream(this IBuffer source);
    //    public static Byte[] ToArray(this IBuffer source);
    //    public static Byte[] ToArray(this IBuffer source, UInt32 sourceIndex,  Int32 count);
    //    // Не показано: метод CopyTo для передачи байтов между IBuffer и Byte[]
    //    // Не показано: методы GetByte, IsSameData
    //}
    #endregion

    #region aleek
    internal class StorageFile
    {
        internal Task<Stream> OpenStreamForReadAsync()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    class SomeOtherException : Exception
    {
        public SomeOtherException()
        {
        }

        public SomeOtherException(string message) : base(message)
        {
        }

        public SomeOtherException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SomeOtherException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    class WinRTType { }

    public interface IAsyncOperation<T> { }

    public interface IAsyncOperationWithProgress<TResult, TProgress>
    {
        Action<object, object> Progress { get; set; }
        Action<object, object> Completed { get; set; }

        void Cancel();
    }

    public enum AsyncStatus {
        Completed,
        Canceled,
        Error
    }

    internal interface IInputStream
    {
        Stream AsStreamForRead();
    }

    internal interface IRandomAccessStream
    {
        Task ReadAsync(object value, uint length, object none);
    }
    #endregion

    internal sealed class MyClass
    {
        private CancellationTokenSource m_cts = new CancellationTokenSource();
        // ВНИМАНИЕ: при вызове из потока графического интерфейса весь код выполняется в этом потоке:
        private async void MappingWinRTAsyncToDotNet(WinRTType someWinRTObj)
        {
            try
            {
                // Предполагается, что XxxAsync возвращает IAsyncOperationWithProgress<IBuffer, UInt32>
                //IBuffer result = await someWinRTObj.XxxAsync(...).AsTask(m_cts.Token, new Progress<UInt32>(ProgressReport));
                /* Завершение... */
            }
            catch (OperationCanceledException) { /* Отмена... */ }
            catch (SomeOtherException) { /* Ошибка... */ }
        }

        private void ProgressReport(UInt32 progress) { /* Оповещение... */ }
        public void Cancel() { m_cts.Cancel(); } // Вызывается позднее
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        //public void WinRTAsyncIntro()
        //{
        //    IAsyncOperation<StorageFile> asyncOp = KnownFolders.MusicLibrary.GetFileAsync("Song.mp3");
        //    asyncOp.Completed = OpCompleted;
        //    // Возможно, позднее будет вызван метод asyncOp.Cancel()
        //}

        public async void WinRTAsyncIntro()
        {
            try
            {
                //StorageFile file = await KnownFolders.MusicLibrary.GetFileAsync("Song.mp3");
                /* Завершение... */
            }
            catch (OperationCanceledException) { /* Отмена... */ }
            catch (SomeOtherException ex) { /* Ошибка... */ }
        }

        //private void OpCompleted(IAsyncOperation<StorageFile> asyncOp, AsyncStatus status)
        //{
        //    switch (status)
        //    {
        //        case AsyncStatus.Completed: // Обработка результата
        //            StorageFile file = asyncOp.GetResults(); /* Завершено... */ break;
        //        case AsyncStatus.Canceled: // Обработка отмены
        //            /* Canceled... */
        //            break;
        //        case AsyncStatus.Error: // Обработка исключения
        //            Exception exception = asyncOp.ErrorCode; /* Ошибка... */ break;
        //    }
        //    asyncOp.Close();
        //}

        //public static Task<TResult> AsTask<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> asyncOp, CancellationToken ct = default(CancellationToken), IProgress<TProgress> progress = null)
        public static Task<TResult> AsTask<TResult, TProgress>(IAsyncOperationWithProgress<TResult, TProgress> asyncOp, CancellationToken ct = default, IProgress<TProgress> progress = null)
        {
            // При отмене CancellationTokenSource отменить асинхронную операцию
            ct.Register(() => asyncOp.Cancel());

            // Когда асинхронная операция оповещает о прогрессе, оповещение передается методу обратного вызова
            //asyncOp.Progress = (asyncInfo, p) => progress.Report(p);
            // Объект TaskCompletionSource наблюдает за завершением асинхронной операции
            
            var tcs = new TaskCompletionSource<TResult>();
            // При завершении асинхронной операции оповестить TaskCompletionSource. Когда это происходит, управление возвращается коду, ожидающему завершения TaskCompletionSource.
            asyncOp.Completed = (asyncOp2, asyncStatus) => {
                switch (asyncStatus)
                {
                    //case AsyncStatus.Completed: tcs.SetResult(asyncOp2.GetResults()); break;
                    case AsyncStatus.Canceled: tcs.SetCanceled(); break;
                    //case AsyncStatus.Error: tcs.SetException(asyncOp2.ErrorCode); break;
                }
            };
            return tcs.Task;
        }

        async Task<XElement> FromStorageFileToXElement(StorageFile file)
        {
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                return XElement.Load(stream);
            }
        }

        XElement FromWinRTStreamToXElement(IInputStream winRTStream)
        {
            Stream netStream = winRTStream.AsStreamForRead();
            return XElement.Load(netStream);
        }

        private async Task ByteArrayAndStreamToIBuffer(IRandomAccessStream winRTStream, Int32 count)
        {
            Byte[] bytes = new Byte[count];
            //await winRTStream.ReadAsync(bytes.AsBuffer(), (UInt32)bytes.Length, InputStreamOptions.None);
            Int32 sum = bytes.Sum(b => b); // Обращение к прочитанным байтам через Byte[]
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            {
                sw.Write("This string represents data in a stream");
                sw.Flush();
                //UInt32 bytesWritten = await winRTStream.WriteAsync(ms.GetWindowsRuntimeBuffer());
            }
        }
    }
}
