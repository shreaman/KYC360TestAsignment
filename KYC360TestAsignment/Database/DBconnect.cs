using KYC360TestAsignment.Models;
using Microsoft.EntityFrameworkCore;

namespace KYC360TestAsignment.Database
{
    public class DBconnect : DbContext
    {
        public DBconnect(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Address>addresses { get; set; }    
        public DbSet<Date> dates { get; set; }
        public DbSet<Name> names { get; set; }  
        public DbSet<Entity> entitys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasOne<Entity>()             // Address has one Entity
                .WithMany(e => e.Addresses)        // Entity has many Addresses
                .HasForeignKey(a => a.EntityId);  // Foreign key relationship

            modelBuilder.Entity<Date>()
                .HasOne<Entity>()             // Date has one Entity
                .WithMany(e => e.Dates)     // Entity has many Dates
                .HasForeignKey(a => a.EntityId); // Foreign key relationship

            modelBuilder.Entity<Name>()
                .HasOne<Entity>()            // Name has one Entity
                .WithMany(e => e.Names)      // Entity has many Names
                .HasForeignKey(a => a.EntityId);  // Foreign key relationship

            base.OnModelCreating(modelBuilder);
        }



    }
}
