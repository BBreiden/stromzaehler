using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddDbContext<BlinkDataContext>(options => options.UseInMemoryDatabase());
            var dbLocation = Configuration.GetSection("Database:Path").Value;
            if (string.IsNullOrEmpty(dbLocation)) {
                throw new InvalidOperationException("Missing database path.");
            }
            if (!Directory.Exists(dbLocation)) {
                throw new InvalidOperationException($"Directory does not exist: {dbLocation}");
            }
            var dbPath = Path.Combine(dbLocation, "BlinkData.db");
            Log.LogInformation($"Using database {dbPath}");

            services.AddDbContext<BlinkDataContext>(o => o.UseSqlite($"Filename={dbPath}"));

            services.AddTransient<IBlinkData, BlinkDataContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            
            app.UseMvc();
        }
    }
}
