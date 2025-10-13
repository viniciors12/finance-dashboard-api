using Amazon;
using Amazon.DynamoDBv2;
using finance_dashboard_api.Interface;
using finance_dashboard_api.Middleware;
using finance_dashboard_api.Repository;
using finance_dashboard_api.Service;
using FinanceDashboardApi.Interface;
using FinanceDashboardApi.Service;
using Microsoft.IdentityModel.Tokens;

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
            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.USEast2
                };
                return new AmazonDynamoDBClient(config);
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ITransactionDynamoDB, TransactionDynamoDB>();
            services.AddScoped<ITransactionService, TransactionService>();
            // services.AddScoped<ITransactionRepository, TransactionRepository>(); sqlLite repo class

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            var userPoolId = _config["Cognito:UserPoolId"];
            var appClientId = _config["Cognito:AppClientId"];

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = $"https://cognito-idp.{RegionEndpoint.USEast2}.amazonaws.com/{userPoolId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = appClientId
                };
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseMiddleware<CognitoUserMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}