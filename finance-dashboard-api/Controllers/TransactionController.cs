using Microsoft.AspNetCore.Mvc;
using FinanceDashboardApi.Interface;
using FinanceDashboardApi.Models;
using finance_dashboard_api.Models;

namespace sample_app_api.Controllers
{
    [ApiController]
    [Route("transactions/")]
    public class TransactionController : ControllerBase
    {
        public ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactions()
        {
            var results = await _transactionService.GetAllTransactionsAsync();
            return Ok(results);
        }

        [HttpGet("{transactionId}")]
        public async Task<ActionResult<Transaction>> GetTransactionById(int transactionId)
        {
            var transaction = await _transactionService.GetTransactionAsync(transactionId);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<List<Transaction>>> AddTransaction([FromBody] Transaction transaction)
        {
            var result = await _transactionService.AddTransactionAsync(transaction);
            if (transaction == null || result == null) 
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPost("filteredTransactions")]
        public async Task<ActionResult<List<TransactionFilterResponse>>> GetFilteredTransactions([FromBody] TransactionFilterDto filter)
        {
            var result = await _transactionService.GetFilteredTransactions(filter);
            if (filter == null || result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<Transaction>> UpdateTransaction([FromBody] Transaction transaction)
        {
            var updated = await _transactionService.UpdateTransactionAsync(transaction);
            if (updated == null)
                return NotFound($"Transaction with ID {transaction.TransactionId} not found");

            return Ok(updated);
        }

        [HttpDelete("{transactionId}")]
        public async Task DeleteTransaction(int transactionId)
        {
            await _transactionService.DeleteTransactionAsync(transactionId);
        }
    }
}