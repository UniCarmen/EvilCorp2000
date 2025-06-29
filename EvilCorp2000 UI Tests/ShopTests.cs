﻿using EvilCorp2000.Pages;
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
using EvilCorp2000.Pages.ProductManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace EvilCorp2000_UI_Tests
{
    public class ShopTests
    {
        private readonly Mock<IProductForSaleManager> _productForSaleManagerMock;
        private readonly Mock<ILogger<ShopViewModel>> _loggerMock;

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
                .Setup(m => m.GetProductsForSale(It.IsAny<GetProductsParameters>()))
                .ReturnsAsync(expectedReturnValues);

            var pageModel = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet(new UIGetProductsParameters());

            // ASSERT
            var returnedObject = new ProductListReturn<ProductForSaleDTO>
            {
                ProductList = pageModel.ProductsForSale,
                MaxPageCount = pageModel.MaxPageCount,
                ProductCount = pageModel.CountProducts
            };

            Assert.True(pageModel.ModelState.IsValid);

            // Verify the manager method was called once
            _productForSaleManagerMock.Verify(m => m.GetProductsForSale(It.IsAny<GetProductsParameters>()), Times.Once);

            //Mit Equivalent werden nur die Werte verglichen und nicht die Objekte, da diese verschiedene Referenzen haben können (wie in diesem Fall)
            Assert.Equivalent(expectedReturnValues, returnedObject);

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
        public async Task OnGet_ShouldAddModelErrorAndLogError_WhenExceptionThrown()
        {
            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale(new GetProductsParameters()))
                .ThrowsAsync(new Exception("Fehler beim Laden der Produkte."));

            _productForSaleManagerMock
                .Setup(m => m.GetCategories())
                .ThrowsAsync(new Exception("Something else went wrong"));

            var pageModel = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // ACT
            await pageModel.OnGet(new UIGetProductsParameters());

            // ASSERT
            Assert.False(pageModel.ModelState.IsValid);

            var errors = pageModel.ModelState[string.Empty].Errors;
            Assert.NotEmpty(errors);

            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => !string.IsNullOrWhiteSpace(v.ToString())), //Prüfung auf Text im Logeintrag
                    It.IsAny<Exception>(), //beliebige Exception
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()), //ZUsammensetzung der Nachricht - Standardmäßig verwendet
                Times.Once
            );
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 20)]
        public async Task OnGet_ShouldPassExpectedParametersAndMapResponse(int pageNumber, int pageSize)
        {
            // Arrange
            var expectedReturn = new ProductListReturn<ProductForSaleDTO>
            {
                ProductList = new List<ProductForSaleDTO>
                {
                    new ProductForSaleDTO { ProductName = $"Page {pageNumber}" }
                },
                MaxPageCount = 5,
                ProductCount = 42
            };

            var expectedParameters = new GetProductsParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = ProductSortOrder.PriceAsc,
                Search = "Test",
                CategoryId = Guid.NewGuid()
            };

            var uiParameters = new UIGetProductsParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrderString = "PriceAsc",
                Search = "Test",
                FilterCategoryString = expectedParameters.CategoryId.ToString()
            };

            _productForSaleManagerMock
                .Setup(m => m.GetProductsForSale(It.IsAny<GetProductsParameters>()))
                .ReturnsAsync(expectedReturn);

            _productForSaleManagerMock
                .Setup(m => m.GetCategories())
                .ReturnsAsync(new List<CategoryDTO>());

            var model = new ShopViewModel(_productForSaleManagerMock.Object, _loggerMock.Object);

            // Act
            await model.OnGet(uiParameters);

            // Assert
            _productForSaleManagerMock.Verify(m =>
                m.GetProductsForSale(It.Is<GetProductsParameters>(p =>
                    p.PageNumber == pageNumber &&
                    p.PageSize == pageSize &&
                    p.SortOrder == ProductSortOrder.PriceAsc &&
                    p.Search == "Test" &&
                    p.CategoryId != Guid.Empty // oder dein Test-GUID
                )), Times.Once);

            Assert.Equal(expectedReturn.ProductList, model.ProductsForSale);
            Assert.Equal(expectedReturn.MaxPageCount, model.MaxPageCount);
            Assert.Equal(expectedReturn.ProductCount, model.CountProducts);
        }

    }
}
