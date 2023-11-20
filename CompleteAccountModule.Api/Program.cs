
using CompleteAccountModule.Api.Extensions;
using CompleteAccountModule.Application;
using CompleteAccountModule.Application.Middlewares;
using CompleteAccountModule.Infrastructure.Database;
using ExamSystem.Application.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompleteAccountModule.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddApplicationsServices(builder.Configuration);

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(x => x.Value?.Errors.Count > 0)
                                            .SelectMany(x => x.Value.Errors)
                                            .Select(x => x.ErrorMessage)
                                            .ToList();

                    var validationErrorResponse = new ApiValidationErrorResponse() { Errors = errors };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            builder.Services.AddSwaggerServices();

            var app = builder.Build();

            await app.UpdateDatabase();

            app.UseSwaggerMiddlewares();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}