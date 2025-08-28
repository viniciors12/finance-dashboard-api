using finance_dashboard_api.Interface;
using finance_dashboard_api.Models;
using FinanceDashboardApi.Constants;
using FinanceDashboardApi.DBContext;
using FinanceDashboardApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace finance_dashboard_api.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly FinanceDBContext _context;
        public TransactionRepository(FinanceDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<List<Transaction>> AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            var transactions = await _context.Transactions.ToListAsync();
            return transactions;
        }

        public async Task<List<TransactionFilterResponse>> GetFilteredTransactions(TransactionFilterDto filter)
        {
            var transactions = _context.Transactions.Where(t => t.Date >= filter.FromDate && t.Date <= filter.ToDate);

            var incomes = transactions.Where(t => t.Type == TransactionType.Income);
            var expenses = transactions.Where(t => t.Type == TransactionType.Expense);

            if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category != "All")
            {
                expenses = expenses.Where(t => t.Category == filter.Category);
            }

            var grouped = incomes.Concat(expenses)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new TransactionFilterResponse
                {
                    Month = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                    Net = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount)
                         - g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                });

            return grouped.ToList();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
