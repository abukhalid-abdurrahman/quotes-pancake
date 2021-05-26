using System.Threading.Tasks;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.DataAccess.Interfaces
{
    public interface IAuthorRepository
    {
        Task<AuthorResponse> AddAsync(AuthorRequest entity);
        Task<bool> CheckAuthorToken(int userId, string hmac);
    }
}