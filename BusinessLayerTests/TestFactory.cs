using BusinessLayer.Models;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayerTests
{
    public static class TestFactory
    {
        public static class TestDataFactory
        {
            public static Product CreateCompleteProduct()
            {
                return new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    ProductPrice = 100.00m,
                    AmountOnStock = 10,
                    ProductDescription = "Test Description",
                    ProductPicture = "test.jpg",
                    Categories = new List<Category>
                {
                    new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" }
                },
                    Discounts = new List<Discount>
                {
                    new Discount { DiscountId = Guid.NewGuid(), DiscountPercentage = 10, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5) }
                }
                };
            }

            public static ProductManagementProductDTO CreateCompleteProductDTO()
            {
                return new ProductManagementProductDTO
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Price = 100.00m,
                    AmountOnStock = 10,
                    Description = "Test Description",
                    ProductPicture = "test.jpg",
                    Categories = new List<CategoryDTO>
                {
                    new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" }
                },
                    Discounts = new List<DiscountDTO>
                {
                    new DiscountDTO { DiscountId = Guid.NewGuid(), DiscountPercentage = 10, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5) }
                }
                };
            }

            public static Category CreateCategory(string name)
            {
                return new Category
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = name
                };
            }

            public static CategoryDTO CreateCategoryDTO(string name)
            {
                return new CategoryDTO
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = name
                };
            }

            public static Discount CreateDiscount(double percentage)
            {
                return new Discount
                {
                    DiscountId = Guid.NewGuid(),
                    DiscountPercentage = percentage,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5)
                };
            }

            public static DiscountDTO CreateDiscountDTO(double percentage)
            {
                return new DiscountDTO
                {
                    DiscountId = Guid.NewGuid(),
                    DiscountPercentage = percentage,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5)
                };
            }

            public static Product CreateProduct(string name, decimal price, List<Category> categories = null, List<Discount> discounts = null)
            {
                return new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = name,
                    ProductPrice = price,
                    ProductDescription = "Test Description",
                    ProductPicture = "test.jpg",
                    AmountOnStock = 10,
                    Categories = categories ?? new List<Category> { CreateCategory("Weapons") },
                    Discounts = discounts ?? new List<Discount> { CreateDiscount(10) }
                };
            }

            public static ProductManagementProductDTO CreateProductManagementDTO(string name, decimal price, List<CategoryDTO> categories = null, List<DiscountDTO> discounts = null)
            {
                return new ProductManagementProductDTO
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = name,
                    Price = price,
                    Description = "Test Description",
                    ProductPicture = "test.jpg",
                    AmountOnStock = 10,
                    Categories = categories ?? new List<CategoryDTO> { CreateCategoryDTO("Weapons") },
                    Discounts = discounts ?? new List<DiscountDTO> { CreateDiscountDTO(10) }
                };
            }
        }
    }
}
