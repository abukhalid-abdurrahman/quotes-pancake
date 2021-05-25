using Quotes.DataAccess.Interfaces;

namespace Quotes.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IQuoteRepository quoteRepository)
        {
            QuoteRepository = quoteRepository;
        }
        
        public IQuoteRepository QuoteRepository { get; }
    }
}