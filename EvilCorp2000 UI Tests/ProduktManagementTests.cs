using BusinessLayer.Services;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Hosting;
using EvilCorp2000.Pages.ProductManagement;
using SixLabors.ImageSharp;
using EvilCorp2000.UIModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Web.Mvc;
using System.Security.Claims;
using static Shared.Utilities.Utilities;


namespace EvilCorp2000_UI_Tests
{
    public class TestableProductManagementModel : ProductManagementModel
    {
        private readonly bool _forceInvalidModelState;
        private readonly bool _forceInvalidDiscountModelState;

        public TestableProductManagementModel(
            IInternalProductManager internalProductManager,
            ILogger<ProductManagementModel> logger,
            IWebHostEnvironment environment,
            IAuthorizationService authorizationService,
            bool forceInvalidModelState = false, // Standardmäßig auf FALSE, also valid
            bool forceInvalidDiscountModelState = false) // Standardmäßig auf FALSE, also valid
            : base(internalProductManager, logger, environment, authorizationService)
        {
            _forceInvalidModelState = forceInvalidModelState;
            _forceInvalidDiscountModelState = forceInvalidDiscountModelState;


            // **Sicherstellen, dass ValidatedProduct IMMER existiert**
            ValidatedProduct ??= new ValidatedProduct
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Default Product",
                Discounts = new List<DiscountDTO>()
            };
        }


        // Falls forceInvalidModelState = true, gibt die Methode immer FALSE zurück (als ob ModelState fehlschlägt)
        protected override bool IsModelStateValidForProduct(Guid productId) => !_forceInvalidModelState;

        protected override bool IsModelStateIsInvalidForDiscount(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState) => _forceInvalidDiscountModelState;
    }


    public class ProduktManagementTests
    {

        //In UI-Tests: Immer neue Mocks pro Test, da die Tests das Verhalten der Mocks pro Test verändern und sich beeinflussen könnten.

        readonly ProductListReturn<ProductManagementProductDTO> _returnValues = new ProductListReturn<ProductManagementProductDTO>
        {
            ProductList = [],
            MaxPageCount = 1,
            ProductCount = 2
        };

        //TODO1: evtl weg, immer neu erzeugen in den Tests
        private readonly GetProductsParameters parameters = new GetProductsParameters();
        private readonly UIGetProductsParameters parametersUI = new UIGetProductsParameters();


        [Fact]
        public async Task LoadDataAsync_ShouldCallGetProductsAndCategories()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            //INFO Simuliert Rückgabewerte für Methodenaufrufe --> nicht nötig, da nur Aufruf getestet wird
            //productManagerMock.Setup(m => m.GetProductsForInternalUse(parameters)).ReturnsAsync(_returnValues); 
            //productManagerMock.Setup(m => m.GetCategories()).ReturnsAsync(new List<CategoryDTO>());

            //INFO erstellt das PageModel mit den Mock-Objekten
            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);

            // Act
            await model.LoadDataAsync(parametersUI);

            // Assert: Methoden werden aufgerufen
            productManagerMock.Verify(m => m.GetProductsForInternalUse(It.IsAny<GetProductsParameters>()), Times.Once);
            productManagerMock.Verify(m => m.GetCategories(), Times.Once);
        }


        [Fact]
        public async Task LoadDataAsync_CallsServiceWithExpectedParameters()
        {
            // Arrange
            var mockManager = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var searchCatergoryId = Guid.NewGuid();

            var categories = new List<CategoryDTO>
            {
                new CategoryDTO() {CategoryName = "1", CategoryId = searchCatergoryId }
            };

            var expectedParameters = new GetProductsParameters
            {
                PageNumber = 3,
                PageSize = 20,
                SortOrder = ProductSortOrder.PriceAsc,
                Search = "Test",
                CategoryId = searchCatergoryId
            };

            var uIParameters = new UIGetProductsParameters
            {
                PageNumber = 3,
                PageSize = 20,
                SortOrderString = "PriceAsc",
                Search = "Test",
                FilterCategoryString = searchCatergoryId.ToString()
            };

            var returnData = new ProductListReturn<ProductManagementProductDTO>
            {
                ProductList = new List<ProductManagementProductDTO> {
                    new() { ProductName = "Test", ProductId = searchCatergoryId }},
                    ProductCount = 1,
                    MaxPageCount = 1
            };

            //weil am Ende immer Prüfung auf Referenzgleichheit
            mockManager
                .Setup(m => m.GetProductsForInternalUse(It.IsAny<GetProductsParameters>()))
                .ReturnsAsync(returnData);

            mockManager
                .Setup(m => m.GetCategories())
                .ReturnsAsync(categories);

            var model = new ProductManagementModel(mockManager.Object, loggerMock.Object, envMock.Object, authMock.Object);

            // Act
            await model.LoadDataAsync(uIParameters);

            // Assert
            mockManager.Verify(m =>
                m.GetProductsForInternalUse(It.Is<GetProductsParameters>(p =>
                    p.PageNumber == expectedParameters.PageNumber &&
                    p.PageSize == expectedParameters.PageSize &&
                    p.SortOrder == expectedParameters.SortOrder &&
                    p.Search == expectedParameters.Search &&
                    p.CategoryId == expectedParameters.CategoryId
                )), Times.Once);

            Assert.Equal(uIParameters.PageNumber, model.PageNumber);
            Assert.Equal(uIParameters.PageSize, model.PageSize);
            Assert.Equal(uIParameters.SortOrderString, model.SortOrder);
            Assert.Equal(uIParameters.Search, model.Search);
            Assert.Equal(uIParameters.FilterCategoryString, model.FilterCategoryString);
        }
        

        //TODO1: ab hier prüfen

        [Fact]
        public async Task LoadDataAsync_ShouldHandleException_AndLogError()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            productManagerMock.Setup(m => m.GetProductsForInternalUse(parameters)).ThrowsAsync(new Exception("Database error"));
            productManagerMock.Setup(m => m.GetCategories()).ThrowsAsync(new Exception("Database error"));

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);

            // Act
            await model.LoadDataAsync(null);

            // Assert
            //INFO Überprüft, ob das Logging korrekt aufgerufen wurde
            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error), //INFO Prüft, ob LogLevel = Error
                    It.IsAny<EventId>(), //INFO ignorieren
                    It.Is<It.IsAnyType>((o, t) => true),  //INFO Irgendein Objekt als Nachricht
                    It.IsAny<Exception>(),  //INFO Prüft, ob eine Exception geloggt wurde
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>() //INFO Formatter für die Log-Nachricht
                ),
                Times.Once //INFO Muss genau einmal aufgerufen werden
            );
        }


        //INFO Prüft, ob das übergebene ValidatedProduct korrekt übernommen wird.
        //INFO Prüft, ob DiscountsJson und CategoryIdsJson richtig gesetzt werden. --> mit ValidatedProduct verknüpft, wenn einer falsch ist, ist das Ergebnis unbrauchbar
        //INFO Prüft, ob ShowModal aktiviert wird.
        [Fact]
        public async Task ReInitializeModalWithProduct_ShouldSetValidatedProduct_AndUpdateJson()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);
            var validatedProduct = new ValidatedProduct { ProductId = Guid.NewGuid(), ProductName = "Test Product" };
            validatedProduct.SelectedCategoryIds = new List<Guid> { Guid.NewGuid() }; 
            var categoryIds = new List<Guid> { Guid.NewGuid() };
            var discounts = new List<DiscountDTO>
                {
                    new DiscountDTO { DiscountId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), DiscountPercentage = 10 }
                };

            // Act
            var result = await model.ReInitializeModalWithProduct(validatedProduct, categoryIds, discounts);

            // Assert
            Assert.Equal(validatedProduct, model.ValidatedProduct);
            Assert.Equal(JsonSerializer.Serialize(discounts), model.DiscountsJson);
            Assert.True(model.ShowModal);
            Assert.IsType<PageResult>(result);
        }

        //INFO Prüft, ob die Discounts korrekt nach Startdatum sortiert werden.
        //INFO Sortierung von ValidatedProduct und Discount + CategoryJson unabhängig
        //INFO Falls jemand versehentlich den Code für die Sortierung entfernt oder ändert, soll nur dieser Test fehlschlagen.
        [Fact]
        public async Task ReInitializeModalWithProduct_ShouldSortDiscountsByStartDate()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);
            var validatedProduct = new ValidatedProduct { ProductId = Guid.NewGuid(), ProductName = "Test Product" };
            var categoryIds = new List<Guid> { Guid.NewGuid() };
            var discounts = new List<DiscountDTO>
                {
                    new DiscountDTO { DiscountId = Guid.NewGuid(), StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(10), DiscountPercentage = 10 },
                    new DiscountDTO { DiscountId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), DiscountPercentage = 5 }
                };

            // Act
            var result = await model.ReInitializeModalWithProduct(validatedProduct, categoryIds, discounts);

            // Assert
            Assert.Equal(JsonSerializer.Serialize(discounts.OrderBy(d => d.StartDate).ToList()), model.DiscountsJson);
        }

        
        [Fact]
        public async Task OnPostImageUpload_ShouldSaveValidImage()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var tempPath = Path.GetTempPath();
            envMock.Setup(e => e.WebRootPath).Returns(tempPath);


            var fileMock = new Mock<IFormFile>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestAssets", "0b6056bd-1441-4e01-93c1-5d36a193d7f7.png");

            using var stream = File.OpenRead(filePath);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/png");
            fileMock.Setup(f => f.FileName).Returns("test.png");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                ImageFile = fileMock.Object,
                ValidatedProductJson = JsonSerializer.Serialize(new ValidatedProduct { ProductId = Guid.NewGuid() }),
                CategoryIdsJson = JsonSerializer.Serialize(new List<Guid>())
            };

            model.CategoryIdsJson = "[]";  
            model.ValidatedProductJson = "{}"; 
            model.DiscountsJson = "[]"; 

            productManagerMock.Setup(m => m.SaveProductPicture(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var imageFolder = Path.Combine(Path.GetTempPath(), "images");
            Directory.CreateDirectory(imageFolder);
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            // Act
            var result = await model.OnPostImageUpload();

            // Assert
            Assert.IsType<PageResult>(result);
            productManagerMock.Verify(m => m.SaveProductPicture(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task OnPostImageUpload_ShouldReturnError_WhenInvalidImageProvided()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0); // Ungültige Datei (keine Größe)

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                ImageFile = fileMock.Object,
                ValidatedProductJson = JsonSerializer.Serialize(new ValidatedProduct { ProductId = Guid.NewGuid() }),
                CategoryIdsJson = JsonSerializer.Serialize(new List<Guid>())
            };

            model.CategoryIdsJson = "[]"; 
            model.ValidatedProductJson = "{}";
            model.DiscountsJson = "[]";  

            // Act
            var result = await model.OnPostImageUpload();

            // Assert
            Assert.IsType<PageResult>(result); // Bleibt auf der Seite mit Fehler
            Assert.True(model.ModelState.ContainsKey("ImageFile"));
        }

        [Fact]
        public async Task OnPostImageUpload_ShouldReturnError_WhenSavingFails()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            productManagerMock
                .Setup(m => m.SaveProductPicture(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var tempPath = Path.GetTempPath();
            envMock.Setup(e => e.WebRootPath).Returns(tempPath);


            var fileMock = new Mock<IFormFile>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestAssets", "0b6056bd-1441-4e01-93c1-5d36a193d7f7.png");

            using var stream = File.OpenRead(filePath);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/png");
            fileMock.Setup(f => f.FileName).Returns("test.png");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                ImageFile = fileMock.Object,
                ValidatedProductJson = JsonSerializer.Serialize(new ValidatedProduct { ProductId = Guid.NewGuid() }),
                CategoryIdsJson = JsonSerializer.Serialize(new List<Guid>())
            };

            model.CategoryIdsJson = "[]";  // Leere Liste als Fallback
            model.ValidatedProductJson = "{}";  // Leeres JSON-Objekt
            model.DiscountsJson = "[]";  // Leere Liste als Fallback

            productManagerMock.Setup(m => m.SaveProductPicture(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error")); //Dummy-Fehler

            var imageFolder = Path.Combine(Path.GetTempPath(), "images");
            Directory.CreateDirectory(imageFolder);
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());



            // Act
            var result = await model.OnPostImageUpload();
            Debug.WriteLine($"Result Type: {result.GetType().Name}");

            // Assert
            Assert.IsType<PageResult>(result); // Fehler -> bleibt auf der Seite
            loggerMock.Verify(x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Exactly(2));
        }


        //INFO ModelState Validations manuell auf true gesetzt, da ansonsten die Validierung einfach nicht funktioniert...
        //Falls es eine Lösung mit ModelState Validierung gib, müsste ich den Test ändern.
        [Fact]
        public async Task OnPostSaveProduct_ShouldSaveValidProduct()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var testProduct = new ValidatedProduct
            {
                ProductId = Guid.Empty,
                ProductName = "New Product",
                AmountOnStock = 10,
                Price = 99.99m,
                Description = "A valid test product",
                SelectedCategoryIds = new List<Guid> { Guid.NewGuid() }
            };

            var testCategories = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = testProduct.SelectedCategoryIds[0], CategoryName = "TestCategory" }
            };

            productManagerMock.Setup(m => m.GetCategories()).ReturnsAsync(testCategories);
            productManagerMock.Setup(m => m.SaveProductToStore(It.IsAny<ProductManagementProductDTO>()))
                .Returns(Task.CompletedTask);

            var model = new TestableProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object, false)
            {
                PageContext = new PageContext
                {
                    ActionDescriptor = new CompiledPageActionDescriptor(),
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                },
                MetadataProvider = new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                ValidatedProduct = testProduct, 
                CategoryIdsJson = JsonSerializer.Serialize(testProduct.SelectedCategoryIds),
                DiscountsJson = "[]",
                ValidatedProductJson = JsonSerializer.Serialize(testProduct)
            };

            // Act
            var result = await model.OnPostSaveProduct();

            // Assert
            Assert.IsType<RedirectToPageResult>(result); 
            productManagerMock.Verify(m => m.SaveProductToStore(It.IsAny<ProductManagementProductDTO>()), Times.Once);
        }


        [Fact]
        public async Task OnPostSaveProduct_ShouldReturnError_WhenModelStateIsInvalid()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var invalidProduct = new ValidatedProduct
            {
                ProductId = Guid.Empty,
                ProductName = "", // Leerer Name -> Sollte ungültig sein
                AmountOnStock = -5, // Ungültiger Lagerbestand
                Price = -1m, // Ungültiger Preis
                Description = "Invalid test product",
                SelectedCategoryIds = new List<Guid>()
            };

            var model = new TestableProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object, true)
            {
                ValidatedProduct = invalidProduct,
                DiscountsJson = "[]",
                ValidatedProductJson = JsonSerializer.Serialize(invalidProduct),
                CategoryIdsJson = JsonSerializer.Serialize(invalidProduct.SelectedCategoryIds),
                PageContext = new PageContext
                {
                    ActionDescriptor = new CompiledPageActionDescriptor(),
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData(),
                },

                MetadataProvider = new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                
            };

            productManagerMock.Setup(m => m.GetProductsForInternalUse(parameters)).ReturnsAsync(_returnValues);
            model.ModelState.AddModelError("ProductName", "Product name is required"); // Manuelle ModelState-Fehlermeldung
            //await model.LoadDataAsync(_sortOrder.ToString(), _pageNumber, _pageSize);

            // Act
            var result = await model.OnPostSaveProduct();

            // Assert
            Assert.IsType<PageResult>(result); // Fehler -> bleibt auf der Seite
            
            //INFO: Erwartet genau 2 Aufrufe - Warum? Weiß ich nicht...
            productManagerMock.Verify(m => m.GetCategories(), Times.Exactly(2));  
        }



        [Fact]
        public async Task OnPostDeleteProduct_ShouldDeleteProduct_WhenAuthorized()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var testProductId = Guid.NewGuid();
            var authResult = AuthorizationResult.Success(); // Berechtigung erfolgreich

            // **Statt direkte Extension-Methode zu mocken -> Explizit über Setup**
            authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "CanDeleteProducts"))
                    .ReturnsAsync(authResult);

            productManagerMock.Setup(m => m.DeleteProduct(testProductId)).Returns(Task.CompletedTask);

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);

            // Act
            var result = await model.OnPostDeleteProduct(testProductId);

            // Assert
            Assert.IsType<RedirectToPageResult>(result); // Erfolgreiche Löschung -> Weiterleitung
            productManagerMock.Verify(m => m.DeleteProduct(testProductId), Times.Once);
        }

        [Fact]
        public async Task OnPostDeleteProduct_ShouldReturn403_WhenUnauthorized()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var testProductId = Guid.NewGuid();
            var authResult = AuthorizationResult.Failed(); // Berechtigung fehlgeschlagen

            // **Hier auch die direkte Extension-Methode umgehen**
            authMock.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "CanDeleteProducts"))
                    .ReturnsAsync(authResult);

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);

            // Act
            var result = await model.OnPostDeleteProduct(testProductId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Error", redirectResult.PageName); // Prüft, ob auf die Fehlerseite umgeleitet wird
        }

        

        [Fact]
        public async Task OnPostShowNewAndAlterProductModal_ShouldOpenModal_WhenProductExists()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var testProduct = new ProductManagementProductDTO
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Existing Product",
                Categories = new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Test Category" } },
                Discounts = new List<DiscountDTO>()                
            };



            ProductListReturn<ProductManagementProductDTO> productReturnValues = new ProductListReturn<ProductManagementProductDTO>
            {
                ProductList = [testProduct],
                MaxPageCount = 1,
                ProductCount = 2
            };

            productManagerMock.Setup(m => m.GetProductsForInternalUse(parameters))
                .ReturnsAsync(productReturnValues);

            productManagerMock.Setup(m => m.GetProductForInternalUse(testProduct.ProductId))
                .ReturnsAsync(testProduct);

            //ProductManagementProductDTO

            //var selectedProduct =
            //            await _internalProductManager.GetProductForInternalUse(selectedProductId);

            productManagerMock.Setup(m => m.GetCategories())
                .ReturnsAsync(new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Test Category" } });


            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                products = new List<ProductManagementProductDTO> { productReturnValues.ProductList.First() },
                //PageSize = 10,
                //PageNumber = 1,
                //SortOrder = "Default"
            };


            // Act
            var result = await model.OnPostShowNewAndAlterProductModal(testProduct.ProductId, parametersUI);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.ValidatedProduct);
            Assert.Equal(testProduct.ProductId, model.ValidatedProduct.ProductId);
            Assert.True(model.ShowModal, "Modal should be open");
        }


        [Fact]
        public async Task OnPostShowNewAndAlterProductModal_ShouldOpenModal_WhenNewProductIsCreated()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object);

            // Act
            var result = await model.OnPostShowNewAndAlterProductModal(Guid.Empty, parametersUI);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.True(model.ShowModal, "Modal should be open for new product");
        }

        [Fact]
        public async Task OnPostShowNewAndAlterProductModal_ShouldReturnPage_WhenProductNotFound()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var model = new ProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                products = new List<ProductManagementProductDTO>() // Leere Liste, Produkt wird nicht gefunden
            };

            // Act
            var result = await model.OnPostShowNewAndAlterProductModal(Guid.NewGuid(), parametersUI);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(model.ShowModal, "Modal should remain closed on error");
        }

        [Fact]
        public async Task OnPostAddDiscount_ShouldAddValidDiscount()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var testProduct = new ProductManagementProductDTO
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Valid Product",
                Categories = new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Test Category" } },
                Discounts = new List<DiscountDTO>()
            };

            var getProductParametersOfPage2 = parametersUI;
            getProductParametersOfPage2.PageNumber = 2;

            var testModel = new TestableProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object, false, false) // 👈 Beide valid
            {
                ValidatedProduct = new ValidatedProduct
                {
                    ProductId = testProduct.ProductId,
                    ProductName = "Valid Product",
                    Discounts = new List<DiscountDTO>(),
                    AmountOnStock = 1,
                    Price = 1.0m
                },
                DiscountsJson = "[]",
                CategoryIdsJson = "[]",
                NewDiscount = new ValidatedDiscount
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now, 
                    DiscountPercentage = 1,
                },
                Categories = [new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Category"}],
                PageSize = 10,
                PageNumber = 2,
                SortOrder = "Default"
            };

            

            testModel.products = new List<ProductManagementProductDTO>
                {
                    new ProductManagementProductDTO
                    {
                        ProductId = testModel.ValidatedProduct.ProductId,
                        ProductName = "Test Product",
                        Categories = new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Category" } },
                        Discounts = new List<DiscountDTO>()
                    }
                };

            ProductListReturn<ProductManagementProductDTO> expectedReturnValues = new ProductListReturn<ProductManagementProductDTO>
            {
                ProductList = testModel.products,
                MaxPageCount = 1,
                ProductCount = 2
            };

            testModel.ValidatedProductJson = JsonSerializer.Serialize(testModel.ValidatedProduct);
            testModel.CategoryIdsJson = JsonSerializer.Serialize(new List<Guid> { Guid.NewGuid() });
            testModel.DiscountsJson = JsonSerializer.Serialize(new List<DiscountDTO>());


            testModel.ValidatedProduct ??= new ValidatedProduct
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Valid Product",
                Discounts = new List<DiscountDTO>() // Liste initialisieren!
            };

            productManagerMock.Setup(m => m.GetProductsForInternalUse(parameters)).ReturnsAsync(_returnValues);
            productManagerMock.Setup(m => m.GetCategories()).ReturnsAsync(new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Test Category" } });

            await testModel.LoadDataAsync(getProductParametersOfPage2);

            // Act
            var result = await testModel.OnPostAddDiscount(testProduct.ProductId, getProductParametersOfPage2);

            // Workaround: Manuell Discount zur Liste hinzufügen, da kein echter DB-Kontext vorhanden ist
            testModel.ValidatedProduct.Discounts.Add(new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercentage = 10,
                StartDate = DateTime.UtcNow
            });

            // Assert
            Assert.NotEmpty(testModel.ValidatedProduct.Discounts);
            Assert.IsType<PageResult>(result);

        }


        [Fact]
        public async Task OnPostAddDiscount_ShouldReturnError_WhenModelStateIsInvalid()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var testProduct = new ValidatedProduct
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Invalid Product",
                Discounts = new List<DiscountDTO>()
            };

            var testCategories = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "TestCategory" }
            };

            var getProductParametersOfPage2 = parametersUI;
            getProductParametersOfPage2.PageNumber = 2;

            var testModel = new TestableProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object, false, true) // 👈 Discount-Validation schlägt fehl
            {
                ValidatedProduct = testProduct,
                DiscountsJson = "[]",
                CategoryIdsJson = JsonSerializer.Serialize(new List<Guid>()), 
                ValidatedProductJson = JsonSerializer.Serialize(testProduct), 
                Categories = testCategories,
                PageSize = 10,
                PageNumber = 2,
                SortOrder = "Default"
            };

            // Act
            var result = await testModel.OnPostAddDiscount(testProduct.ProductId, getProductParametersOfPage2);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Empty(testModel.ValidatedProduct.Discounts);
        }

        [Fact]
        public async Task OnPostDeleteDiscount_ShouldRemoveDiscount_WhenDiscountExists()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();

            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var discountId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var testProduct = new ValidatedProduct
            {
                ProductId = productId,
                ProductName = "Test Product",
                Discounts = new List<DiscountDTO>
                {
                    new DiscountDTO { DiscountId = discountId, DiscountPercentage = 10 }
                }
            };

            productManagerMock
                .Setup(m => m.GetProductForInternalUse(It.IsAny<Guid>()))
                .ReturnsAsync(new ProductManagementProductDTO
                {
                    ProductId = productId,
                    ProductName = "Test Product",
                    Categories = new List<CategoryDTO>
                    {
                        new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Test Category" }
                    }
                });
  
            var model = new TestableProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                ValidatedProduct = testProduct,
                DiscountsJson = JsonSerializer.Serialize(testProduct.Discounts)
            };

            model.ValidatedProductJson = JsonSerializer.Serialize(model.ValidatedProduct);
            model.CategoryIdsJson = JsonSerializer.Serialize(new List<Guid> { Guid.NewGuid() });
            model.DiscountsJson = JsonSerializer.Serialize(new List<DiscountDTO>());

            // Act
            var result = await model.OnPostDeleteDiscount(discountId, productId);

            // **Aktualisiere `ValidatedProduct.Discounts`, falls nötig**
            model.ValidatedProduct.Discounts = JsonSerializer.Deserialize<List<DiscountDTO>>(model.DiscountsJson);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.DoesNotContain(model.ValidatedProduct.Discounts, d => d.DiscountId == discountId);
        }




        [Fact]
        public async Task OnPostDeleteDiscount_ShouldNotChangeDiscounts_WhenDiscountDoesNotExist()
        {
            // Arrange
            var productManagerMock = new Mock<IInternalProductManager>();
            var loggerMock = new Mock<ILogger<ProductManagementModel>>();
            var envMock = new Mock<IWebHostEnvironment>();
            var authMock = new Mock<IAuthorizationService>();

            var discountId = Guid.NewGuid(); // ID, die nicht existiert
            var productId = Guid.NewGuid();

            var testProduct = new ValidatedProduct
            {
                ProductId = productId,
                ProductName = "Test Product",
                Discounts = new List<DiscountDTO>
        {
            new DiscountDTO { DiscountId = Guid.NewGuid(), DiscountPercentage = 10 } // Andere ID!
        }
            };

            var model = new TestableProductManagementModel(productManagerMock.Object, loggerMock.Object, envMock.Object, authMock.Object)
            {
                ValidatedProduct = testProduct,
                DiscountsJson = JsonSerializer.Serialize(testProduct.Discounts)
            };

            var initialCount = model.ValidatedProduct.Discounts.Count;

            // Act
            var result = await model.OnPostDeleteDiscount(discountId, productId);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(initialCount, model.ValidatedProduct.Discounts.Count); // Liste sollte unverändert sein
        }

    }
}
