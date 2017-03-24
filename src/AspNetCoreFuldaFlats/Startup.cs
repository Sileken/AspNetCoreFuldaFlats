using System.IO;
using AspNetCoreFuldaFlats.AuthorizedJsonSerialization;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Middlwares.HtmlFileExtensionMiddleware;
using AspNetCoreFuldaFlats.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCoreFuldaFlats
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath + "/Configs")
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();

            HostingEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            GlobalConstants.DefaultThumbnailUrl = Configuration.GetValue<string>("AppSettings:DefaultThumbnailUrl");

            services.AddDbContext<WebApiDataContext>(
                options =>
                {
                    options.UseMySql(HostingEnvironment.EnvironmentName == "azure"
                        ? Configuration.GetValue<string>("AppSettings:AzureMySqlConnectionString")
                        : Configuration.GetValue<string>("AppSettings:DefaultMySqlConnectionString"));
                });

            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "FuldaFlats API",
                    Description = "FuldaFlats ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Patrick Hasenauer", Email = "patrick.hasenauer@informatik.hs-fulda.de" },
                });
               options.IncludeXmlComments(PlatformServices.Default.Application.ApplicationBasePath + "\\AspNetCoreFuldaFlats.xml");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<MvcJsonOptions> mvcOptions, IHttpContextAccessor httpContextAccessor)
        {
            mvcOptions.Value.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            mvcOptions.Value.SerializerSettings.ContractResolver =
                new JsonAuthorizedContractResolver(httpContextAccessor);

            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = GlobalConstants.CookieAuthenticationSchema,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseHtmlFileExtensionMiddleware();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
                RequestPath = new PathString("/uploads")
            });

            app.UseSession();
            app.UseMvc();

            app.UseSwagger(options => options.RouteTemplate = "api/{documentName}/swagger.json");
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api";
                options.SwaggerEndpoint("/api/v1/swagger.json", "FuldaFlats API V1");
            });
        }
    }
}
