using finance_dashboard_api.Interface;
using FinanceDashboardApi.Models;
using FinanceDashboardApi.Service;
using Moq;

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
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetTransactionById_ThrowsNotFoundException_WhenNotExists()
        {
            var mockRepo = new Mock<ITransactionDynamoDB>();
            var newGuid = Guid.NewGuid();

            mockRepo.Setup(r => r.GetTransactionByIdAsync(newGuid)).ReturnsAsync((Transaction)null);

            var service = new TransactionService(mockRepo.Object);

            await Assert.ThrowsAsync<Exception>(() => service.GetTransactionAsync(newGuid));
        }

        [Fact]
        public async Task AddTransactionAsync_ReturnsCreatedTransaction()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionDynamoDB>();
            var input = new Transaction { Amount = 100 };
            var expected = new Transaction { TransactionId = Guid.NewGuid(), Amount = 100 };

            mockRepo.Setup(r => r.AddTransactionAsync(It.IsAny<Transaction>())).ReturnsAsync(expected);

            var service = new TransactionService(mockRepo.Object);

            // Act
            var result = await service.AddTransactionAsync(input);

            // Assert
            Assert.Equal(expected, result);

            mockRepo.Verify(r => r.AddTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ReturnsAllTransactions()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionDynamoDB>();
            var expected = new List<Transaction>
            {
                new Transaction { TransactionId = Guid.NewGuid(), Amount = 100 },
                new Transaction { TransactionId = Guid.NewGuid(), Amount = 200 }
            };

            mockRepo
                .Setup(r => r.GetAllTransactionsAsync())
                .ReturnsAsync(expected);

            var service = new TransactionService(mockRepo.Object);

            // Act
            var result = await service.GetAllTransactionsAsync();

            // Assert
            Assert.Equal(expected, result);
            mockRepo.Verify(r => r.GetAllTransactionsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ReturnsFiltered()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionDynamoDB>();
            var expected = new List<Transaction>
            {
                new Transaction { TransactionId = Guid.NewGuid(), Amount = 100 },
                new Transaction { TransactionId = Guid.NewGuid(), Amount = 200 }
            };

            mockRepo
                .Setup(r => r.GetAllTransactionsAsync())
                .ReturnsAsync(expected);

            var service = new TransactionService(mockRepo.Object);

            // Act
            var result = await service.GetAllTransactionsAsync();

            // Assert
            Assert.Equal(expected, result);
            mockRepo.Verify(r => r.GetAllTransactionsAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTransactionAsync_ReturnsDeletedTransaction_WhenExists()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var existing = new Transaction { TransactionId = transactionId, Amount = 100 };

            var mockRepo = new Mock<ITransactionDynamoDB>();
            mockRepo.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(existing);
            mockRepo.Setup(r => r.DeleteTransactionAsync(existing)).ReturnsAsync(existing);

            var service = new TransactionService(mockRepo.Object);

            // Act
            var result = await service.DeleteTransactionAsync(transactionId);

            // Assert
            Assert.Equal(existing, result);
            mockRepo.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Never);
            mockRepo.Verify(r => r.DeleteTransactionAsync(existing), Times.Once);
        }

        [Fact]
        public async Task DeleteTransactionAsync_ThrowsException_WhenNotFound()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            var mockRepo = new Mock<ITransactionDynamoDB>();
            mockRepo.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync((Transaction)null);

            var service = new TransactionService(mockRepo.Object);

            // Act & Assert
            var act = async () => await service.DeleteTransactionAsync(transactionId);
            var ex = await Assert.ThrowsAsync<Exception>(() => service.GetTransactionAsync(transactionId));
            Assert.Equal("Not Found", ex.Message);

            mockRepo.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Once);
            mockRepo.Verify(r => r.DeleteTransactionAsync(It.IsAny<Transaction>()), Times.Never);
        }

    }

}
