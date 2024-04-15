namespace dotnetLearning.JsonParserApp
{
    internal class Deal(DateTime date, string id, int sum)
    {
        public DateTime Date { get; set; } = date;
        public string Id { get; set; } = id;
        public int Sum { get; set; } = sum;
    }
}
