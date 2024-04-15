using System.Text.Json;

namespace dotnetLearning.Other
{
    internal static class JsonLearn
    {
        public static void Run()
        {
            var aList = new List<Person>
            {
            new("Tom", 47, DateTime.Parse("2019-02-27T07:52:54.168Z")),
            new("Bob", 33, DateTime.Parse("2020-03-27T07:52:55.168Z"))
            };

            string json = JsonSerializer.Serialize(aList);
            Console.WriteLine(json);

            string filePath = @"C:\Users\user\source\repos\dotnetLearning\Other\Person.json";

            // Запись в файл
            File.WriteAllText(filePath, json);

            Console.WriteLine("Data saved to file Person.json");

            // Чтение из файла
            string fileContent = File.ReadAllText(filePath);
            List<Person>? desPersons = JsonSerializer.Deserialize<List<Person>>(fileContent);

            try
            {
                foreach (var person in desPersons)
                {
                    Console.WriteLine($"{person.Name}, {person.Age}");
                }
                Console.ReadLine();
            }
            catch (NullReferenceException)
            {
                return;
            }
        }
    }
}
