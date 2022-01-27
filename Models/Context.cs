using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class Context : DbContext
    {
        public DbSet<Prodavnica> Prodavnice { get; set; }

        public DbSet<Proizvod> Proizvodi { get; set; }

        public DbSet<Sastojak> Sastojci { get; set; }

        public Context(DbContextOptions options)
        :base(options)
        {
            
        }
    }
}
