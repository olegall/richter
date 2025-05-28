/*
    https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile
    With the volatile modifier added to the declaration of _shouldStop in place, you'll always get the same results 
    (similar to the excerpt shown in the preceding code). 
    However, without that modifier on the _shouldStop member, the behavior is unpredictable. 
    The DoWork method may optimize the member access, resulting in reading stale data. 
    Because of the nature of multi-threaded programming, the number of stale reads is unpredictable. 
    Different runs of the program will produce somewhat different results.
*/

WorkerThreadExample.Main();

public class Worker
{
    // This method is called when the thread is started.
    public void DoWork()
    {
        bool work = false;

        while (!_shouldStop)
        {
            work = !work; // simulate some work
        }

        Console.WriteLine("Worker thread: terminating gracefully.");
    }

    public void RequestStop()
    {
        _shouldStop = true;
    }

    // Keyword volatile is used as a hint to the compiler that this data member is accessed by multiple threads.
    private volatile bool _shouldStop; // TODO volatile не влияет. как это должно рабить? Возможно пример не рабит (класс Worker) Запустить на другом ПК
}

public class WorkerThreadExample
{
    public static void Main()
    {
        // Create the worker thread object. This does not start the thread.
        Worker w = new Worker();
        Thread t = new Thread(w.DoWork);

        // Start the worker thread.
        t.Start();
        Console.WriteLine("Main thread: starting worker thread...");

        // Loop until the worker thread activates.
        var dt1 = DateTime.Now;
        while (!t.IsAlive); // 300-400 ms aleek
        var dt2 = DateTime.Now;

        // Put the main thread to sleep for 500 milliseconds to allow the worker thread to do some work.
        Thread.Sleep(500);

        // Request that the worker thread stop itself.
        w.RequestStop();

        // Use the Thread.Join method to block the current thread until the object's thread terminates.
        t.Join();
        Console.WriteLine("Main thread: worker thread has terminated.");
    }
    // Sample output:
    // Main thread: starting worker thread...
    // Worker thread: terminating gracefully.
    // Main thread: worker thread has terminated.
}
