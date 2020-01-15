using System;
using System.Net;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.WebApi
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger _logger;

        public ErrorHandlingMiddleware(RequestDelegate next) //, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
           // _logger = logger;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex); //, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception) //, ILogger logger)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            ResponseErrorModel rem;
            if (exception is DomainValidationException validationException)
            {
                rem = new ResponseErrorModel()
                    {ErrorCode = validationException.ErrorCode, ErrorMessage = validationException.Message};
                code = HttpStatusCode.BadRequest;
            }
            else if (exception is DomainEntityNotFoundException notFoundException)
            {
                rem = new ResponseErrorModel()
                    { ErrorCode = notFoundException.Code, ErrorMessage = notFoundException.Message };
                code = HttpStatusCode.NotFound;

            }
            else
            {
                rem = new ResponseErrorModel()
                    { ErrorCode = code.ToString(), ErrorMessage = exception.Message };
            }
            //if (exception is BadRequestException) code = HttpStatusCode.BadRequest;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;

            //logger.LogError(exception, exception.Message);

            var result = JsonConvert.SerializeObject(rem);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }

}
