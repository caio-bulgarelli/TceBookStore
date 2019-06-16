using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStore.Controllers.Base
{
    /// <summary>
    /// Class that presents a generic implementation of the database operations and buisiness logic to simplify test procedures.
    /// Controllers are only responsible for simple control and properly expressing the errors.
    /// This implementation allows additional parameters to be added to the query according to the project since Get methods return IQueriable<T>
    /// </summary>
    public static class BooksBase
    {
        public static IQueryable<Book> GetAll(IBookRepository bookRepository) => bookRepository.Query();

        public static IQueryable<Book> Get(IBookRepository bookRepository, int id) => bookRepository.Query().Where(b => b.Id == id).Take(1);

        /// <summary>
        /// Generic implementation of the book business logic for book update.
        /// </summary>
        public static async Task UpdateAsync(IBookRepository bookRepository, int id, Book book, IFormFile file)
        {
            book.Validate();

            //User cannot change the book id.
            if (id != book.Id)
            {
                throw new UnauthorizedException();
            }

            Book oldBook = bookRepository.Query().FirstOrDefault(b => b.Id == book.Id);
            if (oldBook == null)
            {
                throw new NotFoundException();
            }

            //If user is changing Isbn, new Isbn needs to be validated against the database.
            if (oldBook.Isbn != book.Isbn)
            {
                if (await bookRepository.Query().AnyAsync(b => b.Isbn == book.Isbn))
                {
                    throw new BadRequestException("ISBN already registered.");
                }
            }

            //Only save new cover when user uploaded a file or changed the link.
            if (file != null || book.Cover != oldBook.Cover)
            {
                string oldCover = oldBook.Cover;
                await SaveCover(book, file);

                //Deletes oldCover image.
                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldCover);
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
            }

            await bookRepository.UpdateAsync(oldBook, book);
        }

        /// <summary>
        /// Generic implementation of the book business logic for book insertion.
        /// </summary>
        public static async Task AddAsync(IBookRepository bookRepository, Book book, IFormFile file)
        {
            book.Validate();

            //Check if there is already a book with that Isbn.
            //Another check is made at the database by unique constraint.
            if (await bookRepository.Query().AnyAsync(b => b.Isbn == book.Isbn))
            {
                throw new BadRequestException("ISBN already registered.");
            }

            await SaveCover(book, file);
            await bookRepository.InsertAsync(book);
        }

        /// <summary>
        /// Creates a file in the server to store the book cover image.
        /// Downloads file or link image.
        /// </summary>
        private static async Task SaveCover(Book book, IFormFile file)
        {
            string filename = $"{Path.GetRandomFileName()}.";

            filename += file == null ? book.Cover?.Split('.').Last() : file.FileName.Split('.').Last();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", filename);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            //Prioritizes user files.
            if (file == null || file.Length == 0)
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(book.Cover);

                //Checks if link target is an image.
                if (!response.Content.Headers.ContentType.MediaType.StartsWith("image/"))
                {
                    throw new BadRequestException("File is null and link target is not an image.");
                }

                var responseStream = await response.Content.ReadAsStreamAsync();

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await responseStream.CopyToAsync(stream);
                }
            }
            else
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            book.Cover = $"/images/{filename}";
        }

        /// <summary>
        /// Generic implementation of the book business logic for book deletion.
        /// </summary>
        public static async Task<Book> DeleteAsync(IBookRepository bookRepository, int id)
        {
            if (!bookRepository.Query().Any(b => b.Id == id))
            {
                throw new NotFoundException();
            }

            return await bookRepository.DeleteAsync(id);
        }
    }
}
