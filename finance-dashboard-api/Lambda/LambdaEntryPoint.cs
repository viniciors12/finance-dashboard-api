using Amazon.Lambda.AspNetCoreServer;
using FinanceDashboardApi;

    namespace finance_dashboard_api.Lambda
    {
        public class LambdaEntryPoint : APIGatewayHttpApiV2ProxyFunction
    {
            protected override void Init(IWebHostBuilder builder)
            {
                builder.UseStartup<Startup>();
            }
        }
    }
