using BookStore.Exceptions;
using BookStore.Models;
using BookStore.Repository;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using static BookStore.Controllers.Base.BooksBase;

namespace BookStore.Tests
{
    /// <summary>
    /// Tests BookController information. 
    /// Since all business logic was extracted to Base.BookBase, that class will be tested instead. 
    /// There is no need to test BOTH BooksController and BooksMvcController. 
    /// But new tests should be added if functionality diverges between them.
    /// This is done to prevent Mocking HttpContext and Owin Context with user.
    /// </summary>
    public class BooksControllerTests
    {
        /// <summary>
        /// Tests creating a book without file input in form data.
        /// Uses a link to a file on the web instead.
        /// Function used when book image was found on google books and user did not upload an image itself.
        /// </summary>
        [Fact]
        public async void Create_ShouldCreateWithLink()
        {
            IBookRepository repository = GetMockedBookRepository();

            Book book = new Book
            {
                Author = "Neil Gaiman",
                Cover = "https://nerdstore.com.br/wp-content/uploads/2019/01/livro-mitologia-nordica.jpg",
                Isbn = "9788551001288",
                Title = "Mitologia Nórdica",
                Price = 34.9,
                PublicationDate = 2017
            };

            //Should not throw any exceptions.
            await AddAsync(repository, book, null);
        }

        /// <summary>
        /// Tests creating a book with a file input in form data.
        /// </summary>
        [Fact]
        public async void Create_ShouldCreateWithFile()
        {
            IBookRepository repository = GetMockedBookRepository();
            IFormFile file = GetMockedFormFile();

            Book book = new Book
            {
                Author = "Neil Gaiman",
                Isbn = "9788551001288",
                Title = "Mitologia Nórdica",
                Price = 34.9,
                PublicationDate = 2017
            };

            //Should not throw any exceptions.
            await AddAsync(repository, book, file);
        }

        /// <summary>
        /// Tests if server will return error if book was sent without cover.
        /// </summary>
        [Fact]
        public async void Create_FailCreateWithoutCover()
        {
            IBookRepository repository = GetMockedBookRepository();

            Book book = new Book
            {
                Author = "Neil Gaiman",
                Isbn = "9788551001288",
                Title = "Mitologia Nórdica",
                Price = 34.9,
                PublicationDate = 2017
            };

            //Exception occurs when cover has an invalid link.
            await Assert.ThrowsAsync<InvalidOperationException>(() => AddAsync(repository, book, null));

            book.Cover = "http://www.google.com";

            //Exception occurs when link does not return an image.
            await Assert.ThrowsAsync<BadRequestException>(() => AddAsync(repository, book, null));
        }

        /// <summary>
        /// Tests if server will return error if book was sent without cover.
        /// </summary>
        [Fact]
        public async void Create_FailCreateOtherErrors()
        {
            IBookRepository repository = GetMockedBookRepository();

            Book book = new Book();

            try
            {
                await AddAsync(repository, book, null);
            }
            catch (BadRequestException bre)
            {
                Assert.NotNull(bre.ValidationResults);
                Assert.Equal(5, bre.ValidationResults.Count);
            }

            Book isbnBook = new Book
            {
                Author = "David L. Nelson, Michal M. Cox",
                Cover = "../../Images/fakecover.png",
                Id = 1,
                Isbn = "9788576059349",
                Title = "Princípios de Bioquímica de Lehninger",
                Price = 450.0,
                PublicationDate = 2014
            };

            try
            {
                await AddAsync(repository, isbnBook, null);
            }
            catch (BadRequestException bre)
            {
                Assert.NotNull(bre.Message);
                Assert.Equal("ISBN already registered.", bre.Message);
            }
        }

        /// <summary>
        /// Tests Update Information on 
        /// Link cover upload is only available on create.
        /// </summary>
        [Fact]
        public async void Update_ShouldUpdateWithFile()
        {
            IBookRepository repository = GetMockedBookRepository();
            IFormFile file = GetMockedFormFile();

            Book book = new Book
            {
                Author = "David L. Nelson, Michal M. Cox",
                Id = 1,
                Isbn = "9788582710722",
                Title = "Princípios de Bioquímica de Lehninger",
                Price = 450.0,
                PublicationDate = 2014
            };

            //Should not throw any exceptions.
            await UpdateAsync(repository, 1, book, file);
        }

        /// <summary>
        /// Tests Update Information on 
        /// Link cover upload is only available on create.
        /// </summary>
        [Fact]
        public async void Update_FailUpdate()
        {
            IBookRepository repository = GetMockedBookRepository();

            Book book = new Book
            {
                Id = 1,
            };

            try
            {
                await UpdateAsync(repository, 1, book, null);
            }
            catch (BadRequestException bre)
            {
                Assert.NotNull(bre.ValidationResults);
                Assert.Equal(5, bre.ValidationResults.Count);
            }

            Book changeIsbnBook = new Book
            {
                Author = "David L. Nelson, Michal M. Cox",
                Cover = "../../Images/fakecover.png",
                Id = 1,
                Isbn = "9788576059349",
                Title = "Princípios de Bioquímica de Lehninger",
                Price = 450.0,
                PublicationDate = 2014
            };

            try
            {
                await UpdateAsync(repository, 1, changeIsbnBook, null);
            }
            catch (BadRequestException bre)
            {
                Assert.NotNull(bre.Message);
                Assert.Equal("ISBN already registered.", bre.Message);
            }

            Book changeIdBook = new Book
            {
                Author = "David L. Nelson, Michal M. Cox",
                Cover = "../../Images/fakecover.png",
                Id = 3,
                Isbn = "9788582710722",
                Title = "Princípios de Bioquímica de Lehninger",
                Price = 450.0,
                PublicationDate = 2014
            };

            await Assert.ThrowsAsync<UnauthorizedException>(() => UpdateAsync(repository, 1, changeIdBook, null));

            Book idNotFoundBook = new Book
            {
                Author = "David L. Nelson, Michal M. Cox",
                Cover = "../../Images/fakecover.png",
                Id = 12,
                Isbn = "9788582710722",
                Title = "Princípios de Bioquímica de Lehninger",
                Price = 450.0,
                PublicationDate = 2014
            };

            await Assert.ThrowsAsync<NotFoundException>(() => UpdateAsync(repository, 12, idNotFoundBook, null));
        }

        /// <summary>
        /// Tests multiple scenarios of getting multiple books, including filtering and sorting.
        /// </summary>
        [Fact]
        public void Get_AllBooks()
        {
            IBookRepository repository = GetMockedBookRepository();

            IQueryable<Book> result = GetAll(repository);

            Assert.Equal(6, result.Count());
        }

        /// <summary>
        /// Tests getting info on a single book.
        /// </summary>
        [Fact]
        public void Get_SingleBook()
        {
            IBookRepository repository = GetMockedBookRepository();

            IQueryable<Book> result = Get(repository, 3);

            Assert.Equal(1, result.Count());
            Assert.Equal(3, result.First().Id);
        }

        [Fact]
        public async void Delete_ShouldDeleteWithId()
        {
            IBookRepository repository = GetMockedBookRepository();

            await DeleteAsync(repository, 1);
        }

        [Fact]
        public async void Delete_FailDeleteWithId()
        {
            IBookRepository repository = GetMockedBookRepository();

            await Assert.ThrowsAsync<NotFoundException>(() => DeleteAsync(repository, 10));
        }

        private static IQueryable<Book> GetBooks()
        {
            return new List<Book>
            {
                new Book
                {
                    Author = "David L. Nelson, Michal M. Cox",
                    Cover = "../../Images/fakecover.png",
                    Id = 1,
                    Isbn = "9788582710722",
                    Title = "Princípios de Bioquímica de Lehninger",
                    Price = 450.0,
                    PublicationDate = 2014
                },
                new Book
                {
                    Author = "Paul Deitel, Harvey Deitel",
                    Cover = "../../Images/fakecover.png",
                    Id = 2,
                    Isbn = "9788576059349",
                    Title = "C: como programar",
                    Price = 456.0,
                    PublicationDate = 2011
                },
                new Book
                {
                    Author = "George F. Luger",
                    Cover = "../../Images/fakecover.png",
                    Id = 3,
                    Isbn = "9788581435503",
                    Title = "Inteligência Artificial",
                    Price = 234.9,
                    PublicationDate = 2013
                },
                new Book
                {
                    Author = "George C. Schatz, Mark A. Ratner",
                    Cover = "../../Images/fakecover.png",
                    Id = 4,
                    Isbn = "0130750115",
                    Title = "Quantum Mechanics in Chemistry",
                    Price = 34.5,
                    PublicationDate = 1993
                },
                new Book
                {
                    Author = "Shane Cook",
                    Cover = "../../Images/fakecover.png",
                    Id = 5,
                    Isbn = "9780124159334",
                    Title = "Cuda Programming",
                    Price = 134.5,
                    PublicationDate = 2013
                },
                new Book
                {
                    Author = "Eliezer J. Barreiro, Carlos Alberto Manssour Fraga",
                    Cover = "../../Images/fakecover.png",
                    Id = 6,
                    Isbn = "9788582711170",
                    Title = "Química Medicinal: As Bases Moleculares da Ação dos Fármacos",
                    Price = 234.5,
                    PublicationDate =2012
                }
            }.AsQueryable().BuildMock().Object;

        }

        private static IBookRepository GetMockedBookRepository()
        {
            Mock<IBookRepository> _bookRepositoryMock = new Mock<IBookRepository>();
            _bookRepositoryMock.Setup(m => m.Query()).Returns(GetBooks);
            return _bookRepositoryMock.Object;
        }

        private static IFormFile GetMockedFormFile()
        {
            Mock<IFormFile> fileMock = new Mock<IFormFile>();

            //Setup mock file using a memory stream
            string content = "Fake Information the will be saved to file.";
            string fileName = "fakeFileName.jpg";

            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            return fileMock.Object;
        }
    }
}
