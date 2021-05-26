using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Quotes.DataAccess.Interfaces;
using Quotes.Model.DTOs.Request;
using Quotes.Model.DTOs.Response;

namespace Quotes.DataAccess.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IConfiguration _configuration;
        
        public AuthorRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AuthorResponse> AddAsync(AuthorRequest entity)
        {
            var insertQuery = $"INSERT INTO public.authors(name) VALUES (@AuthorName) RETURNING id;";
            var insertTokenQuery = $"INSERT INTO public.tokens(author_id, token) VALUES (@AuthorId, @Token);";
            var authorQuery = $"SELECT " +
                              $"a.id AS Id, " +
                              $"a.name AS AuthorName, " +
                              $"t.token AS Token " +
                              $"FROM public.authors a " +
                              $"LEFT JOIN public.tokens t ON t.author_id = a.id " +
                              $"WHERE a.id=@AuthorId;";
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var authorId = await connection.ExecuteScalarAsync<int>(insertQuery, entity);
            await connection.ExecuteAsync(insertTokenQuery, new { AuthorId = authorId, Token = Guid.NewGuid() });
            var author = await connection.QueryFirstOrDefaultAsync<AuthorResponse>(authorQuery, new { AuthorId = authorId });
            await connection.CloseAsync();
            return author;
        }

        public async Task<bool> CheckAuthorToken(int authorId, string hmacStr)
        {
            var query = $"SELECT token FROM public.tokens WHERE author_id=@AuthorId;";
            
            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            var authorToken = await connection.ExecuteScalarAsync<string>(query, new { AuthorId = authorId });
            await connection.CloseAsync();

            if (string.IsNullOrEmpty(authorToken))
                return false;
            
            var shaKeyBytes = System.Text.Encoding.UTF8.GetBytes(authorToken);
            using var shaAlgorithm = new HMACSHA256(shaKeyBytes);
            var signatureBytes = System.Text.Encoding.UTF8.GetBytes(authorId + authorToken);
            var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
            var signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2")));
            return string.Equals(signatureHashHex, hmacStr, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}