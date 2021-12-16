using System.Threading;
using System.Threading.Tasks;

namespace Richter
{
    public static class ExtensionMethods
    {
        // почему можно без return?
        public static async Task WithCancellationEmpty(this Task originalTask, CancellationToken ct)
        {
        }

        private struct Void { } // Из-за отсутствия необобщенного класса TaskCompletionSource.

        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> originalTask, CancellationToken ct)
        {
            // Создание объекта Task, завершаемого при отмене CancellationToken
            var cancelTask = new TaskCompletionSource<Void>();

            // При отмене CancellationToken завершить Task
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
            {
                // Создание объекта Task, завершаемого при отмене исходного объекта Task или объекта Task от CancellationToken
                Task any = await Task.WhenAny(originalTask, cancelTask.Task); // почему originalTask, cancelTask одного типа, а согласованы?
                // Если какой-либо объект Task завершается из-за CancellationToken, инициировать OperationCanceledException
                if (any == cancelTask.Task)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
            // Выполнить await для исходного задания (синхронно); awaiting it если произойдет ошибка, выдать первое внутреннее исключение вместо AggregateException
            return await originalTask;
        }

        // починить
        /*private static async Task WithCancellation(this Task originalTask, CancellationToken ct)
        {
            return await originalTask;
        }*/
    }
}
