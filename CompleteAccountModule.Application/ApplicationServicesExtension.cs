using CompleteAccountModule.Application.Helpers;
using CompleteAccountModule.Application.Services;
using CompleteAccountModule.Application.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompleteAccountModule.Application
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ISMSService, SMSService>();


            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.Configure<MailSettings>(configuration.GetSection(MailSettings.SectionKey));
            services.Configure<TwilioSettings>(configuration.GetSection(TwilioSettings.SectionKey));


            return services;
        }
    }
}
