using FinanceDashboardApi.Constants;

namespace finance_dashboard_api.Models
{
    public class TransactionFilterDto
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string Category { get; set; }
    }
}
