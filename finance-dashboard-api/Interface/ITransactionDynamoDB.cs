using finance_dashboard_api.Models;
using FinanceDashboardApi.Models;

namespace finance_dashboard_api.Interface
{
    public interface ITransactionDynamoDB
    {
        Task<Transaction?> GetTransactionByIdAsync(Guid transactionId);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<List<TransactionFilterResponse>> GetFilteredTransactions(TransactionFilterDto transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task<Transaction> DeleteTransactionAsync(Transaction transaction);
    }
}
