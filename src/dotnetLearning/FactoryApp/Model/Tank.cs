namespace dotnetLearning.FactoryApp.Model
{
    public class Tank : IFacility, IEntity<Tank>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; set; }
        public int? UnitId { get; set; }
        public int? Volume { get; set; }
        public int? MaxVolume { get; set; }

        public IList<string> GetKeys() => GetTankKeys();
        public IDictionary<string, object?> GetValues() => GetValues(this);

        public override string ToString() =>
            $"Название резервуара: {Name}. Описание: {Description}. Id: {Id}. Текущий объём: {Volume}. Максимальный объём {MaxVolume}";

        public Tank Create(IDictionary<string, object?> values)
        {
            return new Tank
            {
                Name = values["Name"] as string ?? string.Empty,
                Description = values["Description"] as string,
                Id = values["Id"] as int? ?? 0,
                UnitId = values["UnitId"] as int?,
                Volume = values["Volume"] as int?,
                MaxVolume = values["MaxVolume"] as int?
            };
        }

        public static IDictionary<string, object?> GetValues(Tank tank) =>
            new Dictionary<string, object?>
            {
                { "Name", tank.Name },
                { "Description", tank.Description },
                { "Id", tank.Id },
                { "UnitId", tank.UnitId},
                { "Volume", tank.Volume},
                { "MaxVolume" ,tank.MaxVolume }
            };

        public static IList<string> GetTankKeys() => ["Name", "Description", "Id", "UnitId", "Volume", "MaxVolume"];
    }
}