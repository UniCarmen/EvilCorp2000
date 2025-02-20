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
            // Because OnGet() does not explicitly "return" anything, 
            // we can only indirectly test the outcome by checking ModelState.
            await pageModel.OnGet();

            // ASSERT
            // The catch block calls ExecuteOnDBExceptionCatch which adds a model error 
            // and returns Page(). However, OnGet() has no explicit return type, 
            // so we can't do: var result = ...
            // Instead, we verify that a model error is present.
            Assert.False(pageModel.ModelState.IsValid);

            // Because the key is string.Empty in AddModelError(string.Empty, "Fehler in der Datenbank")
            Assert.True(pageModel.ModelState.ContainsKey(string.Empty));

            var errorMessage = pageModel.ModelState[string.Empty].Errors[0].ErrorMessage;
            Assert.Equal("Fehler in der Datenbank", errorMessage);

            // Check that no error log was called for this scenario
            // because the code doesn't log an error in the DbUpdateException branch
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
            // The generic exception catch block logs an error, 
            // but does not add a model error or return anything.
            // So we check that the model state remains valid.
            Assert.True(pageModel.ModelState.IsValid);

            // Verify an error log was created:
            // We can't do logger.LogError("message", It.IsAny<Exception>()) because it's an extension method.
            // So we check the underlying Log call at LogLevel.Error. 
            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error getting the products from the database")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }
    }
}
