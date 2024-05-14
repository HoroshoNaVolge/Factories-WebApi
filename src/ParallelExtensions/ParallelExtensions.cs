using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace ParallelExtensions
{
    /// <summary>
    /// Provides extension methods for executing multiple asynchronous tasks in parallel.
    /// </summary>
    public static class ParallelExtensions
    {
        /// <summary>
        /// Executes multiple asynchronous tasks in parallel and returns their results.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by each task.</typeparam>
        /// <param name="tasks">The collection of asynchronous tasks to execute.</param>
        /// <param name="maxParallelTasks">The maximum number of tasks to execute simultaneously. Defaults to 4 or the number of available processor cores, whichever is lower.</param>
        /// <param name="exceptionHandlingStrategy">The strategy for handling exceptions that occur during task execution.</param>
        /// <param name="logger">The logger to use for logging errors occurred during task execution. Defaults to <see langword="null"/>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>An immutable collection containing the results of the completed tasks.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tasks"/> is <see langword="null"/>.</exception>
        /// <exception cref="AggregateException">Thrown if <paramref name="exceptionHandlingStrategy"/> is <see cref="ExceptionHandlingStrategy.AggregateExceptions"/> and one or more exceptions occur during task execution.</exception>
        public static async Task<IReadOnlyCollection<T>> RunInParallel<T>(
            this IEnumerable<Func<Task<T>>> tasks,
            int maxParallelTasks = 4,
            ExceptionHandlingStrategy exceptionHandlingStrategy = ExceptionHandlingStrategy.PropagateImmediately,
            ILogger? logger = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(tasks);

            // Limit the number of parallel tasks to the specified maximum or the number of available processor cores
            maxParallelTasks = Math.Min(maxParallelTasks, Environment.ProcessorCount);

            var results = new ConcurrentBag<T>(); // ConcurrentBag to store results safely in a multi-threaded environment
            var exceptions = new ConcurrentBag<Exception>(); // List to hold exceptions occurred during task execution

            var semaphore = new SemaphoreSlim(maxParallelTasks, maxParallelTasks); // SemaphoreSlim to control the number of parallel tasks

            async Task ExecuteTask(Func<Task<T>> taskFunc)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await semaphore.WaitAsync(cancellationToken); // Wait for semaphore to acquire a slot for parallel execution

                    cancellationToken.ThrowIfCancellationRequested();
                    results.Add(await taskFunc()); // Execute the task and add its result to the ConcurrentBag
                }
                catch (OperationCanceledException) when (exceptionHandlingStrategy == ExceptionHandlingStrategy.PropagateImmediately)
                {
                    // Propagate cancellation immediately if configured to do so
                    semaphore.Release(); // Release semaphore slot if task execution is canceled
                    throw;
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error occurred during task execution"); // Log error if logger is provided

                    if (exceptionHandlingStrategy == ExceptionHandlingStrategy.AggregateExceptions)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex); // Add exception to the list if aggregation is enabled
                        }
                    }
                }
                finally
                {
                    semaphore.Release(); // Release semaphore slot after task execution
                }
            }

            var tasksToRun = tasks.Select(ExecuteTask).ToList(); // Convert task functions to tasks and start execution

            try
            {
                await Task.WhenAll(tasksToRun); // Wait for all tasks to complete
            }
            catch (OperationCanceledException) when (exceptionHandlingStrategy == ExceptionHandlingStrategy.PropagateImmediately)
            {
                // Swallow the cancellation exception if configured to propagate immediately
            }

            if (exceptionHandlingStrategy == ExceptionHandlingStrategy.AggregateExceptions && !exceptions.IsEmpty)
            {
                throw new AggregateException("Exceptions occurred during parallel execution.", exceptions); // Throw aggregate exception if enabled and exceptions occurred
            }

            return results?.ToImmutableList() ?? throw new NullReferenceException(nameof(results)); // Return immutable collection of results
        }
    }

    /// <summary>
    /// Specifies the strategy for handling exceptions that occur during parallel execution.
    /// </summary>
    public enum ExceptionHandlingStrategy
    {
        /// <summary>
        /// Propagate exceptions immediately when they occur during task execution.
        /// </summary>
        PropagateImmediately,

        /// <summary>
        /// Aggregate all exceptions that occur during task execution and throw them together at the end.
        /// </summary>
        AggregateExceptions,

        /// <summary>
        /// Ignore any exceptions that occur during task execution.
        /// </summary>
        IgnoreExceptions
    }
}
