using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KYC360TestAsignment.Models
{
 
    public class Address
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        [Key]
        [ForeignKey("Entity")]
        public string? EntityId { get; set; }

    }

}