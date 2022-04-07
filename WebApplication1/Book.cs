using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [DefaultValue("name")]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [DefaultValue("author")]
        public string Author { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        [DefaultValue(1)]
        public int CountOfPages { get; set; } = 1;
    }
}
