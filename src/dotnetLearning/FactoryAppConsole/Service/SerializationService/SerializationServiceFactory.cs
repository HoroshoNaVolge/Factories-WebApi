using Microsoft.Extensions.DependencyInjection;

namespace dotnetLearning.FactoryApp.Service.SerializationService
{
    public interface ISerializationServiceFactory
    {
        public ISerializationService CreateService(SerializationServiceType serviceType);
    }

    public class SerializationServiceFactory(IServiceProvider serviceProvider) : ISerializationServiceFactory
    {
        public ISerializationService CreateService(SerializationServiceType serviceType)
        {
            return serviceType switch
            {
                SerializationServiceType.Json => serviceProvider.GetService<JsonService>()!,
                SerializationServiceType.Excel => serviceProvider.GetService<ExcelService>()!,
                SerializationServiceType.Db => serviceProvider.GetService<DbFacilitiesService>()!,
                _ => throw new ArgumentException(MessageConstants.SerializationServiceTypeErrorMessage),
            };
        }
    }
    public enum SerializationServiceType
    {
        Json,
        Excel,
        Db
    }
}
