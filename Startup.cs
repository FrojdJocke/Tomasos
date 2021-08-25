using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using TomasosASP.Models;

namespace TomasosASP
{
    public class Startup
    {
        public IWebHostEnvironment Environment;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TomasosContext>(options =>
            {
                if (Environment.IsDevelopment())
                {
                    options.UseInMemoryDatabase(databaseName: "TomasosMockDB");
                }
                else
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                }
            });

            services.AddIdentity<User, IdentityRole>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 4;
                })
                .AddEntityFrameworkStores<TomasosContext>()
                .AddDefaultTokenProviders();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            services.AddDistributedMemoryCache();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //AddInMemoryData(app.ApplicationServices);
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
    
}





