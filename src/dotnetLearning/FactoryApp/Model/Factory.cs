namespace dotnetLearning.FactoryApp.Model
{
    public class Factory : IFacility, IEntity<Factory>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; set; }

        public IList<string> GetKeys() => GetFactoryKeys();

        public IDictionary<string, object?> GetValues() => GetValues(this);

        public override string ToString() =>
            $"Название завода: {Name}. Описание: {Description}. Id: {Id}.";

        public Factory Create(IDictionary<string, object?> values)
        {
            return new Factory
            {
                Name = values["Name"] as string ?? string.Empty,
                Description = values["Description"] as string,
                Id = values["Id"] as int? ?? 0
            };
        }

        public static IDictionary<string, object?> GetValues(Factory factory) =>
                new Dictionary<string, object?>
                {
                    { "Name", factory.Name },
                    { "Description", factory.Description },
                    { "Id", factory.Id },
                 };

        public static IList<string> GetFactoryKeys() => ["Name", "Description", "Id"];
    }
}