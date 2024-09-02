namespace dotnetLearning.FactoryApp.Model
{
    public interface IFacility
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string? Description { get; set; }
    }
}