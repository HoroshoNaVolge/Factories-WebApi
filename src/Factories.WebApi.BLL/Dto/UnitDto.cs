using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Factories.WebApi.BLL.Dto
{
    public class UnitDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? FactoryId { get; set; }
    }
}
