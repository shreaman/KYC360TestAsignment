using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KYC360TestAsignment.Models
{
    
    public class Date
    {
        public string? DateType { get; set; }
        public DateTime? date { get; set; }

        [Key]
        [ForeignKey("Entity")]
        public string? EntityId { get; set; }

    }
}
