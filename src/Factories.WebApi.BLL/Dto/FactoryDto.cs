namespace Factories.WebApi.BLL.Dto
{
    public class FactoryDto
    {
        public required int Id { get; init; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
