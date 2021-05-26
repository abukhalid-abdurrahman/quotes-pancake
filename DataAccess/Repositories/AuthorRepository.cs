using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Quotes.DataAccess.Interfaces;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.DataAccess.Repositories
{
    public class AuthorRepository : IGenericRepository<AuthorRequest, AuthorResponse>
    {
        private readonly IConfiguration _configuration;
        
        public AuthorRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<AuthorResponse> GetByIdAsync(int id)
        {
            var query = $"SELECT " +
                        $"q.quote AS QuoteText, " +
                        $"q.id AS Id, " +
                        $"q.category_id AS CategoryId, " +
                        $"q.author_id AS AuthorId, " +
                        $"a.name AS AuthorName, " +
                        $"c.category AS CategoryText " +
                        $"FROM public.quotes q " +
                        $"LEFT JOIN public.authors a ON a.id = q.author_id " +
                        $"LEFT JOIN public.categories c ON c.id = q.category_id " +
                        $"WHERE q.removed = False AND a.removed = False AND c.removed = False AND q.id = @quoteId;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<AuthorResponse>(query, new { quoteId = id });
            await connection.CloseAsync();
            return quote;
        }

        public async Task<IReadOnlyList<AuthorResponse>> GetAllAsync()
        {
            var query = $"SELECT " +
                        $"q.quote AS QuoteText, " +
                        $"q.id AS Id, " +
                        $"q.category_id AS CategoryId, " +
                        $"q.author_id AS AuthorId, " +
                        $"a.name AS AuthorName, " +
                        $"c.category AS CategoryText " +
                        $"FROM public.quotes q " +
                        $"LEFT JOIN public.authors a ON a.id = q.author_id " +
                        $"LEFT JOIN public.categories c ON c.id = q.category_id " +
                        $"WHERE q.removed = False AND a.removed=False AND c.removed = False;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quotes = await connection.QueryAsync<AuthorResponse>(query);
            await connection.CloseAsync();
            return quotes as IReadOnlyList<AuthorResponse>;
        }

        public async Task<AuthorResponse> AddAsync(AuthorRequest entity)
        {
            var query = $"INSERT INTO public.quotes(quote, " +
                        $"category_id, " +
                        $"author_id) " +
                        $"VALUES " +
                        $"(@QuoteText," +
                        $"@CategoryId," +
                        $"@AuthorId) RETURNING *;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<AuthorResponse>(query, entity);
            await connection.CloseAsync();
            return quote;
        }

        public async Task<AuthorResponse> UpdateAsync(AuthorRequest entity)
        {
            var query = $"UPDATE public.quotes " +
                        $"SET quote=@QuoteText, " +
                        $"category_id=@CategoryId, " +
                        $"author_id=@AuthorId, " +
                        $"\"updateDate\"=CURRENT_DATE " +
                        $"WHERE id=@QuoteId RETURNING *;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<AuthorResponse>(query, entity);
            await connection.CloseAsync();
            return quote;
        }

        public async Task<AuthorResponse> RemoveAsync(int id)
        {
            var query = $"UPDATE public.quotes " +
                        $"SET removed=True," +
                        $"\"updateDate\"=CURRENT_DATE " +
                        $"WHERE id=@QuoteId RETURNING *;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<AuthorResponse>(query, new { QuoteId = id });
            await connection.CloseAsync();
            return quote;
        }
    }
}