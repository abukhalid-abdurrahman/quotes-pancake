using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quotes.Infrastructure.Attributes;
using Quotes.Infrastructure.Services;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(MessageDigestAttribute))]
    public class QuotesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public QuotesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        public async Task<IReadOnlyList<QuoteResponse>> GetAll()
        {
            return await _unitOfWork.QuoteRepository.GetAllAsync();
        }
        
        [HttpGet("{id}")]
        public async Task<QuoteResponse> Get(int id)
        {
            return await _unitOfWork.QuoteRepository.GetByIdAsync(id);
        }
        
        [HttpGet("Category/{id}")]
        public async Task<IReadOnlyList<QuoteResponse>> GetByCategory(int id)
        {
            return await _unitOfWork.QuoteRepository.GetQuotesByCategory(id);
        }
        
        [HttpGet("Random")]
        public async Task<QuoteResponse> GetRandomQuote()
        {
            return await _unitOfWork.QuoteRepository.GetRandomQuote();
        }
        
        [HttpDelete("{id}")]
        public async Task<QuoteResponse> RemoveQuote(int id)
        {
            return await _unitOfWork.QuoteRepository.RemoveAsync(id);
        }
        
        [HttpPost]
        public async Task<QuoteResponse> Create(QuoteRequest quoteRequest)
        {
            return await _unitOfWork.QuoteRepository.AddAsync(quoteRequest);
        }
        
        [HttpPut]
        public async Task<QuoteResponse> Edit(QuoteRequest quoteRequest)
        {
            return await _unitOfWork.QuoteRepository.UpdateAsync(quoteRequest);
        }
    }
}