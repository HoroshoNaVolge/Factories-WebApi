namespace dotnetLearning.FactoryApp.Model
{
    public class Unit : IFacility, IEntity<Unit>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; set; }
        public int FactoryId { get; set; }


        public IList<string> GetKeys() => GetUnitKeys();
        public IDictionary<string, object?> GetValues() => GetValues(this);
        public override string ToString() =>
            $"Название установки: {Name}. Описание: {Description}. Id: {Id}. Id завода {FactoryId}";

        public Unit Create(IDictionary<string, object?> values)
        {
            return new Unit
            {
                Name = values["Name"] as string ?? string.Empty,
                Description = values["Description"] as string,
                Id = values["Id"] as int? ?? 0,
                FactoryId = values["FactoryId"] as int? ?? 0
            };
        }

        public static IDictionary<string, object?> GetValues(Unit unit) =>
           new Dictionary<string, object?>
           {
                { "Name", unit.Name },
                { "Description", unit.Description },
                { "Id", unit.Id },
                { "FactoryId", unit.FactoryId},
           };

        public static IList<string> GetUnitKeys() => ["Name", "Description", "Id", "FactoryId"];
    }
}