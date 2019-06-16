using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class BooksMvcController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BooksMvcController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: BooksMvc
        public IActionResult Index()
        {
            return View(Base.BooksBase.GetAll(_bookRepository));
        }

        // GET: BooksMvc/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = Base.BooksBase.Get(_bookRepository, (int)id);

            if (!book.Any())
            {
                return NotFound();
            }

            return View(book.First());
        }

        // GET: BooksMvc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BooksMvc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Author,Title,Price,PublicationDate,Cover")] Book book, IFormFile file)
        {
            try
            {
                await Base.BooksBase.AddAsync(_bookRepository, book, file);
            }
            catch (UnauthorizedException)
            {
                return Unauthorized();
            }
            catch (BadRequestException bre)
            {
                if (bre.ValidationResults != null)
                {
                    return BadRequest(bre.ValidationResults);
                }

                return BadRequest(bre.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: BooksMvc/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = Base.BooksBase.Get(_bookRepository, (int)id);

            if (!book.Any())
            {
                return NotFound();
            }

            return View(book.First());
        }

        // POST: BooksMvc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Isbn,Author,Title,Price,PublicationDate,Cover")] Book book, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await Base.BooksBase.UpdateAsync(_bookRepository, id, book, file);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookRepository.Exists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (UnauthorizedException)
                {
                    return Unauthorized();
                }
                catch (BadRequestException bre)
                {
                    if (bre.ValidationResults != null)
                    {
                        return BadRequest(bre.ValidationResults);
                    }

                    return BadRequest(bre.Message);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        /// <summary>
        /// Deleting information from relational databases is risky.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            Book book;

            try
            {
                book = await Base.BooksBase.DeleteAsync(_bookRepository, id);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return Ok(book);
        }
    }
}
