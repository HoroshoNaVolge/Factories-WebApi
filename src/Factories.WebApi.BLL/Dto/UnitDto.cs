namespace Factories.WebApi.BLL.Dto
{
    public class UnitDto
    {
        public required int Id { get; init; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? FactoryId { get; set; }
    }
}
