using Amazon.Lambda.AspNetCoreServer;
using FinanceDashboardApi;

    namespace finance_dashboard_api.Lambda
    {
        public class LambdaEntryPoint : APIGatewayProxyFunction
        {
            protected override void Init(IWebHostBuilder builder)
            {
                builder
                    .UseStartup<Startup>();
            }
        }
    }
