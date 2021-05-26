using System.Threading.Tasks;

namespace Quotes.Infrastructure.Services.Statistics
{
    public interface IQuoteStatistics
    {
        Task SendStatistics(string url);
    }
}