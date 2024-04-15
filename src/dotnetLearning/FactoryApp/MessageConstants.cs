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
    }
}
