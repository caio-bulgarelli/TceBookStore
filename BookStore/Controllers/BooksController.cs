using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Authorize]
    public class BooksController : ODataController
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: odata/Books
        [EnableQuery]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(Base.BooksBase.GetAll(_bookRepository));
        }

        // GET: odata/Books(5)
        [EnableQuery]
        [AllowAnonymous]
        public IActionResult Get(int key)
        {
            return Ok(Base.BooksBase.Get(_bookRepository, key));
        }

        // PUT: odata/Books(5) 
        /// <summary>
        /// Use MultipartFormDataContent to send Information to this endpoint.
        /// 
        /// Example:
        /// 
        /// HttpClient client = new HttpClient { BaseAddress = new Uri("https://localhost:44345/") };
        /// HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Put, "odata/Books");            
        /// MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----Book Boundary----");
        /// 
        /// var stream = new FileStream("C:\\Users\\Caio Bulgarelli\\Desktop\\ccomoprogramar.jpg", FileMode.Open);
        /// HttpContent fileContent = new StreamContent(stream);
        /// fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        /// {
        ///     Name = "file",
        /// FileName = "Teste.txt"
        /// };
        /// 
        /// multiPartContent.Add(new StringContent("0123456789"), "Isbn");
        /// multiPartContent.Add(new StringContent("Título"), "Title");
        /// multiPartContent.Add(new StringContent("Autor"), "Author");
        /// multiPartContent.Add(new StringContent("temp"), "Cover");
        /// multiPartContent.Add(fileContent);
        /// 
        /// message.Content = multiPartContent;
        /// 
        /// 
        /// HttpResponseMessage response = await client.SendAsync(message);
        /// 
        /// string responseContent = await response.Content.ReadAsStringAsync();
        ///
        /// </summary>
        /// <param name="book"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [EnableQuery]
        public async Task<IActionResult> PutBook([FromRoute] int id, [FromForm] Book book, [FromForm]IFormFile file)
        {
            try
            {
                await Base.BooksBase.UpdateAsync(_bookRepository, id, book, file);
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!await _bookRepository.Exists(id))
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

            return NoContent();
        }

        // POST: odata/Books
        /// <summary>
        /// Use MultipartFormDataContent to send Information to this endpoint.
        /// 
        /// Example:
        /// 
        /// HttpClient client = new HttpClient { BaseAddress = new Uri("https://localhost:44345/") };
        /// HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "odata/Books");            
        /// MultipartFormDataContent multiPartContent = new MultipartFormDataContent("----Book Boundary----");
        /// 
        /// var stream = new FileStream("C:\\Users\\Caio Bulgarelli\\Desktop\\ccomoprogramar.jpg", FileMode.Open);
        /// HttpContent fileContent = new StreamContent(stream);
        /// fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        /// {
        ///     Name = "file",
        ///     FileName = "Teste.txt"
        /// };
        /// 
        /// multiPartContent.Add(new StringContent("0123456789"), "Isbn");
        /// multiPartContent.Add(new StringContent("Título"), "Title");
        /// multiPartContent.Add(new StringContent("Autor"), "Author");
        /// multiPartContent.Add(new StringContent("temp"), "Cover");
        /// multiPartContent.Add(fileContent);
        /// 
        /// message.Content = multiPartContent;
        /// 
        /// 
        /// HttpResponseMessage response = await client.SendAsync(message);
        /// 
        /// string responseContent = await response.Content.ReadAsStringAsync();
        ///
        /// </summary>
        [EnableQuery]
        public async Task<IActionResult> PostBook([FromForm]Book book, [FromForm]IFormFile file)
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

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        /// <summary>
        /// Deleting information from relational databases is risky.
        /// Implement function to hide unwanted information.
        /// </summary>
        [EnableQuery]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
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