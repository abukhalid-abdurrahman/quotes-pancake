﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Quotes.DataAccess.Interfaces;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.DataAccess.Repositories
{
    public class QuoteRepository : IQuoteRepository
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
                        $"WHERE q.removed = False AND a.removed = False AND c.removed = False AND q.id = @quoteId;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, new { quoteId = id });
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
                        $"@AuthorId) RETURNING id AS Id, author_id AS AuthorId, quote AS QuoteText, category_id AS CategoryId;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, entity);
            await connection.CloseAsync();
            await CreateStatistics(quote.Id, quote.AuthorId, "created");
            return quote;
        }

        public async Task<QuoteResponse> UpdateAsync(QuoteRequest entity)
        {
            var query = $"UPDATE public.quotes " +
                        $"SET quote=@QuoteText, " +
                        $"category_id=@CategoryId, " +
                        $"author_id=@AuthorId, " +
                        $"\"updateDate\"=CURRENT_DATE " +
                        $"WHERE id=@QuoteId RETURNING *;";
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
                        $"WHERE id=@QuoteId RETURNING id AS Id, author_id AS AuthorId, quote AS QuoteText, category_id AS CategoryId;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query, new { QuoteId = id });
            await connection.CloseAsync();
            await CreateStatistics(quote.Id, quote.AuthorId, "removed");
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
                        $"WHERE q.removed = False AND a.removed=False AND c.removed = False " +
                        $"ORDER BY RANDOM() " +
                        $"LIMIT 1;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quote = await connection.QueryFirstOrDefaultAsync<QuoteResponse>(query);
            await connection.CloseAsync();
            return quote;
        }

        public async Task<IReadOnlyList<QuoteResponse>> GetQuotesByCategory(int id)
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
            var quote = await connection.QueryAsync<QuoteResponse>(query, new { categoryId = id });
            await connection.CloseAsync();
            return quote as IReadOnlyList<QuoteResponse>;
        }

        public async Task<IReadOnlyList<QuoteStatisticsResponse>> GetStatistics()
        {
            var query = $@"SELECT qh.quote_id AS Id,
	                              qh.action AS Action,
	                              qh.""insertDate"" AS ActionDate,
                                  a.name AS AuthorName,
                                  a.id AS AuthorId,
                                  c.category AS CategoryText,
                                  c.id AS CategoryId,
                                  q.quote AS QuoteText
                                FROM quotes_history qh
                                LEFT JOIN quotes q ON q.id = qh.quote_id
                                LEFT JOIN authors a ON a.id = qh.author_id
                                LEFT JOIN categories c ON c.id = q.category_id
                                ORDER BY qh.""insertDate"" DESC;";
            
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var quotes = await connection.QueryAsync<QuoteStatisticsResponse>(query);
            await connection.CloseAsync();
            return quotes as IReadOnlyList<QuoteStatisticsResponse>;
        }

        public async Task CreateStatistics(int quoteId, int authorId, string action)
        {
            var query = $"INSERT INTO public.quotes_history(quote_id, action, author_id) VALUES (@QuoteId, @Action, @AuthorId);";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            await connection.ExecuteAsync(query, new { QuoteId = quoteId, AuthorId = authorId, Action = action });
            await connection.CloseAsync();
        }
    }
}