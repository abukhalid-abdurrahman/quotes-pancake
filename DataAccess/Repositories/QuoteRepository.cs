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
    public class QuoteRepository : IGenericRepository<QuoteRequest, QuoteResponse>, IQuoteRepository
    {
        private readonly IConfiguration _configuration;
        
        public QuoteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<QuoteResponse> GetByIdAsync(int id)
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
                        $"WHERE q.removed = False AND a.removed=False AND c.removed = False AND q.id = @id;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, id);
            await connection.CloseAsync();
            return quote;
        }

        public async Task<IReadOnlyList<QuoteResponse>> GetAllAsync()
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
            var quotes = await connection.QueryAsync<QuoteResponse>(query);
            await connection.CloseAsync();
            return quotes as IReadOnlyList<QuoteResponse>;
        }

        public async Task<QuoteResponse> AddAsync(QuoteRequest entity)
        {
            var query = $"INSERT INTO public.quotes(quote, " +
                        $"category_id, " +
                        $"author_id) " +
                        $"VALUES " +
                        $"(@QuoteText," +
                        $"@CategoryId," +
                        $"@AuthorId); RETURNING *;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, entity);
            await connection.CloseAsync();
            return quote;
        }

        public async Task<QuoteResponse> UpdateAsync(QuoteRequest entity)
        {
            var query = $"UPDATE public.quotes " +
                        $"SET quote=@QuoteText, " +
                        $"category_id=@CategoryId, " +
                        $"author_id=@AuthorId, " +
                        $"\"updateDate\"=CURRENT_DATE " +
                        $"WHERE id=@QuoteId; RETURNING *;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, entity);
            await connection.CloseAsync();
            return quote;        
        }

        public async Task<QuoteResponse> RemoveAsync(int id)
        {
            var query = $"UPDATE public.quotes " +
                        $"SET removed=True," +
                        $"\"updateDate\"=CURRENT_DATE " +
                        $"WHERE id=@QuoteId; RETURNING *;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, new { QuoteId = id });
            await connection.CloseAsync();
            return quote;
        }

        public async Task<QuoteResponse> GetRandomQuote()
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
                        $"WHERE q.removed = False AND a.removed=False AND c.removed = False AND q.id = @id " +
                        $"ORDER BY RANDOM() " +
                        $"LIMIT 1;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query);
            await connection.CloseAsync();
            return quote;
        }

        public async Task<IReadOnlyList<QuoteResponse>> GetQuotesByCategory(int categoryId)
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
                        $"WHERE q.removed = False AND a.removed = False AND c.removed = False AND c.id = @categoryId;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryAsync<QuoteResponse>(query, categoryId);
            await connection.CloseAsync();
            return quote as IReadOnlyList<QuoteResponse>;
        }
    }
}