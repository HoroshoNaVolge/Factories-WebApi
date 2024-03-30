using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Factories.WebApi.BLL.Dto
{
    public class FactoryDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
