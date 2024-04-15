using Dadata;

namespace dotnetLearning.DadataAppConsole
{
    // Задание: реализовать консольное приложение, которое единственным параметром будет принимать ИНН организации,
    // и печатать название организации, которое оно получит от сервиса DaData.
    // То есть нужно разобраться в их API, зарегистрироваться (сервис бесплатный для небольших масштабов),
    // составить запрос и реализовать это на C# Рекомендуется установить и использовать Fiddler/Postman или другой аналогичный инструмент.
    internal static class StartDadataAppConsole
    {

        /// <summary>
        /// Вызов через пакет Dadata
        /// </summary>
        /// <param name="companyInn">ИНН компании</param>
        /// <returns></returns>
        internal static async Task RunAsyncUsingDadataPackage(string companyInn)
        {
            {
                var token = "bbc8e57adbc7bebaf5265017740c30fce634cdc0";
                var api = new SuggestClientAsync(token);
                var response = await api.FindParty(companyInn);

                Console.WriteLine(response?.suggestions?.FirstOrDefault()?.value);
            }
        }
    }
}
