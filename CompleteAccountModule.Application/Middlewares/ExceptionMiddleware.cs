using CompleteAccountModule.Application.Errors;
using ExamSystem.Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace CompleteAccountModule.Application.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                // log error in console
                logger.LogError(ex, ex.Message);


                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new ApiErrorResponse(500);

                //TODO:: cover all validation errors
                switch (ex)
                {
                    case UnauthorizedAccessException e:
                        // custom application error
                        responseModel.ErrorMessage = e.Message;
                        responseModel.StatusCode = (int)HttpStatusCode.Unauthorized;
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case ValidationException e:
                        // custom validation error
                        responseModel.ErrorMessage = e.Message;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        responseModel.ErrorMessage = e.Message;
                        responseModel.StatusCode = (int)HttpStatusCode.NotFound;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case InvalidOperationException e:
                        // not found error
                        responseModel.ErrorMessage = e.Message;
                        responseModel.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case DbUpdateException e:
                        // can't update error
                        responseModel = env.IsDevelopment()
                            ? new ApiExceptionResponse((int)HttpStatusCode.BadRequest, e.Message, e.StackTrace?.ToString())
                            : new ApiExceptionResponse((int)HttpStatusCode.BadRequest, e.Message);
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    default:
                        responseModel = env.IsDevelopment()
                            ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace?.ToString())
                            : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonResponse = JsonSerializer.Serialize(responseModel, jsonSerializerOptions);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
