using Microsoft.Extensions.DependencyInjection;
using Quotes.DataAccess.Interfaces;
using Quotes.DataAccess.Repositories;
using Quotes.Infrastructure.Attributes;
using Quotes.Infrastructure.Services;

namespace Quotes.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<MessageDigestAttribute>();
            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddTransient<IQuoteRepository, QuoteRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}