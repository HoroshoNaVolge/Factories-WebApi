using dotnetLearning.FactoryApp.Model;
using dotnetLearning.FactoryApp.Service.FacilityService;

namespace dotnetLearning.FactoryApp.Service.SerializationService
{
    public interface ISerializationService
    {
        public Task CreateOrUpdateAllAsync(FacilitiesContainer container, CancellationToken token);

        public Task AddFacilityAsync(IFacility facility, CancellationToken token);

        public Task UpdateFacilityAsync(IFacility facility, CancellationToken token);

        public Task DeleteFacilityAsync(IFacility facility, CancellationToken token);

        public Task GetFacilitiesAsync(FacilitiesContainer container, CancellationToken token);
    }
}
