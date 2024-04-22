using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Dakar
{
    public class Startup1  {
        public Startup1(IConfiguration configuration) {

            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // Aceasta metoda va fi apelata de catre runtime. Foloseste aceasta metoda pentru a adauga servicii catre container
        public void ConfigureServices(IServiceCollection services) {

            // Activam CORS
            services.AddCors(c =>
            {

                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            });

            // JSON Serializer
            //services.AddControllersWithViews().AddNewtonsoftJson(options =>
            //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(options =>
            //options.SerializerSettings.ContractResolver = new DefaultContractResolver());
           
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                // Add the custom Type converter here
                options.SerializerSettings.Converters.Add(new Dakar.Converters.TypeJsonConverter());
            });
           

            services.AddControllers();
        }

        // Aceasta metoda va fi apelata de catre runtime. Foloseste aceasta metoda pentru a configura pipeline-ul pentru request-urile de tip HTTP
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            // Activam CORS
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllers();

            });
        }    
    }
}
