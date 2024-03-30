using System.Security.Cryptography.X509Certificates;

namespace Factories.WebApi.DAL.Entities
{
    public class Unit
    {
        public required int Id { get; init; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? FactoryId { get; set; }

        public Factory? Factory { get; set; }
    }
}