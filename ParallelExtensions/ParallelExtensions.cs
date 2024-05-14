using Microsoft.Extensions.Logging;

namespace ParallelExtensions
{
    /// <summary>
    /// Provides extension methods for running tasks in parallel.
    /// </summary>
    public static class ParallelExtensions
    {
        /// <summary>
        /// Executes a collection of asynchronous tasks in parallel with a specified maximum degree of parallelism.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the tasks.</typeparam>
        /// <param name="tasks">The collection of tasks to execute.</param>
        /// <param name="maxParallelTasks">The maximum number of tasks to execute concurrently.</param>
        /// <param name="exceptionHandlingStrategy">The strategy for handling exceptions during task execution.</param>
        /// <param name="logger">The optional logger instance for logging exceptions.</param>
        /// <returns>A readonly collection of task results.</returns>
        /// <remarks>
        /// This method executes the specified tasks concurrently with a maximum degree of parallelism.
        /// It ensures that no more than the specified number of tasks run simultaneously.
        /// </remarks>
        public static async Task<IReadOnlyCollection<T>> RunInParallel<T>(
        this IEnumerable<Func<Task<T>>> tasks,
        int maxParallelTasks = 4,
        ExceptionHandlingStrategy exceptionHandlingStrategy = ExceptionHandlingStrategy.PropagateImmediately,
        ILogger? logger = null)
        {
            var results = new List<T?>();
            var exceptions = new List<Exception>();

            var semaphore = new SemaphoreSlim(maxParallelTasks, maxParallelTasks);
            var cancellationTokenSource = new CancellationTokenSource();

            async Task ExecuteTask(Func<Task<T>> taskFunc)
            {
                await semaphore.WaitAsync(cancellationTokenSource.Token);
                try
                {
                    results.Add(await taskFunc().ConfigureAwait(false));
                }
                catch (Exception ex)
                {
                    switch (exceptionHandlingStrategy)
                    {
                        case ExceptionHandlingStrategy.PropagateImmediately:
                            cancellationTokenSource.Cancel();
                            semaphore.Dispose();
                            throw;
                        case ExceptionHandlingStrategy.AggregateExceptions:
                            exceptions.Add(ex);
                            break;
                        case ExceptionHandlingStrategy.IgnoreExceptions:
                            logger?.LogError(ex, "Error occurred during task execution.");
                            break;
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }

            var tasksToRun = tasks.Select(ExecuteTask);

            try
            {
                await Task.WhenAll(tasksToRun).ConfigureAwait(false);
            }
            catch
            {
                // Обработка исключений из задач уже выполнена в методе ExecuteTask.
                // Это исключение возникает, когда используется стратегия PropagateImmediately.
                throw;
            }

            switch (exceptionHandlingStrategy)
            {
                case ExceptionHandlingStrategy.AggregateExceptions:
                    if (exceptions.Any())
                    {
                        throw new AggregateException("Exceptions occurred during parallel execution.", exceptions);
                    }
                    break;
            }

            return results.AsReadOnly();
        }
    }
}
