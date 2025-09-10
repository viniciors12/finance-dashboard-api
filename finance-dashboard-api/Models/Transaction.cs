using FinanceDashboardApi.Constants;

namespace FinanceDashboardApi.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        public TransactionType Type { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public long Amount { get; set; }

        public DateTime Date { get; set; }
    }
}