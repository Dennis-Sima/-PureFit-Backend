using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PureFit_REST.api.Model;
using Microsoft.EntityFrameworkCore;
using PureFit_REST.api.Extensions;
using PureFit_REST.api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace PureFit_REST.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
      
            services.AddControllers();

            // Soll ein gespeichertes Secret verwendet werden, kann folgende Zeite statt dessen
            // verwendet werden:
            string jwtSecret = Configuration["AppSettings:Secret"] ?? AuthService.GenerateRandom(1024);

            


            // JWT aktivieren, aber nicht standardmäßig aktivieren. Daher muss beim Controller
            //     [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
            // geschrieben werden. Wird nur eine API bereitgestellt, kann dieser Parameter auf
            // true gesetzt und Cookies natürlich deaktiviert werden.
            services.AddJwtAuthentication(jwtSecret, setDefault: false);

            // Cookies aktivieren. Dies ist für Blazor oder MVC Applikationen gedacht.
            services.AddCookieAuthentication(setDefault: true);

            // Instanzieren des Userservices mit einer Factorymethode. Diese übergibt das gespeicherte
            // Secret.
            services.AddScoped<AuthService>(services =>
                new AuthService(jwtSecret));

            // SQLite Datenbank mit dem Namen aus appsettings.json konfigurieren. Wird
            // automatisch bei den Controllern im Konstruktor übergeben, wenn sie den Parameter TestsContext
            // erwarten.
            services.AddDbContext<PureFitDbContext>(options =>
                options.UseSqlite($"DataSource={Configuration["AppSettings:Database"]}")
            );
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();

            // Muss NACH UseRouting() und VOR UseEndpoints() stehen.
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}