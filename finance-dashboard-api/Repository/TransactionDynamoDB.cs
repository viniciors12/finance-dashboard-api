using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using finance_dashboard_api.Interface;
using finance_dashboard_api.Models;
using FinanceDashboardApi.Constants;
using FinanceDashboardApi.Models;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace finance_dashboard_api.Repository
{
    public class TransactionDynamoDB : ITransactionDynamoDB
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private const string TableName = "transactions";

        public TransactionDynamoDB(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                ["TransactionId"] = new AttributeValue { S = Guid.NewGuid().ToString() },
                ["Type"] = new AttributeValue { S = transaction.Type.ToString() },
                ["Category"] = new AttributeValue { S = transaction.Category },
                ["Description"] = new AttributeValue { S = transaction.Description },
                ["Amount"] = new AttributeValue { N = transaction.Amount.ToString() },
                ["Date"] = new AttributeValue { S = transaction.Date.ToString("o") }
            };

            await _dynamoDb.PutItemAsync(new PutItemRequest
            {
                TableName = "transactions",
                Item = item
            });

            return transaction;

        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            var key = new Dictionary<string, AttributeValue>
            {
                ["TransactionId"] = new AttributeValue { S = transaction.TransactionId.ToString() },
            };

            var request = new UpdateItemRequest
            {
                TableName = "transactions",
                Key = key,
                UpdateExpression = "SET #date = :date, #type = :type, #category = :category, #description = :description, #amount = :amount",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    ["#date"] = "Date",
                    ["#type"] = "Type",
                    ["#category"] = "Category",
                    ["#description"] = "Description",
                    ["#amount"] = "Amount"
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":date"] = new AttributeValue { S = transaction.Date.ToString() },
                    [":type"] = new AttributeValue { S = transaction.Type.ToString() },
                    [":category"] = new AttributeValue { S = transaction.Category },
                    [":description"] = new AttributeValue { S = transaction.Description },
                    [":amount"] = new AttributeValue { N = transaction.Amount.ToString() }
                }
            };

            await _dynamoDb.UpdateItemAsync(request);
        }

        public async Task<Transaction> DeleteTransactionAsync(Transaction transaction)
        {
            var key = new Dictionary<string, AttributeValue>
            {
                ["TransactionId"] = new AttributeValue { S = transaction.TransactionId.ToString() },
            };

            var request = new DeleteItemRequest
            {
                TableName = "transactions",
                Key = key
            };

           await _dynamoDb.DeleteItemAsync(request);

           return transaction;

        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            var response = await _dynamoDb.ScanAsync(request);

            var transactions = response.Items.Select(item => new Transaction
            {
                TransactionId = Guid.Parse(item["TransactionId"].S),
                Type = Enum.Parse<TransactionType>(item["Type"].S),
                Category = item["Category"].S,
                Description = item["Description"].S,
                Amount = long.Parse(item["Amount"].N),
                Date = DateTime.Parse(item["Date"].S)
            });

            return transactions;

        }

        public async Task<List<TransactionFilterResponse>> GetFilteredTransactions(TransactionFilterDto filter)
        {
            var response = await GetTransactionsByDates(filter);
            var transactions = response.Items.Select(item => new Transaction
            {
                TransactionId = Guid.Parse(item["TransactionId"].S),
                Type = Enum.Parse<TransactionType>(item["Type"].S),
                Category = item["Category"].S,
                Description = item["Description"].S,
                Amount = long.Parse(item["Amount"].N),
                Date = DateTime.Parse(item["Date"].S)
            });

            var incomes = transactions.Where(t => t.Type == TransactionType.Income);
            var expenses = transactions.Where(t => t.Type == TransactionType.Expense);

            if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category != "All")
            {
                expenses = expenses.Where(t => t.Category == filter.Category);
            }

            return incomes.Concat(expenses)
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => { 
                    var income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                    var expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                    var net = income - expense >= 0 ? income - expense : 0;
                    return new TransactionFilterResponse
                    {
                        Month = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                        Income = income,
                        Expense = expense,
                        Net = net
                    }; 
            })
            .ToList();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
        {
            try
            {
                var key = new Dictionary<string, AttributeValue>
                {
                    ["TransactionId"] = new AttributeValue { S = transactionId.ToString() },

                };

                var request = new GetItemRequest
                {
                    TableName = "transactions",
                    Key = key
                };

                var response = await _dynamoDb.GetItemAsync(request);

                if (response.Item == null || response.Item.Count == 0)
                {
                    return null;
                }

                var item = response.Item;

                return new Transaction
                {
                    TransactionId = Guid.Parse(item["TransactionId"].S),
                    Type = Enum.Parse<TransactionType>(item["Type"].S),
                    Category = item["Category"].S,
                    Description = item["Description"].S,
                    Amount = long.Parse(item["Amount"].N),
                    Date = DateTime.Parse(item["Date"].S)
                };
            }
            catch (AmazonDynamoDBException ex)
            {
                //_logger.LogError(ex, "Error en DynamoDB: {Message}", ex.Message);
                throw new ApplicationException("Error accessing DynamoDB", ex);
            }

        }

        private async Task<ScanResponse> GetTransactionsByDates(TransactionFilterDto transaction)
        {
            var request = new ScanRequest
            {
                TableName = "transactions",
                FilterExpression = "#date BETWEEN :from AND :to",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    ["#date"] = "Date"
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":from"] = new AttributeValue { S = transaction.FromDate?.ToString("o") },
                    [":to"] = new AttributeValue { S = transaction.ToDate?.ToString("o") }
                }
            };

            return await _dynamoDb.ScanAsync(request);

        }
    }
}
