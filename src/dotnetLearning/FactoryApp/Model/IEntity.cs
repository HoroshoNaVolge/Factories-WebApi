namespace dotnetLearning.FactoryApp.Model
{
    public interface IEntity<T>
    {
        // Для создания эземпляров модели при десериализации из Excel
        // с целью исключения использования рефлексии, так как свойства или их порядок могут измениться
        T Create(IDictionary<string, object?> values);
        IDictionary<string, object?> GetValues();
        IList<string> GetKeys();
    }
}
