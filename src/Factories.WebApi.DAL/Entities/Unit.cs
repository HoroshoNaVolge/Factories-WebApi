using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace Factories.WebApi.DAL.Entities
{
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int FactoryId { get; set; }

        public required Factory Factory { get; set; }
    }
}
