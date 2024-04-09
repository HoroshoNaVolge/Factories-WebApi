using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Factories.WebApi.BLL.Dto
{
    public class TankDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? UnitId { get; set; }
        public int Volume { get; set; }
        public int MaxVolume { get; set; }
    }
}
