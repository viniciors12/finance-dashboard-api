using finance_dashboard_api.Interface;
using FinanceDashboardApi.Models;
using FinanceDashboardApi.Service;
using Moq;
using Xunit;

namespace finance_dashboard_api_test.TransactionServiceTests
{
    public class TransactionServiceTest
    {
        [Fact]
        public async Task GetTransactionById_ReturnsTransaction_WhenExists()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionDynamoDB>();
            var newGuid = Guid.NewGuid();
            var expected = new Transaction { TransactionId = newGuid, Amount = 100 };
            mockRepo.Setup(r => r.GetTransactionByIdAsync(newGuid)).ReturnsAsync(expected);

            var service = new TransactionService(mockRepo.Object);

            // Act
            var result = await service.GetTransactionAsync(newGuid);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }

}
