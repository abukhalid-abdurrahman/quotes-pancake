using System.Collections.Generic;
using System.Threading.Tasks;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.DataAccess.Interfaces
{
    public interface IQuoteRepository : IGenericRepository<QuoteRequest, QuoteResponse>
    {
        Task<QuoteResponse> GetRandomQuote();
        Task<IReadOnlyList<QuoteResponse>> GetQuotesByCategory(int categoryId);
        Task<IReadOnlyList<QuoteStatisticsResponse>> GetStatistics();
        Task CreateStatistics(int quoteId, int authorId, string action);
    }
}