using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using EducomOpdrachtAPI.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EducomOpdrachtAPI
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
            // Registratie van database context als service
            //services.AddDbContext<WeerstationContext>(opt => opt.UseInMemoryDatabase("Weerstations"));
            services.AddDbContext<WeerstationContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));
            //services.AddDbContext<WeerberichtContext>(opt => opt.UseInMemoryDatabase("Weerberichten"));
            services.AddDbContext<WeerberichtContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
