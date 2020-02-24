
using Kopis_Showcase.Models;
using Microsoft.EntityFrameworkCore;

namespace Kopis_Showcase.Data
{
    public class PersonContext : DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<MaritalStatus> MaritalStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Gender>().ToTable("Gender");
            modelBuilder.Entity<MaritalStatus>().ToTable("MaritalStatus");
        }
    }
}
