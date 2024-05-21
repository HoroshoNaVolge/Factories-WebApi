using ParallelExtensions;
using System.Net.Http.Json;

namespace RunInParallelJsonPlaceholder
{
    internal class Program
    {
        static async Task Main()
        {
            try
            {
                var postIds = GeneratePostIds(103);
                var posts = await LoadPostsInParallel(postIds);

                Console.WriteLine($"Total posts loaded: {posts.Count(p => p.Title != "Error loading post")}");

                foreach (var post in posts)
                    Console.WriteLine(post.ToString());
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An http request error occurred: {ex.Message}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static IEnumerable<int> GeneratePostIds(int count)
        {
            for (int i = 1; i <= count; i++)
                yield return i;
        }

        public static async Task<IReadOnlyCollection<Post>> LoadPostsInParallel(IEnumerable<int> postIds)
        {
            using var httpClient = new HttpClient();

            async Task<Post> LoadPost(int postId)
            {
                try
                {
                    var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts/{postId}")
                        .ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadFromJsonAsync<Post>().ConfigureAwait(false) ?? new Post() { Id = postId, Title = "Error loading post" };
                }

                catch (HttpRequestException)
                {
                    return new Post { Id = postId, Title = "Error loading post" };
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error happened with postId={postId}: {ex.Message}");
                    return new Post { Id = postId, Title = "Error loading post" };
                }
            }

            var tasks = new List<Func<Task<Post>>>();

            foreach (var postId in postIds)
            {
                int capturedPostId = postId;
                tasks.Add(() => LoadPost(capturedPostId));
            }

            var results = await tasks.RunInParallel(maxParallelTasks: 5).ConfigureAwait(false);

            return [.. results.OrderBy(post => post.Id)];
        }
    }
}
