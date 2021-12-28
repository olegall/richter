using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Richter
{
    #region Приоритеты запросов ввода-вывода
    internal static class ThreadIO
    {
        public static BackgroundProcessingDisposer BeginBackgroundProcessing(Boolean process = false)
        {
            ChangeBackgroundProcessing(process, true);
            return new BackgroundProcessingDisposer(process);
        }

        public static void EndBackgroundProcessing(Boolean process = false)
        {
            ChangeBackgroundProcessing(process, false);
        }

        private static void ChangeBackgroundProcessing(Boolean process, Boolean start)
        {
            Boolean ok = process ? 
                SetPriorityClass(GetCurrentWin32ProcessHandle(), start ? ProcessBackgroundMode.Start : ProcessBackgroundMode.End) :
                SetThreadPriority(GetCurrentWin32ThreadHandle(), start ? ThreadBackgroundgMode.Start : ThreadBackgroundgMode.End);

            if (!ok) 
                throw new Win32Exception();
        }

        // Эта структура позволяет инструкции using выйти из режима фоновой обработки
        public struct BackgroundProcessingDisposer : IDisposable
        {
            private readonly Boolean m_process;

            public BackgroundProcessingDisposer(Boolean process)
            {
                m_process = process;
            }

            public void Dispose() // называется Dispose, т.к. общепринятое название. Будут искать по имени этого метода
            {
                EndBackgroundProcessing(m_process);
            }
        }

        // См. Win32-функции THREAD_MODE_BACKGROUND_BEGIN и THREAD_MODE_BACKGROUND_END
        private enum ThreadBackgroundgMode { Start = 0x10000, End = 0x20000 }

        // См. Win32-функции PROCESS_MODE_BACKGROUND_BEGIN и PROCESS_MODE_BACKGROUND_END
        private enum ProcessBackgroundMode { Start = 0x100000, End = 0x200000 }

        [DllImport("Kernel32", EntryPoint = "GetCurrentProcess", ExactSpelling = true)]
        private static extern SafeWaitHandle GetCurrentWin32ProcessHandle();

        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern Boolean SetPriorityClass(SafeWaitHandle hprocess, ProcessBackgroundMode mode);
        [DllImport("Kernel32", EntryPoint = "GetCurrentThread", ExactSpelling = true)]

        private static extern SafeWaitHandle GetCurrentWin32ThreadHandle();
        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]

        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean SetThreadPriority(SafeWaitHandle hthread, ThreadBackgroundgMode mode);

        // http://msdn.microsoft.com/en-us/library/aa480216.aspx
        [DllImport("Kernel32", SetLastError = true, EntryPoint = "CancelSynchronousIo")]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern Boolean CancelSynchronousIO(SafeWaitHandle hThread);
    }
    #endregion
}
