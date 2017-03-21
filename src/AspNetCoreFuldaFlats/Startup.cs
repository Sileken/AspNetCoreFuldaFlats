using System.IO;
using System.Linq;
using AspNetCoreFuldaFlats.Constants;
using AspNetCoreFuldaFlats.Database;
using AspNetCoreFuldaFlats.Database.Models;
using AspNetCoreFuldaFlats.Middlwares.HtmlFileExtensionMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspNetCoreFuldaFlats
{
    public class Startup
    {
        private IHostingEnvironment HostingEnvironment { get; }

        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebApiDataContext>(options =>
            {
                options.UseMySql(HostingEnvironment.EnvironmentName == "azure" ? "server=127.0.0.1;userid=azure;password=6#vWHD_$;database=fuldaflats;Port=49761;convertzerodatetime=True" : "server=localhost;user id=root;database=fuldaflats;convertzerodatetime=True");
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        } 

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = GlobalConstants.CookieAuthenticationSchema,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseHtmlFileExtensionMiddleware();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
                RequestPath = new PathString("/uploads")
            });

            app.UseMvc();
        }
    }
}
