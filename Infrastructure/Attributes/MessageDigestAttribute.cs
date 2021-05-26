using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Quotes.DataAccess.Interfaces;

namespace Quotes.Infrastructure.Attributes
{
    public class MessageDigestAttribute : ActionFilterAttribute
    {
        private readonly IAuthorRepository _repository;

        public MessageDigestAttribute(IAuthorRepository repository)
        {
            _repository = repository;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var messageDigestHeader = request.Headers["X-MessageDigest"];
            var userIdHeader = request.Headers["X-UserId"];
            if (messageDigestHeader.Count <= 0 || userIdHeader.Count <= 0)
            {
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new UnauthorizedResult();
                return;
            }
            else
            {
                var isAuthenticated =
                     await _repository.CheckAuthorToken(Convert.ToInt32(userIdHeader), messageDigestHeader.ToString());
                if (isAuthenticated)
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }
    }
}