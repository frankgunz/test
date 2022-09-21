using Hubbell.EHubb.Common.Context;
using Hubbell.EHubb.Common.Repository;
using Hubbell.EHubb.Common.TokenHandler;
using Hubbell.EHubb.Common.ErrorHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Debugging;
using System.Diagnostics;
using Hubbell.EHubb.Common.Swagger;
using Hubbell.EHubb.Common.ResponseHandler;
using Hubbell.EHubb.Common;
using Hubbell.EHubb.Common.Security;
using Hubbell.EHubb.Common.Security.Headers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Hubbell.EHubb.Common.APIComm;
using Hubbell.EHubb.Infrastructure.Messaging.Configuration;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Hubbell.EHubb.BACnetAPI
{
    public class Startup
    {
        
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("bacnetapi.appsettings.json", true, true)
               .AddJsonFile($"bacnetapi.appsettings.{env.EnvironmentName}.json", true, true)
               .AddEnvironmentVariables();
            _environment = env;
            _configuration = builder.Build();

        }

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_environment.IsDevelopment())
            {
                SelfLog.Enable(msg =>
                {
                    Debug.Print(msg);
                    Debugger.Break();
                });
            }

            // Response will go to the C/C++ BACnet Adapter need to figure out how to decompress there before adding this. 
            //services.AddEhubbResponseCompression();

            //services.AddControllers();
            services.AddControllers(option => { option.Filters.Add(new AuthorizeFilter()); });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = _ => new ValidationResult();
            });

            services.AddJwtTokenAuthentication(_configuration);
            services.AddServiceContext();
            //services.AddDatabaseConnection(_configuration);
            services.AddServiceScopes();

            services.AddDataProtection();
            services.AddSwaggerConfiguration(_configuration);
            services.AddLocalizationResource(_configuration);
            services.AddHealthChecks();
            services.AddHttpClient();
            services.AddHmacOptions(_configuration);
            services.AddAPICommSettings(_configuration);
            //services.UsePostgreProducer(_configuration);
            

            // Cross Origin Requests, I think we can leave this off for now since we are not sending or recieving signal R here
            //services.AddCors();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.Configure();

            app.UseHttpsRedirection();
            app.UseSwaggerSetup(_configuration);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseAndExceptionWrapper();

            app.UseAntiXssMiddleware();
            app.UseSecurityHeadersMiddleware(
                new SecurityHeadersBuilder()
                    .AddDefaultSecurePolicy());

            // Leave these off for now
            //app.UseCors(x => x
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());
            //app.UseEhubbResponseCompression();

            app.UseLocalizationResource(_configuration);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
