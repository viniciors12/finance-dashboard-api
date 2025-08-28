namespace finance_dashboard_api.Models
{
    public class TransactionFilterResponse
    {
        public string Month { get; set; }
        public long Income { get; set; }
        public long Expense { get; set; }
        public long Net { get; set; }
    }
}
