using System.Collections.Immutable;
using System.Text.Json;

namespace RunInParallelJsonPlaceholder
{
    internal class Program
    {
        static volatile int postId = 1;

        static async Task Main()
        {
            var posts = await LoadPostsInParallel(5);
            Console.WriteLine($"Total posts loaded: {posts.Count}");

            foreach (var post in posts)
                Console.WriteLine(post.ToString());
        }

        public static async Task<IReadOnlyCollection<Post>> LoadPostsInParallel(int numThreads)
        {
            var tasks = new List<Func<Task<Post>>>();

            while (true)
            {
                var task = FetchPostAsync(postId++);
                tasks.Add(() => task);
                var result = await task;

                if (result == null)
                {
                    Console.WriteLine("No more posts to load.");
                    break;
                }
            }

            var results = await tasks.RunInParallel(numThreads);

            return results.Where(post => post != null).ToImmutableList();
        }


        public static async Task<Post> FetchPostAsync(int postId)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts/{postId}");

                if (response.IsSuccessStatusCode)
                {
                    var postJson = await response.Content.ReadAsStringAsync();
                    var post = JsonSerializer.Deserialize<Post>(postJson);
                    return post!;
                }
                else
                    return null!;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error occurred while loading post {postId}: {ex.Message}");
                return null!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading post {postId}: {ex.Message}");
                return null!;
            }
        }
    }

    public static class ParallelExtensions
    {
        public static async Task<IReadOnlyCollection<T>> RunInParallel<T>(
            this IEnumerable<Func<Task<T>>> tasks, int maxParallelTasks = 4)
        {
            var taskLists = tasks.ToList();
            var results = new List<T?>();

            var semaphore = new SemaphoreSlim(maxParallelTasks, maxParallelTasks);

            var tasksToRun = taskLists.Select(async taskFunc =>
            {
                await semaphore.WaitAsync();
                try
                {
                    return await taskFunc();
                }

                catch (HttpRequestException ex)
                {
                    await Console.Out.WriteLineAsync($"Network error: {ex.Message}");
                    return default;
                }

                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            foreach (var task in tasksToRun)
            {
                if (task is not null)
                    results.Add(await task);
            }
            return results.AsReadOnly()!;
        }
    }
}
