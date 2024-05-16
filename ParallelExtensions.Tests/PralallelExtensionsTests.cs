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
            {
                Assert.Contains(expectedResult, results);
            }
        }

        [Fact]
        public async Task RunInParallel_CancelsAllOperations()
        {
            // Arrange
            var tasks = new List<Func<Task<int>>>
            {
                async () => { await Task.Delay(1000); return 1; },
                async () => { await Task.Delay(1000); return 2; },
                async () => { await Task.Delay(1000); return 3; },
                async () => { await Task.Delay(1000); return 4; },
                async () => { await Task.Delay(1000); return 5; }
            };

            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100); // Cancel after 100 milliseconds

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
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
    }
}
