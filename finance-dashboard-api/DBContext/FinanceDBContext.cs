using FinanceDashboardApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboardApi.DBContext
{
    public class FinanceDBContext : DbContext
    {
        public FinanceDBContext(DbContextOptions<FinanceDBContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
