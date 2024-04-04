using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Factories.WebApi.DAL.Entities
{
    public class Tank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public double? Volume { get; set; }
        public double? MaxVolume { get; set; }

        public Unit? Unit { get; set; }
    }
}
