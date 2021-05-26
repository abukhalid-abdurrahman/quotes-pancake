using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quotes.Infrastructure.Services;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpPost]
        public async Task<AuthorResponse> Create(AuthorRequest authorRequest)
        {
            return await _unitOfWork.AuthorRepository.AddAsync(authorRequest);
        }
    }
}