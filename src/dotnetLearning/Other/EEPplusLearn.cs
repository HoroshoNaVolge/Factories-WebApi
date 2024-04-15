using OfficeOpenXml;

namespace dotnetLearning.Other
{
    internal class EEPplusLearn
    {  // Создаем коллекцию данных для записи в Excel
        List<MyData> data = new List<MyData>
        {
            new MyData { Id = 1, Name = "Item1", Value = 10.5 },
            new MyData { Id = 2, Name = "Item2", Value = 20.7 },
        };

        // Указываем путь к файлу Excel
        string excelFilePath = "C:\\Users\\user\\Desktop\\test.xlsx";

        // Записываем данные в файл Excel
        public void Run()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            WriteToExcel(data, excelFilePath);
        }

        static void WriteToExcel(List<MyData> data, string filePath)
        {
            using (var package = new ExcelPackage())
            {
                // Добавляем новый лист в Excel
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Заполняем заголовки
                worksheet.Cells["A1"].Value = "Id";
                worksheet.Cells["B1"].Value = "Name";
                worksheet.Cells["C1"].Value = "Value";

                // Заполняем данные
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.Value;
                    row++;
                }

                // Сохраняем Excel файл
                package.SaveAs(new FileInfo(filePath));
            }

            Console.WriteLine($"Данные сохранены в {filePath}");
        }
    }

    class MyData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
    }
}