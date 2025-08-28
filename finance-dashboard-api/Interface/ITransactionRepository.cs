using finance_dashboard_api.Models;
using FinanceDashboardApi.Models;

namespace finance_dashboard_api.Interface
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<List<Transaction>> AddTransactionAsync(Transaction transaction);
        Task<List<TransactionFilterResponse>> GetFilteredTransactions(TransactionFilterDto transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(Transaction transaction);
    }
}
