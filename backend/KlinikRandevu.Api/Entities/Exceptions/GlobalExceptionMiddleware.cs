using Entities.Exeptions.CustomExceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Entities.Exeptions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next=next;
            _logger=logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex,_logger);
            }

        }
        private static Task HandleExceptionAsync(HttpContext context,Exception ex,ILogger logger)
        {
            context.Response.ContentType="application/json";
            int statusCode;
            string message;
            if(ex is BadRequestException)
            {
                statusCode=(int)HttpStatusCode.BadRequest;
                message=ex.Message;
            }
            else if(ex is NotFoundException)
            {
                statusCode=(int)HttpStatusCode.NotFound;
                message=ex.Message;
            }
            else
            {
                statusCode=(int)HttpStatusCode.InternalServerError;
                message="Something went wrong. Please try again later.";
                logger.LogError(ex,ex.Message);
            }
            context.Response.StatusCode = statusCode;
            var result = JsonSerializer.Serialize(new
            {
                statusCode=statusCode,
                message=message
            });
            return context.Response.WriteAsync(result);
        }  
    }
}
