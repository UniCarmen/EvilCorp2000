using EvilCorp2000.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using BusinessLayer.Models;
using BusinessLayer.Services;

namespace EvilCorp2000_UI_Tests
{
    public class IndexModelTests
    {
        private readonly Mock<IProductForSaleManager> _productForSaleManagerMock;
        private readonly Mock<ILogger<IndexModel>> _loggerMock;

        public IndexModelTests()
        {
            _productForSaleManagerMock = new Mock<IProductForSaleManager>();
            _loggerMock = new Mock<ILogger<IndexModel>>();
        }

        [Fact]
        public async Task OnGet_ShouldSetProductsForSale_WhenNoException()
        {
            // ARRANGE
            var expectedProducts = new List<ProductForSaleDTO>
            {
                new ProductForSaleDTO { ProductName = "Test1" },
                new ProductForSaleDTO { ProductName = "Test2" }
            };

            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale())
                .ReturnsAsync(expectedProducts);

            var pageModel = new IndexModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet();

            // ASSERT
            // Verify the products were set
            Assert.Equal(expectedProducts, pageModel.ProductsForSale);

            // No model error
            Assert.True(pageModel.ModelState.IsValid);

            // Verify the manager method was called once
            _productForSaleManagerMock.Verify(m => m.GetProductsForSale(), Times.Once);

            // Verify no error log
            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never
            );
        }

        [Fact]
        public async Task OnGet_ShouldAddModelError_AndReturnPage_WhenDbUpdateExceptionThrown()
        {
            // ARRANGE
            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale())
                .ThrowsAsync(new DbUpdateException("DB update failure"));

            var pageModel = new IndexModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet();

            // ASSERT
            Assert.False(pageModel.ModelState.IsValid);
            
            var errors = pageModel.ModelState[string.Empty].Errors;
            Assert.NotEmpty(errors);

            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task OnGet_ShouldLogError_WhenGenericExceptionThrown()
        {
            // ARRANGE
            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale())
                .ThrowsAsync(new Exception("Something else went wrong"));

            var pageModel = new IndexModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet();

            // ASSERT            
            Assert.False(pageModel.ModelState.IsValid);
            Assert.Contains("Fehler beim Laden der Produkte.", pageModel.ModelState[string.Empty]?.Errors[0].ErrorMessage);

            // Verify an error log was created:
            // We can't do logger.LogError("message", It.IsAny<Exception>()) because it's an extension method.
            // So we check the underlying Log call at LogLevel.Error. 
            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fehler beim Laden der Produkte.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }
    }
}
