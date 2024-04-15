namespace dotnetLearning.FactoryApp.Service.FacilityService
{
    public partial class FacilityServiceOptions
    {
        public const string SectionName = "FilePath";
        public string? FacilitiesJsonFilePath { get; set; }
        public string? FacilitiesExcelFilePath { get; set; }
        public string? FacilitiesDbConnectionString { get; set; }
    }
}