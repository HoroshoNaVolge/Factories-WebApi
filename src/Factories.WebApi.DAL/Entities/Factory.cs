namespace Factories.WebApi.DAL.Entities
{
    public class Factory
    {
        public required int Id { get; init; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}