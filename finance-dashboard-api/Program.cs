using Amazon;
using Amazon.DynamoDBv2;
using finance_dashboard_api.Interface;
using finance_dashboard_api.Repository;
using FinanceDashboardApi.DBContext;
using FinanceDashboardApi.Interface;
using FinanceDashboardApi.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var config = new AmazonDynamoDBConfig
    {
        RegionEndpoint = RegionEndpoint.USEast2
    };
    return new AmazonDynamoDBClient(config);
});

builder.Services.AddScoped<ITransactionDynamoDB, TransactionDynamoDB>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddDbContext<FinanceDBContext>(options =>
    options.UseSqlite("Data Source=finance.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configura el pipeline HTTP
app.UseCors("AllowFrontend");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();