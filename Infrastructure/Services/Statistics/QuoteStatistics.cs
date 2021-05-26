using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quotes.DataAccess.Interfaces;

namespace Quotes.Infrastructure.Services.Statistics
{
    public class QuoteStatistics : IQuoteStatistics
    {
        private readonly IQuoteRepository _repository;

        public QuoteStatistics(IQuoteRepository repository)
        {
            _repository = repository;
        }
        
        public async Task SendStatistics(string url)
        {
            var httpClient = new HttpClient();
            var statistics = await _repository.GetStatistics();
            await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(statistics), Encoding.UTF8, "application/json"));
        }
    }
}