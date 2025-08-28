using FinanceDashboardApi.Interface;
using FinanceDashboardApi.Service;
using FinanceDashboardApi.DBContext;
using Microsoft.EntityFrameworkCore;
using finance_dashboard_api.Repository;
using finance_dashboard_api.Interface;

namespace FinanceDashboardApi
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration configuration) 
        { 
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddDbContext<FinanceDBContext>(options =>
            options.UseSqlite("Data Source=finance.db"));

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseRouting();
            app.UseCors("AllowFrontend");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
