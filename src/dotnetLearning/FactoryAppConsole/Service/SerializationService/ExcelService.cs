using dotnetLearning.FactoryApp.Model;
using dotnetLearning.FactoryApp.Service.FacilityService;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace dotnetLearning.FactoryApp.Service.SerializationService
{
    public class ExcelService : ISerializationService
    {
        private readonly string excelFilePath;

        public ExcelService(IOptions<FacilityServiceOptions> options)
        {
            excelFilePath = options.Value.FacilitiesExcelFilePath!;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task CreateOrUpdateAllAsync(FacilitiesContainer container, CancellationToken token)
        {
            if (container is null) return;

            var factoryValues = container.Factories.Select(Factory.GetValues).ToList();
            var unitValues = container.Units.Select(Unit.GetValues).ToList();
            var tankValues = container.Tanks.Select(Tank.GetValues).ToList();

            await Task.Run(() =>
            {
                using var package = new ExcelPackage();

                WriteWorksheet(factoryValues, Factory.GetFactoryKeys(), package.Workbook.Worksheets.Add("Factories"));
                WriteWorksheet(unitValues, Unit.GetUnitKeys(), package.Workbook.Worksheets.Add("Units"));
                WriteWorksheet(tankValues, Tank.GetTankKeys(), package.Workbook.Worksheets.Add("Tanks"));

                foreach (var sh in package.Workbook.Worksheets)
                    sh.Cells[sh.Dimension.Address].AutoFitColumns();

                try
                {
                    package.SaveAs(new FileInfo(excelFilePath));
                }
                // Если файл уже открыт в excel например, то сохраняем с другим именем.
                // Проверить без исключения не понимаю как, даже через поток FileStream нужно ловить будет.
                catch (InvalidOperationException)
                {
                    package.SaveAs(new FileInfo($"{excelFilePath} Reserved.xlsx"));
                }
            }, token);
        }

        public async Task GetFacilitiesAsync(FacilitiesContainer container, CancellationToken token)
        {
            if (!File.Exists(excelFilePath)) return;

            await Task.Run(() =>
            {
                container.Factories = ReadFromExcel<Factory>(excelFilePath, "Factories");
                container.Units = ReadFromExcel<Unit>(excelFilePath, "Units");
                container.Tanks = ReadFromExcel<Tank>(excelFilePath, "Tanks");
            }, token);
        }

        public async Task AddFacilityAsync(IFacility facility, CancellationToken token)
        {
            if (facility is null || !File.Exists(excelFilePath)) return;

            FacilitiesContainer containerToUpdate = await Task.Run(() =>
            {
                return new FacilitiesContainer(
                    ReadFromExcel<Factory>(excelFilePath, "Factories"),
                    ReadFromExcel<Unit>(excelFilePath, "Units"),
                    ReadFromExcel<Tank>(excelFilePath, "Tanks"));
            }) ?? throw new ArgumentNullException(MessageConstants.ExcelReaderErrorMessage);

            switch (facility)
            {
                case Factory factory:
                    containerToUpdate.Factories.Add(factory);
                    break;

                case Unit unit:
                    containerToUpdate.Units.Add(unit);
                    break;

                case Tank tank:
                    containerToUpdate.Tanks.Add(tank);
                    break;

                default:
                    throw new ArgumentException(MessageConstants.InvalidFacilityTypeErrorMessage);
            }
            await CreateOrUpdateAllAsync(containerToUpdate, token);
        }

        public async Task UpdateFacilityAsync(IFacility facility, CancellationToken token)
        {
            if (facility is null || !File.Exists(excelFilePath)) return;

            FacilitiesContainer containerToUpdate = await Task.Run(() =>
            {
                return new FacilitiesContainer(
                    ReadFromExcel<Factory>(excelFilePath, "Factories"),
                    ReadFromExcel<Unit>(excelFilePath, "Units"),
                    ReadFromExcel<Tank>(excelFilePath, "Tanks"));
            }) ?? throw new ArgumentNullException(MessageConstants.ExcelReaderErrorMessage);

            switch (facility)
            {
                case Factory factory:
                    var factoryIndex = containerToUpdate.Factories.ToList().FindIndex(f => f.Id == factory.Id);
                    if (factoryIndex != -1)
                    {
                        // Потому что свойства Id и Name объекта в модели неизменяемы
                        var updatedFactory = new Factory
                        {
                            Id = factory.Id,
                            Name = factory.Name,
                            Description = factory.Description
                        };
                        containerToUpdate.Factories[factoryIndex] = updatedFactory;
                    }
                    break;

                case Unit unit:
                    var unitIndex = containerToUpdate.Units.ToList().FindIndex(u => u.Id == unit.Id);
                    if (unitIndex != -1)
                    {
                        var updatedUnit = new Unit
                        {
                            Id = unit.Id,
                            Name = unit.Name,
                            Description = unit.Description,
                            FactoryId = unit.FactoryId
                        };
                        containerToUpdate.Units[unitIndex] = updatedUnit;
                    }
                    break;

                case Tank tank:
                    var tankIndex = containerToUpdate.Tanks.ToList().FindIndex(t => t.Id == tank.Id);
                    if (tankIndex != -1)
                    {
                        var updatedTank = new Tank
                        {
                            Id = tank.Id,
                            Name = tank.Name,
                            Description = tank.Description,
                            Volume = tank.Volume,
                            MaxVolume = tank.MaxVolume,
                            UnitId = tank.UnitId
                        };
                        containerToUpdate.Tanks[tankIndex] = updatedTank;
                    }
                    break;

                default:
                    throw new ArgumentException(MessageConstants.InvalidFacilityTypeErrorMessage);
            }
            await CreateOrUpdateAllAsync(containerToUpdate, token);
        }

        public async Task DeleteFacilityAsync(IFacility facility, CancellationToken token)
        {
            if (facility is null || !File.Exists(excelFilePath)) return;

            FacilitiesContainer containerToUpdate = await Task.Run(() =>
            {
                return new FacilitiesContainer(
                    ReadFromExcel<Factory>(excelFilePath, "Factories"),
                    ReadFromExcel<Unit>(excelFilePath, "Units"),
                    ReadFromExcel<Tank>(excelFilePath, "Tanks"));
            }) ?? throw new ArgumentNullException(MessageConstants.ExcelReaderErrorMessage);

            switch (facility)
            {
                case Factory factory:
                    var factoriesToRemove = containerToUpdate.Factories.Where(f => f.Name == facility.Name).ToList();
                    foreach (var factoryToRemove in factoriesToRemove)
                        containerToUpdate.Factories.Remove(factoryToRemove);
                    break;

                case Unit unit:
                    var unitsToRemove = containerToUpdate.Units.Where(u => u.Name == facility.Name).ToList();
                    foreach (var unitToRemove in unitsToRemove)
                        containerToUpdate.Units.Remove(unitToRemove);
                    break;

                case Tank tank:
                    var tanksToRemove = containerToUpdate.Tanks.Where(t => t.Name == facility.Name).ToList();
                    foreach (var tankToRemove in tanksToRemove)
                        containerToUpdate.Tanks.Remove(tankToRemove);
                    break;
                default:
                    throw new ArgumentException(MessageConstants.InvalidFacilityTypeErrorMessage);
            }
            await CreateOrUpdateAllAsync(containerToUpdate, token);
        }

        private static void WriteWorksheet(IList<IDictionary<string, object?>> values, IList<string> keys, ExcelWorksheet worksheet)
        {
            for (int i = 0; i < keys.Count; i++)
                worksheet.Cells[1, i + 1].Value = keys[i];

            int row = 2;
            foreach (var value in values)
            {
                for (int i = 0; i < value.Count; i++)
                    worksheet.Cells[row, i + 1].Value = value[keys[i]];
                row++;
            }
        }

        private static IList<T> ReadFromExcel<T>(string excelFilePath, string sheetName) where T : IEntity<T>
        {
            IList<T> items = [];

            using var package = new ExcelPackage(new FileInfo(excelFilePath));
            var worksheet = package.Workbook.Worksheets[sheetName];

            if (worksheet is null)
                return items;

            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                T item = Activator.CreateInstance<T>();

                var values = item.GetValues();
                var keys = item.GetKeys();

                for (int col = 1; col <= keys.Count; col++)
                {
                    var cellValue = worksheet.Cells[row, col].Value;

                    if (cellValue is null)
                        continue;

                    if (cellValue is double)
                        cellValue = Convert.ToInt32(cellValue);

                    values[keys[col - 1]] = cellValue;
                }
                items.Add(item.Create(values));
            }
            return items;
        }
    }
}