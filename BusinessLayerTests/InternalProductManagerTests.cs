using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccess.Entities;
using DataAccess.Repositories;
using static BusinessLayerTests.TestFactory;
using Moq;

namespace BusinessLayerTests
{
    public class InternalProductManagerTests
    {
        //Moq, um die Repos zu simulieren --> es wird nur die Logik des InternalProductManagers getestet
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly Mock<IDiscountRepository> _discountRepositoryMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
        private readonly InternalProductManager _productManager;

        public InternalProductManagerTests()
        {
            _productManager = new InternalProductManager(
                _discountRepositoryMock.Object,
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object
            );
        }

        
        [Fact]
        public async Task GetProductsForInternalUse_ShouldReturnProductList()
        {
            // Arrange
            var products = new List<Product>
            {
                TestDataFactory.CreateCompleteProduct()
            };

            //INFO: Setup Methoden sagen, was passieren soll, wenn die RepoFktn aufgerufen werden
            //ReturnsAsync gibt hier eine Liste zurück, als würde sie aus einer Datenbank kommen
            _productRepositoryMock.Setup(repo => repo.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _productManager.GetProductsForInternalUse();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Product", result[0].ProductName);
        }

        [Fact]
        public void MapDiscountsAndCategoriesToDTOs_ShouldThrowException_WhenCategoriesOrDiscountsContainNull()
        {
            Assert.Throws<ArgumentNullException>(() => _productManager.MapDiscountsAndCategoriesToDTOs([null], [null]));
        }

        [Fact]
        public void MapDiscountsAndCategoriesToDTOs_ShouldThrowException_WhenCategoriesOrDiscountsAreNull()
        {
            Assert.Throws<ArgumentNullException>( () => _productManager.MapDiscountsAndCategoriesToDTOs(null, null));
        }

        [Fact]
        public void MapDiscountsAndCategoriesToEntities_ShouldThrowException_WhenCategoriesOrDiscountsContainNull()
        {
            Assert.Throws<ArgumentNullException>(() => _productManager.MapDiscountsAndCategoriesToEntities([null], [null], Guid.NewGuid()));
        }

        [Fact]
        public void MapDiscountsAndCategoriesToEntities_ShouldThrowException_WhenCategoriesOrDiscountsAreNull()
        {
            Assert.Throws<ArgumentNullException>(() => _productManager.MapDiscountsAndCategoriesToEntities(null, null, Guid.NewGuid()));
        }

        [Fact]
        public async Task GetProductForInternalUse_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            Guid productId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.GetProductByIdWithCategoriesAnsdDiscounts(productId))
                .ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productManager.GetProductForInternalUse(productId));
        }


        [Fact]
        public async Task GetProductForInternalUse_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = TestDataFactory.CreateCompleteProduct();
            var productId = product.ProductId;

            _productRepositoryMock
                .Setup(repo => repo.GetProductByIdWithCategoriesAnsdDiscounts(productId))
                .ReturnsAsync(product);

            _discountRepositoryMock
                .Setup(repo => repo.GetDiscountsByProductId(productId))
                .ReturnsAsync(product.Discounts.ToList());

            // Act
            var result = await _productManager.GetProductForInternalUse(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductName, result.ProductName);
            Assert.Equal(product.ProductPrice, result.Price);
            Assert.Equal(product.Categories.Count, result.Categories.Count);
            Assert.Equal(product.Discounts.Count, result.Discounts.Count);
        }

        [Fact]
        public async Task SaveProductToStore_ShouldSaveProduct_WhenValidDataProvided()
        {
            // Arrange
            var productDTO = TestDataFactory.CreateCompleteProductDTO();

            _categoryRepositoryMock
                .Setup(repo => repo.AttachCategoriesIfNeeded(It.IsAny<List<Category>>()))
                .Returns<List<Category>>(categories => categories);

            _productRepositoryMock
                .Setup(repo => repo.IsProductNameUniqueAsync(productDTO.ProductName, productDTO.ProductId))
                .ReturnsAsync(true);

            _productRepositoryMock
                .Setup(repo => repo.AddProduct(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            await _productManager.SaveProductToStore(productDTO);

            // Assert
            _productRepositoryMock.Verify(repo => repo.AddProduct(It.IsAny<Product>()), Times.Once);
        }


        [Fact]
        public async Task SaveProductToStore_ShouldThrowException_WhenProductIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productManager.SaveProductToStore(null));
        }

        [Fact]
        public async Task SaveProductToStore_ShouldCallAddProduct()
        {
            // Arrange
            var productDTO = TestDataFactory.CreateCompleteProductDTO();

            // Gibt einfach die Liste zurück
            _categoryRepositoryMock
                .Setup(repo => repo.AttachCategoriesIfNeeded(It.IsAny<List<Category>>()))
                .Returns<List<Category>>(categories => categories); 

            _productRepositoryMock.Setup(repo => repo.IsProductNameUniqueAsync(productDTO.ProductName, productDTO.ProductId))
                .ReturnsAsync(true);

            // Act
            await _productManager.SaveProductToStore(productDTO);

            // Assert
            _productRepositoryMock.Verify(repo => repo.AddProduct(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductToStore_ShouldUpdateProduct_WhenValidDataProvided()
        {
            // Arrange
            var productDTO = TestDataFactory.CreateCompleteProductDTO();
            var product = TestDataFactory.CreateCompleteProduct();

            _productRepositoryMock
                .Setup(repo => repo.GetProductByIdWithCategoriesAnsdDiscounts(productDTO.ProductId))
                .ReturnsAsync(product);

            _productRepositoryMock
                .Setup(repo => repo.UpdateProduct(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _productRepositoryMock
                .Setup(repo => repo.IsProductNameUniqueAsync(productDTO.ProductName, productDTO.ProductId))
                .ReturnsAsync(true);

            // Act
            await _productManager.UpdateProductToStore(productDTO);

            // Assert
            _productRepositoryMock.Verify(repo => repo.UpdateProduct(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductToStore_ShouldThrowException_WhenProductIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productManager.UpdateProductToStore(null));
        }

        [Fact]
        public async Task DeleteProduct_ShouldCallRepository_WhenProductIdIsValid()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productRepositoryMock
                .Setup(repo => repo.DeleteProduct(productId))
                .Returns(Task.CompletedTask);

            // Act
            await _productManager.DeleteProduct(productId);

            // Assert
            _productRepositoryMock.Verify(repo => repo.DeleteProduct(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowException_WhenProductIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productManager.DeleteProduct(Guid.Empty));
        }

        [Fact]
        public async Task DeleteProduct_ShouldCallRepositoryMethod()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepositoryMock.Setup(repo => repo.DeleteProduct(productId)).Returns(Task.CompletedTask);

            // Act
            await _productManager.DeleteProduct(productId);

            // Assert
            _productRepositoryMock.Verify(repo => repo.DeleteProduct(productId), Times.Once);
        }

        [Fact]
        public async Task SaveProductPicture_ShouldCallRepository_WhenValidDataProvided()
        {
            // Arrange
            var productId = Guid.NewGuid();
            string picture = "encoded_picture_data";

            _productRepositoryMock
                .Setup(repo => repo.SaveProductPicture(productId, picture))
                .Returns(Task.CompletedTask);

            // Act
            await _productManager.SaveProductPicture(productId, picture);

            // Assert
            _productRepositoryMock.Verify(repo => repo.SaveProductPicture(productId, picture), Times.Once);
        }

        [Fact]
        public async Task SaveProductPicture_ShouldThrowException_WhenProductIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productManager.SaveProductPicture(Guid.Empty, "image.jpg"));
        }

        [Fact]
        public async Task SaveProductPicture_ShouldCallRepositoryMethod()
        {
            // Arrange
            var productId = Guid.NewGuid();
            string picture = "image.jpg";

            // Act
            await _productManager.SaveProductPicture(productId, picture);

            // Assert
            _productRepositoryMock.Verify(repo => repo.SaveProductPicture(productId, picture), Times.Once);
        }

        [Fact]
        public async Task DeleteProductPicture_ShouldThrowException_WhenProductIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productManager.DeleteProductPicture(Guid.Empty));
        }

        [Fact]
        public async Task DeleteProductPicture_ShouldCallRepositoryMethod()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            await _productManager.DeleteProductPicture(productId);

            // Assert
            _productRepositoryMock.Verify(repo => repo.DeleteProductPicture(productId), Times.Once);
        }

        

    }
}
