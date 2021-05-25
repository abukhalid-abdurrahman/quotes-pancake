using System.Collections.Generic;
using System.Threading.Tasks;
using Quotes.Model.DTOs.Response;

namespace Quotes.DataAccess.Interfaces
{
    public interface IQuoteRepository
    {
        Task<QuoteResponse> GetRandomQuote();
        Task<IReadOnlyList<QuoteResponse>> GetQuotesByCategory(int categoryId);
    }
}