using Microsoft.Extensions.Logging.Abstractions;

namespace ParallelExtensions.Tests
{
    public class ParallelExtensionsTests
    {
        [Fact]
        public async Task RunInParallel_ExecutesFiveTasksSuccessfully()
        {
            // Arrange
            var tasks = new List<Func<Task<int>>>
            {
                async () => { await Task.Delay(100); return 1; },
                async () => { await Task.Delay(100); return 2; },
                async () => { await Task.Delay(100); return 3; },
                async () => { await Task.Delay(100); return 4; },
                async () => { await Task.Delay(100); return 5; }
            };

            var expectedResults = new List<int> { 1, 2, 3, 4, 5 };

            // Act
            var results = await tasks.RunInParallel(
                maxParallelTasks: 5,
                exceptionHandlingStrategy: ExceptionHandlingStrategy.PropagateImmediately,
                logger: NullLogger.Instance
            );

            // Assert
            Assert.Equal(expectedResults.Count, results.Count);
            foreach (var expectedResult in expectedResults)
                Assert.Contains(expectedResult, results);
        }

        [Fact]
        public async Task RunInParallel_CancelsAllOperations()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100);

            // Arrange
            var tasks = new List<Func<Task<int>>>
            {
                async () => { await Task.Delay(1000,cancellationTokenSource.Token); return 1; },
                async () => { await Task.Delay(1000,cancellationTokenSource.Token); return 2; },
                async () => { await Task.Delay(1000,cancellationTokenSource.Token); return 3; },
                async () => { await Task.Delay(1000,cancellationTokenSource.Token); return 4; },
                async () => { await Task.Delay(1000,cancellationTokenSource.Token); return 5; }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                await tasks.RunInParallel(
                    maxParallelTasks: 5,
                    exceptionHandlingStrategy: ExceptionHandlingStrategy.PropagateImmediately,
                    logger: NullLogger.Instance,
                    cancellationToken: cancellationTokenSource.Token
                );
            });

            // Verify that the exception is due to cancellation
            Assert.Equal(cancellationTokenSource.Token, exception.CancellationToken);
        }

        [Fact]
        public async Task RunInParallel_Cancellation_CancelsAllTasks()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var tasks = new List<Func<Task<int>>>
            {
                async () => { await Task.Delay(1000, cancellationToken); return 1; },
                async () => { await Task.Delay(1000, cancellationToken); return 2; },
                async () => { await Task.Delay(1000, cancellationToken); return 3; }
            };

            var executionTask = ParallelExtensions.RunInParallel(tasks, cancellationToken: cancellationToken);

            cancellationTokenSource.CancelAfter(500); 

            try
            {
                await executionTask;
            }
            catch (TaskCanceledException)
            {
                // Check that all tasks were indeed canceled
                foreach (var task in tasks)
                    Assert.True(task().IsCanceled);
                
                // All tasks were canceled as expected
                return;
            }
        }

        [Fact]
        public async Task RunInParallel_AggregateExceptionsStrategy()
        {
            // Arrange
            var tasks = new List<Func<Task<int>>>
            {
                async () => { await Task.Delay(100); throw new InvalidOperationException("Task 1 failed."); },
                async () => { await Task.Delay(200); throw new ArgumentException("Task 2 failed."); },
                async () => { await Task.Delay(300); throw new NotSupportedException("Task 3 failed."); },
                async () => { await Task.Delay(400); throw new NotImplementedException("Task 4 failed."); },
                async () => { await Task.Delay(500); throw new TimeoutException("Task 5 failed."); }
            };

            // Act & Assert
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await tasks.RunInParallel(
                    maxParallelTasks: 5,
                    exceptionHandlingStrategy: ExceptionHandlingStrategy.AggregateExceptions,
                    logger: NullLogger.Instance
                );
            });

            // Verify that all exceptions are included in the aggregate exception
            Assert.Equal(5, aggregateException.InnerExceptions.Count);
            Assert.Contains(aggregateException.InnerExceptions, ex => ex is InvalidOperationException);
            Assert.Contains(aggregateException.InnerExceptions, ex => ex is ArgumentException);
            Assert.Contains(aggregateException.InnerExceptions, ex => ex is NotSupportedException);
            Assert.Contains(aggregateException.InnerExceptions, ex => ex is NotImplementedException);
            Assert.Contains(aggregateException.InnerExceptions, ex => ex is TimeoutException);
        }

        [Fact]
        public async Task RunInParallel_IgnoreExceptionsStrategy()
        {
            // Arrange
            var tasks = new List<Func<Task<int>>>
            {
                async () => { await Task.Delay(100); throw new InvalidOperationException("Task 1 failed."); },
                async () => { await Task.Delay(200); return 2; }, // This task will complete successfully
                async () => { await Task.Delay(300); throw new ArgumentException("Task 3 failed."); },
                async () => { await Task.Delay(400); return 4; }, // This task will complete successfully
                async () => { await Task.Delay(500); throw new TimeoutException("Task 5 failed."); }
            };

            // Act
            var results = await tasks.RunInParallel(
                maxParallelTasks: 5,
                exceptionHandlingStrategy: ExceptionHandlingStrategy.IgnoreExceptions,
                logger: NullLogger.Instance
            );

            // Assert
            // Ensure that the method completes successfully and returns the results of successful tasks
            Assert.Equal(2, results.Count);
            Assert.Contains(2, results);
            Assert.Contains(4, results);
        }
    }
}
