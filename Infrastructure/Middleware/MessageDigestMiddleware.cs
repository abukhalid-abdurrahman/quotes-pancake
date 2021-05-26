using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Quotes.DataAccess.Interfaces;

namespace Quotes.Infrastructure.Middleware
{
    public class MessageDigestMiddleware
    {
        private readonly RequestDelegate _next;
        private IAuthorRepository _repository;

        public MessageDigestMiddleware(RequestDelegate next, IAuthorRepository repository)
        {
            _next = next;
            _repository = repository;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var messageDigestHeader = context.Request.Headers["X-MessageDigest"];
            var userIdHeader = context.Request.Headers["X-UserId"];
            if (messageDigestHeader.Count <= 0 || userIdHeader.Count <= 0)
            {
                context.Response.StatusCode = 403;
            }
            else
            {
                var isAuthenticated =
                    await _repository.CheckAuthorToken(Convert.ToInt32(userIdHeader), messageDigestHeader.ToString());
                if (isAuthenticated)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 403;
                }
            }
        }
    }
}