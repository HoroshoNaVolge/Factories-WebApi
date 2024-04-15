using System.Text.Json;

namespace dotnetLearning.JsonParserApp
{
    internal static class JsonParser
    {
        public static IEnumerable<Deal> GetJsonFromFile(string path)
        {
            string fileContent = File.ReadAllText(path);
            IEnumerable<Deal> deserializedDeals = JsonSerializer.Deserialize<List<Deal>>(fileContent);
            return deserializedDeals;
        }
    }
}
