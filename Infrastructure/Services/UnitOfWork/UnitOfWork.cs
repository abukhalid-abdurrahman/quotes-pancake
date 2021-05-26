using Quotes.DataAccess.Interfaces;

namespace Quotes.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IQuoteRepository quoteRepository, 
            IAuthorRepository authorRepository)
        {
            QuoteRepository = quoteRepository;
            AuthorRepository = authorRepository;
        }
        
        public IQuoteRepository QuoteRepository { get; }
        public IAuthorRepository AuthorRepository { get; }
    }
}