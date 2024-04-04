using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Factories.WebApi.DAL.Entities
{
    public class Factory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // убираем private для возможности явной установки Id при первоначальной миграции БД
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
