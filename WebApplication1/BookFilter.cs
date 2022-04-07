using System.ComponentModel.DataAnnotations;

namespace WebApplication1
{
    public class BookFilter
    {
        [Range(1, int.MaxValue)]
        public int MaxCountOfPages { get; set; } = int.MaxValue;

        [Range(1, int.MaxValue)]
        public int MinCountOfPages { get; set; } = 1;

        [MaxLength(100)]
        public string Author { get; set; } = string.Empty;
    }
}
