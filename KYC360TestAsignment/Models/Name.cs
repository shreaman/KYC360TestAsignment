using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KYC360TestAsignment.Models
{
    public class Name
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }

        [Key]
        [ForeignKey("Entity")]
        public string? EntityId { get; set; }
    }
}