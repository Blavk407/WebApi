using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : ControllerBase
    {
        private readonly Context _context;

        public BooksController(Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Post(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok(book);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get([FromQuery] BookFilter filter)
        {
            Console.WriteLine(filter.Author);
            var books = await _context.Books
                .Where(
                x => x.CountOfPages >= filter.MinCountOfPages && 
                x.CountOfPages <= filter.MaxCountOfPages && (
                string.IsNullOrEmpty(filter.Author) || x.Author == filter.Author))
                .ToListAsync();
            return Ok(books);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Book>> Delete([FromRoute] int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return BadRequest();
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Book>> Put([FromRoute] int id, [FromBody] Book model)
        {
            var putBook = await _context.Books.FindAsync(id);
            if (putBook == null)
                return BadRequest();
            putBook.Author = model.Author;
            putBook.Name = model.Name;
            putBook.CountOfPages = model.CountOfPages;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook([FromRoute] int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }
    }
}
