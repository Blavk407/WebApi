using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
    }

    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Author { get; set; } = string.Empty;

        public int CountOfPages { get; set; }
    }
}
