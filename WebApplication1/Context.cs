using Microsoft.EntityFrameworkCore;

namespace WebApplication1
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected Context()
        {
        }

        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(x => x.Author);
                entity.HasIndex(x => x.CountOfPages);
            });
        }
    }
}
