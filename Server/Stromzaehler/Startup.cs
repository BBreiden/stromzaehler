using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stromzaehler.Analysis;
using Stromzaehler.Models;

namespace Stromzaehler
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> log)
        {
            Configuration = configuration;
            Log = log;
        }

        public IConfiguration Configuration { get; }
        public ILogger<Startup> Log { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSingleton<Status>(new Status(DateTime.Now, Configuration));

            //services.AddDbContext<BlinkDataContext>(options => options.UseInMemoryDatabase());
            var dbLocation = Configuration.GetSection("Database:Path").Value;
            if (string.IsNullOrEmpty(dbLocation))
            {
                throw new InvalidOperationException("Missing database path.");
            }
            if (!Directory.Exists(dbLocation))
            {
                throw new InvalidOperationException($"Directory does not exist: {dbLocation}");
            }
            var dbPath = Path.Combine(dbLocation, "BlinkData.db");
            Log.LogInformation($"Using database {dbPath}");

            services.AddDbContext<BlinkDataContext>(o => o.UseSqlite($"Filename={dbPath}"));
            services.AddTransient<BlinkDataContext>();
            services.AddSingleton<IBlinkData>(svp =>
            {
                var scope = svp.CreateScope();
                return new BlinkAnalysis(scope.ServiceProvider.GetRequiredService<BlinkDataContext>());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(configure =>
            {
                configure.MapControllers();
                configure.MapRazorPages();
            });
        }
    }
}
