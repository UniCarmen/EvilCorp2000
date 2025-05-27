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
using Microsoft.Data.SqlClient;
using static Shared.Utilities.Utilities;

namespace EvilCorp2000_UI_Tests
{
    public class ShopTests
    {
        private readonly Mock<IProductForSaleManager> _productForSaleManagerMock;
        private readonly Mock<ILogger<ShopViewModel>> _loggerMock;

        private readonly ProductSortOrder _sortOrder = Shared.Utilities.Utilities.ProductSortOrder.Default;
        private readonly int _pageNumber = 1;
        private readonly int _pageSize = 10;

        public ShopTests()
        {
            _productForSaleManagerMock = new Mock<IProductForSaleManager>();
            _loggerMock = new Mock<ILogger<ShopViewModel>>();
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

            ProductListReturn<ProductForSaleDTO> expectedReturnValues = new ProductListReturn<ProductForSaleDTO>
            {
                ProductList = expectedProducts,
                MaxPageCount = 1,
                ProductCount = 2
            };


            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale(
                    It.IsAny<ProductSortOrder>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(expectedReturnValues);

            var pageModel = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet();

            // ASSERT
            var returnedObject = new ProductListReturn<ProductForSaleDTO>
            {
                ProductList = pageModel.ProductsForSale,
                MaxPageCount = pageModel.MaxPageCount,
                ProductCount = pageModel.CountProducts
            };

            //Mit Equivalent werden nur die Werte verglichen und nicht die Objekte, da diese verschiedene Referenzen haben können (wie in diesem Fall)
            Assert.Equivalent(expectedReturnValues, returnedObject);

            // No model error
            Assert.True(pageModel.ModelState.IsValid);

            // Verify the manager method was called once
            _productForSaleManagerMock.Verify(m => m.GetProductsForSale(_sortOrder, _pageNumber, _pageSize), Times.Once);

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
        public async Task OnGet_ShouldAddModelError_AndReturnPage_ExceptionThrown()
        {
            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale(_sortOrder, _pageNumber, _pageSize))
                .ThrowsAsync(new Exception("Fehler beim Laden der Produkte."));

            var pageModel = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object);

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
                .Setup(m => m.GetProductsForSale(_sortOrder, _pageNumber, _pageSize))
                .ThrowsAsync(new Exception("Something else went wrong"));

            var pageModel = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet();

            // ASSERT            
            Assert.False(pageModel.ModelState.IsValid);
            Assert.Contains("Fehler beim Laden der Produkte.", pageModel.ModelState[string.Empty]?.Errors[0].ErrorMessage);

            // Verify an error log was created:
            // Can't do logger.LogError("message", It.IsAny<Exception>()) because it's an extension method.
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

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 10)]
        [InlineData(3, 15)]
        public async Task OnGet_ShouldRespectPaginationParameters(int pageNumber, int pageSize)
        {
            // ARRANGE
            var expectedProducts = new List<ProductForSaleDTO>
                {
                    new ProductForSaleDTO { ProductName = $"Product page {pageNumber}" }
                };

            ProductListReturn<ProductForSaleDTO> expectedReturn = new ProductListReturn<ProductForSaleDTO>
            {
                ProductList = expectedProducts,
                MaxPageCount = 4,
                ProductCount = 40
            };

            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale(
                    It.IsAny<ProductSortOrder>(),
                    pageNumber,
                    pageSize))
                .ReturnsAsync(expectedReturn);

            var pageModel = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object)
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            // ACT
            await pageModel.OnGet("Default", pageNumber, pageSize);
            //Calls GetProductsForSale 

            // ASSERT
            Assert.Equal(expectedProducts, pageModel.ProductsForSale);
            Assert.Equal(expectedReturn.MaxPageCount, pageModel.MaxPageCount);
            Assert.Equal(expectedReturn.ProductCount, pageModel.CountProducts);

            _productForSaleManagerMock.Verify(m =>
                m.GetProductsForSale(It.IsAny<ProductSortOrder>(), pageNumber, pageSize), Times.Once);
        }

    }
}
