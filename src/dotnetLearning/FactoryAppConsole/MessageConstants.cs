namespace dotnetLearning.FactoryApp
{
    public static class MessageConstants
    {
        public const string FacilityServiceTypeErrorMessage = "Ошибка в типе сервиса сериализации";

        public const string InvalidFacilityTypeErrorMessage = "Ошибка в типе объекта Facility";

        public const string InvalidInputErrorMessage = "Ошибка ввода";

        public const string NothingFoundMessage = "Ничего не найдено";

        public const string UnknownCommandErrorMessage = "Неизвестная команда";

        public const string DeserializationErrorMessage = "Ошибка десериализации объектов в FacilitiesContainer";

        public const string InvalidConfigurationFilePathErrorMessage = "Ошибка конфигурации: отсутствует путь файлов Json или Excel";

        public const string InvalidConnectionStringErrorMessage = "Ошибка конфигурации: отсутствует или пуста строка подключения к БД";

        public const string ServiceRegistrationErrorMessage = "Ошибка регистрации сервисов в контейнере зависимостей";

        public const string SerializationServiceTypeErrorMessage = "Ошибка в типе сервиса для сериализации";

        public const string ExcelReaderErrorMessage = "Ошибка чтения Excel-файла";

        public const string AskUserInputCommand = "Введите команду";

        public const string AskUserInputFactoryName = "Введите название завода";

        public const string AskUserInputUnitName = "Введите название установки";

        public const string AskUserInputTankName = "Введите название резервуара";

        public const string AskUserInputFacilityName = "Введите название объекта для поиска";

        public const string WelcomeMessage = "\nread json - десериализовать все объекты из json\n" +
                    "write json - сериализовать все объекты в json\n" +
                    "add json - добавить объект в json\n" +
                    "update json - обновить объект в json\n" +
                    "delete json - удалить объект из json\n\n" +
                    "read excel - импортировать все объекты из Excel\n" +
                    "write excel - экспортировать все объекты в Excel\n" +
                    "add excel - добавить объект в Excel\n" +
                    "delete excel - удалить объект из Excel\n\n" +
                    "get conf - показать текущую конфигурацию системы\n" +
                    "get total - показать полную сводку\n" +
                    "get tanksVolumeTotal - показать общую вместимость резервуаров\n" +
                    "get tanksSummary - показать сводку по резервуарам\n" +
                    "get factoriesSummary - показать сводку по установкам\n" +
                    "get unitsSummary - показать сводку по заводам\n" +
                    "find unit - найти резервуар по названию\n" +
                    "find factory - найти установку по названию\n" +
                    "search - поиск по названию\n\n" +
                    "write db - записать все объекты в БД\n" +
                    "read db - прочитать все объекты из БД\n" +
                    "add db - добавить объект в БД\n" +
                    "delete db - удалить объект из БД\n" +
                    "update db - обновить объект в БД\n\n" +
                    "clear - очистить консоль\n" +
                    "exit - выход из программы\n";
    }
}
