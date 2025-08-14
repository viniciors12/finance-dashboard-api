using FinanceDashboardApi.Interface;
using FinanceDashboardApi.Service;
using FinanceDashboardApi.DBContext;
using Microsoft.EntityFrameworkCore;
using finance_dashboard_api.Repository;

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
