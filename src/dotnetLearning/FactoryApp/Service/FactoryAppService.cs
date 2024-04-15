using Microsoft.Extensions.Options;
using dotnetLearning.FactoryApp.Service.FacilityService;
using dotnetLearning.FactoryApp.View;
using dotnetLearning.FactoryApp.Model;
using dotnetLearning.FactoryApp.Service.SerializationService;

namespace dotnetLearning.FactoryApp.Service
{
    public class FactoryAppService(IFacilityService facilityService, IView view)
    {
        internal async Task RunAsync()
        {
            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            var userInteractionFinished = false;
#nullable disable
            view.InputReceived += HandleUserInput;
#nullable restore

            view.ShowMessage(
                    "\nread json - десериализовать все объекты из json\n" +
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
                    "exit - выход из программы\n");

            view.ShowMessage($"Система при инициализации: {facilityService.GetCurrentConfiguration()}");
            view.ShowMessage("Возможен CRUD JSON, Excel и PostgresDB (для ДБ необходима настройка appsettings.json (порт/пароль).\nДля проверки после выполнения команд используйте get total\n\nДля первичной инициализации используйте read json");

            while (!userInteractionFinished)
            {
                var userInput = view.GetUserInput("Введите команду: ");

                switch (userInput)
                {
                    #region get info
                    case "get conf":
                        view.ShowMessage(facilityService.GetCurrentConfiguration() ?? MessageConstants.NothingFoundMessage);
                        break;

                    case "get total":
                        view.ShowMessage(facilityService.GetTotalSummary() ?? MessageConstants.NothingFoundMessage);
                        break;

                    case "get tanksVolumeTotal":
                        view.ShowMessage(facilityService.GetTotalVolumeTanks() ?? MessageConstants.NothingFoundMessage);
                        break;

                    case "get tanksSummary":
                        view.ShowMessage(facilityService.GetTanksSummary() ?? MessageConstants.NothingFoundMessage);
                        break;

                    case "get factoriesSummary":
                        view.ShowMessage(facilityService.GetFactoriesSummary() ?? MessageConstants.NothingFoundMessage);
                        break;

                    case "get unitsSummary":
                        view.ShowMessage(facilityService.GetUnitsSummary() ?? MessageConstants.NothingFoundMessage);
                        break;

                    case "find unit":
                        var tankName = view.GetUserInput("Введите название резервуара:");
                        if (string.IsNullOrEmpty(tankName))
                        {
                            view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                            return;
                        }

                        var foundUnit = FindUnit(tankName);
                        if (string.IsNullOrEmpty(foundUnit))
                            view.ShowMessage($"Резервуар {tankName} не найден");
                        else
                            view.ShowMessage($"Найдена установка: {foundUnit}");
                        break;

                    case "find factory":
                        var unitName = view.GetUserInput("Введите название установки:");

                        if (string.IsNullOrEmpty(unitName))
                        {
                            view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                            return;
                        }

                        var foundFactory = FindFactory(unitName);
                        if (string.IsNullOrEmpty(foundFactory))
                            view.ShowMessage($"Установка {unitName} не найдена");
                        else
                            view.ShowMessage($"Найден завод: {foundFactory}");

                        break;

                    case "search":
                        var searchName = view.GetUserInput("Введите название для поиска:");
                        if (string.IsNullOrEmpty(searchName))
                        {
                            view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                            return;
                        }

                        var result = facilityService.Search(searchName);
                        if (result is null)
                            view.ShowMessage("Объект не найден");
                        else
                            view.ShowMessage($"Результат поиска: {result}");
                        break;
                    #endregion

                    #region Json CRUD
                    case "write json":
                        await facilityService.CreateOrUpdateDataAsync(SerializationServiceType.Json, cancellationToken);
                        break;

                    case "add json":
                        var facilityTypeString = "dotnetLearning.FactoryApp.Model." + view.GetUserInput("Введите название типа объекта");
                        var facilityType = Type.GetType(facilityTypeString);

                        if (facilityType is null)
                        {
                            view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                            break;
                        }

                        switch (facilityType.Name)
                        {
                            case "Factory":
                                var factory = CreateFactoryByUserInput(view);
                                if (factory is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Json, factory, cancellationToken);
                                break;
                            case "Unit":
                                var unit = CreateUnitByUserInput(view);
                                if (unit is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Json, unit, cancellationToken);
                                break;
                            case "Tank":
                                var tank = CreateTankByUserInput(view);
                                if (tank is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Json, tank, cancellationToken);
                                break;
                            default:
                                view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                break;
                        }
                        break;

                    case "delete json":
                        var inputString = view.GetUserInput("Введите название объекта для удаления");
                        if (string.IsNullOrEmpty(inputString))
                        {
                            view.ShowMessage("Пустой ввод");
                            break;
                        }
                        await facilityService.GetDataAsync(SerializationServiceType.Json, cancellationToken);
                        var fac = facilityService.Search(inputString);

                        if (fac is null)
                        {
                            view.ShowMessage("Объект не найден");
                            break;
                        }

                        await facilityService.DeleteDataAsync(SerializationServiceType.Json, fac, cancellationToken);
                        break;

                    case "read json":
                        await facilityService.GetDataAsync(SerializationServiceType.Json, cancellationToken);
                        break;

                    case "update json":
                        {
                            var typeFacStr = "dotnetLearning.FactoryApp.Model." + view.GetUserInput("Введите название типа объекта");
                            var facType = Type.GetType(typeFacStr);

                            if (facType is null)
                            {
                                view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                break;
                            }

                            switch (facType.Name)
                            {
                                case "Factory":
                                    var factory = CreateFactoryByUserInput(view);
                                    if (factory is null)
                                    {
                                        view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                        break;
                                    }
                                    await facilityService.UpdateDataAsync(SerializationServiceType.Json, factory, cancellationToken);
                                    break;
                                case "Unit":
                                    var unit = CreateUnitByUserInput(view);
                                    if (unit is null)
                                    {
                                        view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                        break;
                                    }
                                    await facilityService.UpdateDataAsync(SerializationServiceType.Json, unit, cancellationToken);
                                    break;
                                case "Tank":
                                    var tank = CreateTankByUserInput(view);
                                    if (tank is null)
                                    {
                                        view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                        break;
                                    }
                                    await facilityService.UpdateDataAsync(SerializationServiceType.Json, tank, cancellationToken);
                                    break;
                                default:
                                    view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                    break;
                            }
                            break;
                        }
                    #endregion

                    #region Excel CRUD
                    case "write excel":
                        await facilityService.CreateOrUpdateDataAsync(SerializationServiceType.Excel, cancellationToken);
                        break;

                    case "read excel":
                        await facilityService.GetDataAsync(SerializationServiceType.Excel, cancellationToken);
                        break;

                    case "add excel":
                        var typeFacExcelStr = "dotnetLearning.FactoryApp.Model." + view.GetUserInput("Введите название типа объекта");
                        var excelFacType = Type.GetType(typeFacExcelStr);

                        if (excelFacType is null)
                        {
                            view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                            break;
                        }

                        switch (excelFacType.Name)
                        {
                            case "Factory":
                                var factory = CreateFactoryByUserInput(view);
                                if (factory is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Excel, factory, cancellationToken);
                                break;
                            case "Unit":
                                var unit = CreateUnitByUserInput(view);
                                if (unit is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Excel, unit, cancellationToken);
                                break;
                            case "Tank":
                                var tank = CreateTankByUserInput(view);
                                if (tank is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Excel, tank, cancellationToken);
                                break;
                            default:
                                view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                break;
                        }
                        break;

                    case "update excel":
                        var typeFacExcelStrUpdate = "dotnetLearning.FactoryApp.Model." + view.GetUserInput("Введите название типа объекта");
                        var excelFacTypeUpdate = Type.GetType(typeFacExcelStrUpdate);

                        if (excelFacTypeUpdate is null)
                        {
                            view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                            break;
                        }

                        switch (excelFacTypeUpdate.Name)
                        {
                            case "Factory":
                                var factory = CreateFactoryByUserInput(view);
                                if (factory is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.UpdateDataAsync(SerializationServiceType.Excel, factory, cancellationToken);
                                break;
                            case "Unit":
                                var unit = CreateUnitByUserInput(view);
                                if (unit is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.UpdateDataAsync(SerializationServiceType.Excel, unit, cancellationToken);
                                break;
                            case "Tank":
                                var tank = CreateTankByUserInput(view);
                                if (tank is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.UpdateDataAsync(SerializationServiceType.Excel, tank, cancellationToken);
                                break;
                            default:
                                view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                break;
                        }
                        break;

                    case "delete excel":
                        var inString = view.GetUserInput("Введите название объекта для удаления");
                        if (string.IsNullOrEmpty(inString))
                        {
                            view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                            break;
                        }
                        await facilityService.GetDataAsync(SerializationServiceType.Excel, cancellationToken);
                        var facExcel = facilityService.Search(inString);
                        if (facExcel is null)
                        {
                            view.ShowMessage(MessageConstants.NothingFoundMessage);
                            break;
                        }

                        await facilityService.DeleteDataAsync(SerializationServiceType.Excel, facExcel, cancellationToken);
                        break;
                    #endregion

                    #region Db CRUD
                    case "write db":
                        await facilityService.CreateOrUpdateDataAsync(SerializationServiceType.Db, cancellationToken);
                        break;

                    case "read db":
                        await facilityService.GetDataAsync(SerializationServiceType.Db, cancellationToken);
                        break;

                    case "add db":
                        var typeFacDbStr = "dotnetLearning.FactoryApp.Model." + view.GetUserInput("Введите название типа объекта");
                        var dbFacType = Type.GetType(typeFacDbStr);

                        if (dbFacType is null)
                        {
                            view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                            break;
                        }

                        switch (dbFacType.Name)
                        {
                            case "Factory":
                                var factory = CreateFactoryByUserInput(view);
                                if (factory is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Db, factory, cancellationToken);
                                break;
                            case "Unit":
                                var unit = CreateUnitByUserInput(view);
                                if (unit is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Db, unit, cancellationToken);
                                break;
                            case "Tank":
                                var tank = CreateTankByUserInput(view);
                                if (tank is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.AddDataAsync(SerializationServiceType.Db, tank, cancellationToken);
                                break;
                            default:
                                view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                break;
                        }
                        break;
                    case "delete db":
                        var inpString = view.GetUserInput("Введите название объекта для удаления");
                        if (string.IsNullOrEmpty(inpString))
                        {
                            view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                            break;
                        }

                        var facDb = facilityService.Search(inpString);
                        if (facDb is null)
                        {
                            view.ShowMessage(MessageConstants.NothingFoundMessage);
                            break;
                        }

                        await facilityService.DeleteDataAsync(SerializationServiceType.Db, facDb, cancellationToken);
                        break;

                    case "update db":
                        var typeFacDbStrUpdate = "dotnetLearning.FactoryApp.Model." + view.GetUserInput("Введите название типа объекта");
                        var dbFacTypeUpdate = Type.GetType(typeFacDbStrUpdate);

                        if (dbFacTypeUpdate is null)
                        {
                            view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                            break;
                        }

                        switch (dbFacTypeUpdate.Name)
                        {
                            case "Factory":
                                var factory = CreateFactoryByUserInput(view);
                                if (factory is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.UpdateDataAsync(SerializationServiceType.Db, factory, cancellationToken);
                                break;
                            case "Unit":
                                var unit = CreateUnitByUserInput(view);
                                if (unit is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.UpdateDataAsync(SerializationServiceType.Db, unit, cancellationToken);
                                break;
                            case "Tank":
                                var tank = CreateTankByUserInput(view);
                                if (tank is null)
                                {
                                    view.ShowMessage(MessageConstants.InvalidInputErrorMessage);
                                    break;
                                }
                                await facilityService.UpdateDataAsync(SerializationServiceType.Db, tank, cancellationToken);
                                break;
                            default:
                                view.ShowMessage(MessageConstants.InvalidFacilityTypeErrorMessage);
                                break;
                        }
                        break;
                    #endregion

                    case "clear":
                        view.ClearView();
                        break;
                    case "exit":
                        userInteractionFinished = true;
                        break;

                    default:
                        view.ShowMessage(MessageConstants.UnknownCommandErrorMessage);
                        break;
                }
            }
            view.InputReceived -= HandleUserInput!;
        }
        private string? FindUnit(string tankName) => facilityService.FindUnit(tankName)?.ToString();
        private string? FindFactory(string unitName) => facilityService.FindFactory(unitName)?.ToString();

        private void HandleUserInput(object sender, UserInputEventArgs e) => view.ShowMessage($"Пользователь ввел: {e.UserInput} в {e.InputTime}");

        #region Create Facilities by User input
        private static Tank? CreateTankByUserInput(IView view)
        {
            view.ShowMessage("Введите данные резервуара:");

            if (!int.TryParse(view.GetUserInput("Id:"), out var id))
            {
                view.ShowMessage("Ошибка ввода. Id должен быть числом");
                return null;
            }

            var name = view.GetUserInput("Name: ") ?? "Not set";

            var description = view.GetUserInput("Description: ") ?? "Not set";

            if (!int.TryParse(view.GetUserInput("Volume: "), out var volume))
            {
                view.ShowMessage("Ошибка ввода. Volume должен быть числом");
                return null;
            }

            if (!int.TryParse(view.GetUserInput("MaxVolume: "), out var maxVolume))
            {
                view.ShowMessage($"Ошибка ввода. MaxVolume должен быть числом");
                return null;
            }

            if (!int.TryParse(view.GetUserInput("UnitId: "), out var unitId))
            {
                view.ShowMessage($"Ошибка ввода. UnitId должен быть числом");
                return null;
            }

            return new Tank
            {
                Id = id,
                Name = name,
                Description = description,
                Volume = volume,
                MaxVolume = maxVolume,
                UnitId = unitId
            };
        }

        private static Factory? CreateFactoryByUserInput(IView view)
        {
            view.ShowMessage("Введите данные завода:");

            if (!int.TryParse(view.GetUserInput("Id:"), out var id))
            {
                view.ShowMessage("Ошибка ввода. Id должен быть числом");
                return null;
            }

            var name = view.GetUserInput("Name: ") ?? "Not set";

            var description = view.GetUserInput("Description: ") ?? "Not set";

            return new Factory
            {
                Id = id,
                Name = name,
                Description = description
            };
        }

        private static Unit? CreateUnitByUserInput(IView view)
        {

            if (!int.TryParse(view.GetUserInput("Id:"), out var id))
            {
                view.ShowMessage("Ошибка ввода. Id должен быть числом");
                return null;
            }

            var name = view.GetUserInput("Name: ") ?? "Not set";

            var description = view.GetUserInput("Description: ") ?? "Not set";

            if (!int.TryParse(view.GetUserInput("FactoryId: "), out var factoryId))
            {
                view.ShowMessage("Ошибка ввода. FactoryId должен быть числом");
                return null;
            }

            return new Unit
            {
                Id = id,
                Name = name,
                Description = description,
                FactoryId = factoryId
            };
        }
        #endregion
    }
}