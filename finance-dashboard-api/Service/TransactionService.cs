using finance_dashboard_api.Repository;
using FinanceDashboardApi.DBContext;
using FinanceDashboardApi.Interface;
using FinanceDashboardApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboardApi.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        public TransactionService(ITransactionRepository repository) 
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync() 
        {
            return await _repository.GetAllTransactionsAsync();
        }

        public async Task<Transaction> GetTransactionAsync(int id)
        {
            var result = await _repository.GetTransactionByIdAsync(id);
            if (result == null) 
            {
                throw new Exception("Not Found"); //TODO: Make proper error handling
            }
            return result;
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            return await _repository.AddTransactionAsync(transaction);
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            var record = await _repository.GetTransactionByIdAsync(transaction.TransactionId);
            if (record == null)
            {
                throw new Exception("Not Found"); //TODO: Make proper error handling
            }

            record.Amount = transaction.Amount;
            record.Description = transaction.Description;
            record.Date = transaction.Date;
            record.Type = transaction.Type;
            record.Category = transaction.Category;

            await _repository.UpdateTransactionAsync(record);
            return record;
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            var record = await _repository.GetTransactionByIdAsync(transactionId);
            if (record == null)
            {
                throw new Exception("Not Found"); //TODO: Make proper error handling
            }

            await _repository.DeleteTransactionAsync(record);
        }
    }
}
