using System.Reflection;

namespace dotnetLearning.JsonParserApp
{
    internal class StartJsonParserApp
    {
        internal static void Run()
        {
            IEnumerable<Deal> objectsFromJson = JsonParser.GetJsonFromFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\JSON_sample_1.json");

            var result = GetNumbersOfDeals(objectsFromJson);
            Console.WriteLine("Номера 5 самых ранних сделок с суммой >100 руб по убыванию суммы: " + string.Join(", ", result));

            IEnumerable<SumByMonth> periods = GetSumsByMonth(objectsFromJson);

            foreach (var period in periods)
            {
                string yearName = period.Period.ToString("yyyy");
                string monthName = period.Period.ToString("MMMM");
                Console.WriteLine($"Год: {yearName} Месяц: {monthName} Выручка: {period.Sum}\n");
            }
        }

        static List<string> GetNumbersOfDeals(IEnumerable<Deal> deals)
        {
            // Фильтрация сделок по сумме не меньше 100 рублей
            var filteredDeals = deals.Where(deal => deal.Sum >= 100);

            // Выбор 5 сделок с самой ранней датой
            var selectedDeals = filteredDeals.OrderBy(deal => deal.Date).Take(5);

            // Возврат номеров сделок в отсортированном порядке по убыванию суммы
            var result = selectedDeals.OrderByDescending(deal => deal.Sum).Select(deal => deal.Id).ToList();

            return result;

        }
        record SumByMonth(DateTime Period, int Sum);

        // Реализовать метод GetSumsByMonth, который принимает коллекцию объектов класса Deal, группирует по месяцу сделки и возвращает сумму сделок за каждый месяц
        static List<SumByMonth> GetSumsByMonth(IEnumerable<Deal> deals)
        {
            var sumByMonth = deals
                            .GroupBy(deal => new { deal.Date.Year, deal.Date.Month })
                            //Ключ делим на два, так как год может отличаться.
                            .Select(g => new SumByMonth(new DateTime(g.Key.Year, g.Key.Month, 1), g.Sum(deal => deal.Sum)));
            return sumByMonth.ToList();
        }
    }
}