using dotnetLearning.FactoryApp.Model;
using System.Text.Json.Serialization;

namespace dotnetLearning.FactoryApp.Service.FacilityService
{
    public class FacilitiesContainer(IList<Factory> factories, IList<Unit> units, IList<Tank> tanks)
    {
        [JsonPropertyName("Factory")]
        public IList<Factory> Factories { get; set; } = factories;
        [JsonPropertyName("Unit")]
        public IList<Unit> Units { get; set; } = units;
        [JsonPropertyName("Tank")]
        public IList<Tank> Tanks { get; set; } = tanks;
    }
}