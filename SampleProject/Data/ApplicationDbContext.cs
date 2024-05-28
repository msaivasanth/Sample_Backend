using Microsoft.EntityFrameworkCore;
using SampleProject.Models;

namespace SampleProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } 
        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Sample_Name_1"
                },
                new Villa
                {
                    Id = 2,
                    Name = "Sample_Name_2"
                },
                new Villa
                {
                    Id = 3,
                    Name = "Sample_Name_3"
                }
            );
            //base.OnModelCreating(modelBuilder);
        }
    }
}
