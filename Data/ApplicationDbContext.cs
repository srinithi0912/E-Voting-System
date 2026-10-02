using Microsoft.EntityFrameworkCore;
using EVotingSystem.Models;

namespace EVotingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Voter> Voter { get; set; }
        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<Vote> Vote { get; set; }
    }
}