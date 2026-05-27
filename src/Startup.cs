using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TableCharm.Data;
using TableCharm.Services;

namespace TableCharm
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add DbContext
            services.AddDbContext<TableCharmDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Register Services
            services.AddScoped<IDistributorService, DistributorService>();
            services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<ICommissionService, CommissionService>();

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Add Controllers
            services.AddControllers();

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TableCharm API",
                    Version = "v1",
                    Description = "API for managing distributors, sales, and commissions in the direct selling network",
                    Contact = new OpenApiContact
                    {
                        Name = "TableCharm Team",
                        Url = new System.Uri("https://github.com/swmtechsolutions123-netizen/TableCharm")
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TableCharmDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TableCharm API v1"));

                // Apply migrations automatically in development
                dbContext.Database.Migrate();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
