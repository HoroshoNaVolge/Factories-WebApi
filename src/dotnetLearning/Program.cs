using Microsoft.Extensions.Configuration;
using dotnetLearning.FactoryApp.Service;
using Microsoft.Extensions.DependencyInjection;
using dotnetLearning.FactoryApp.Service.FacilityService;
using dotnetLearning.FactoryApp.View;
using Microsoft.EntityFrameworkCore;
using dotnetLearning.FactoryApp.Service.SerializationService;
using dotnetLearning.FactoryApp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
           //Для Visual Studio. Несколько раз вверх, т.к. current directory выдаёт /bin/debug/dotnet8.0.
           .AddJsonFile(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "../../../appsettings.json")))
           .Build();

        var services = new ServiceCollection();
        services.Configure<FacilityServiceOptions>(config.GetSection(FacilityServiceOptions.SectionName));

        var optionsCheckIfValid = config.GetSection(FacilityServiceOptions.SectionName).Get<FacilityServiceOptions>() ?? throw new ArgumentNullException(nameof(FacilityServiceOptions), "Ошибка конфигурации: не найдена секция FilePath или отсутствует файл Json");

        if (string.IsNullOrEmpty(optionsCheckIfValid.FacilitiesJsonFilePath) || string.IsNullOrEmpty(optionsCheckIfValid.FacilitiesExcelFilePath))
            throw new ArgumentNullException(nameof(FacilityServiceOptions), MessageConstants.InvalidConfigurationFilePathErrorMessage);

        var connectionString = string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection"))
            ? throw new ArgumentNullException(MessageConstants.InvalidConnectionStringErrorMessage)
            : config.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));

        services.AddSingleton<FactoryAppService>()
            .AddScoped<IFacilityService, FacilityService>()
            .AddSingleton<IView, ConsoleView>()
            .AddSingleton<JsonService>()
            .AddSingleton<ExcelService>()
            .AddSingleton<DbFacilitiesService>()
            .AddSingleton<ISerializationServiceFactory, SerializationServiceFactory>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        var app = serviceProvider.GetService<FactoryAppService>() ?? throw new ArgumentNullException(nameof(FactoryAppService), MessageConstants.ServiceRegistrationErrorMessage);
        await app.RunAsync();
    }
}