using BookStore.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository
{
    public class BookRepository : GenericRepository<Book, int>, IBookRepository
    {
        public BookRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
