using finance_dashboard_api.Models;
using FinanceDashboardApi.Models;

namespace FinanceDashboardApi.Interface
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction> GetTransactionAsync(int id);
        Task<List<Transaction>> AddTransactionAsync(Transaction transaction);
        Task<List<TransactionFilterResponse>> GetFilteredTransactions(TransactionFilterDto transactionFilter);
        Task<Transaction> UpdateTransactionAsync(Transaction transaction);
        Task<List<Transaction>> DeleteTransactionAsync(int transactionId);
    }
}
