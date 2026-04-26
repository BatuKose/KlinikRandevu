using Entities.Exeptions.CustomExceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Entities.Exeptions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next=next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }

        }
        private static Task HandleExceptionAsync(HttpContext context,Exception ex)
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
