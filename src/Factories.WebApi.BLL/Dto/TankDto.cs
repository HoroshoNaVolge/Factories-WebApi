namespace Factories.WebApi.BLL.Dto
{
    public class TankDto
    {
        public required int Id { get; init; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? UnitId { get; set; }
        public double? Volume { get; set; }
        public double? MaxVolume { get; set; }
    }
}
