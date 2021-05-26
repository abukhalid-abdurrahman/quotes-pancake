using Quotes.DataAccess.Interfaces;

namespace Quotes.Infrastructure.Services
{
    public interface IUnitOfWork
    {
        IQuoteRepository QuoteRepository { get; }
        IAuthorRepository AuthorRepository { get; }
    }
}