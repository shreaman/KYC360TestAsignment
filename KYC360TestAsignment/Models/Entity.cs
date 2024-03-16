
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KYC360TestAsignment.Models
{
    public class Entity : IEntity
    {
        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set ; }
        public string? Gender { get; set; }

        [Key]
        public string Id { get; set; }
        public List<Name> Names { get; set; }
    }
}
