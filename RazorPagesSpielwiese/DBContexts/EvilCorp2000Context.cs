using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.DBContexts;

public partial class EvilCorp2000Context : DbContext
{

    public EvilCorp2000Context(DbContextOptions<EvilCorp2000Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategoryMapping> ProductCategoryMappings { get; set; }

    public virtual DbSet<Category> Category { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
         modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId);
            entity.HasOne(d => d.Product)
                .WithMany(p => p.Discounts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ProductPrice).HasColumnType("decimal(18, 0)");
            //entity.HasMany(p => p.ProductCategoryMappings)
            //    .WithOne()
            //    .HasForeignKey(pcm => pcm.ProductId);
        });

        modelBuilder.Entity<ProductCategoryMapping>(entity =>
        {
            // Definiere ProductId und CategoryId als zusammengesetzten Primärschlüssel
            entity.HasKey(e => new { e.ProductId, e.CategoryId });

            // Fremdschlüssel für Product
            entity.HasOne(pcm => pcm.Product)
                .WithMany(p => p.ProductCategoryMappings)
                .HasForeignKey(pcm => pcm.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fremdschlüssel für Category
            entity.HasOne(pcm => pcm.Category)
                .WithMany(c => c.ProductCategoryMappings)
                .HasForeignKey(pcm => pcm.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        //für das direkte Mapping von Product und Productclass über ProductCategoryMappings
        modelBuilder.Entity<Category>(entity =>
        {
            //entity.HasMany(pc => pc.ProductCategoryMappings)
            //    .WithOne()
            //    .HasForeignKey(pcm => pcm.CategoryId);
        });

        // Product Seeding
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = Guid.Parse("0076132c-5a07-4446-bee2-96bf3f097273"), ProductName = "Shark Tank", ProductDescription = "Enjoy watching enemies experience primal fear. A terrifying experience for enemies.", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 50000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("9c074b87-3109-401f-963b-3fd5b9449c26"), ProductName = "Guillotine", ProductDescription = "The classic", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 5000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("2d821223-b7d5-494b-893b-d310b9266dec"), ProductName = "Shoes with Hidden Knives", ProductDescription = "Polyplastic knife - safe for planes. Comfortable and chic.", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 50000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("2adcff47-f0ee-46da-b876-ae2e6803b8a8"), ProductName = "Cage Without Bottom", ProductDescription = "Ideal for traps where the cage drops onto the target", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 200m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("e656b89d-522b-49bc-9105-9fb261b9fd48"), ProductName = "Narcotics - Set of 10 Syringes", ProductDescription = "Convenient, silent, effective", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 4000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("b4403ab3-c9a0-4bf2-b2b8-84420d5bec37"), ProductName = "Chloroform Bottle, 250ml", ProductDescription = "Stock up - you’ll need it", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 100m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("e6692d04-8556-4659-accd-341a187d1632"), ProductName = "Curse Scroll, Basic: Nausea", ProductDescription = "Causes intense nausea when cast upon a target", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 30m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("42868247-7fa2-4fea-a6eb-02edb3d7cb2f"), ProductName = "Sacrificial Dagger", ProductDescription = "Elaborately decorated with golden tentacles and a black metal blade, it’s ideal for both ritual and display", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 10000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("dbf2f207-7edf-4642-87d7-bcc4ee8bf8e1"), ProductName = "Victorian Style Mansion", ProductDescription = "Large estate with leafless trees and an ivy-covered mansion with extensive cellars and catacombs", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 1500000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("73565726-218f-4024-8de1-e950917ed8af"), ProductName = "Blowgun", ProductDescription = "Simple but possible deadly. 10 darts included.", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 45m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("29d74c54-e267-4ddb-b156-29a40a2c9581"), ProductName = "Blowgun Darts - Terribilis", ProductDescription = "3 darts prepared with the poison of the renowned Phyllobates terribilis. Highly effective and silent", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 100m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("45c32195-f5bc-4b61-9680-7fa8bb8ab321"), ProductName = "Creepy Gateway", ProductDescription = "A spine-chilling addition to any haunted property. Make everyone shiver with fright when nearing your not so secret hideout", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 999m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"), ProductName = "Ninja Master Education", ProductDescription = "Become proficient in the art of stealth and combat. Get a full bootcamp education to become a ninja. Use thorwing stars, wear a black mask and be stealthy as hell", ProductPicture = null, AmountOnStock = 0, Rating = null, ProductPrice = 15000m },//, ProductCategoryMappings = new List<ProductCategoryMapping>(), Discounts = new List<Discount>() },
                new Product { ProductId = Guid.Parse("b9d6a7b2-8e7f-4e13-9dcf-5731f080d612"), ProductName = "Mind-Control Serum", ProductDescription = "A powerful serum that temporarily enables mind control over unsuspecting targets. Effect duration varies.", ProductPicture = null, AmountOnStock = 50, Rating = 4.7, ProductPrice = 1500m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"), ProductName = "Invisible Cloak", ProductDescription = "A cloak that grants temporary invisibility, ideal for stealth missions and unexpected escapes.", ProductPicture = null, AmountOnStock = 0, Rating = 4.9, ProductPrice = 25000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("75ef12de-aeb6-4b0f-8bf4-21e1e6a67b7b"), ProductName = "Teleportation Crystal", ProductDescription = "Single-use crystal that teleports the user to a pre-set location. Perfect for rapid escape.", ProductPicture = null, AmountOnStock = 5, Rating = 4.8, ProductPrice = 50000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("f60793df-4d93-4823-84ab-23699848b154"), ProductName = "Cursed Mirror", ProductDescription = "A mirror that shows the darkest fears of whoever gazes into it. Popular in haunted mansions.", ProductPicture = null, AmountOnStock = 7, Rating = 3, ProductPrice = 4000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("d0d5fd6d-9e4b-4b8b-8732-9f78f9a4e8c3"), ProductName = "Venomous Fangs", ProductDescription = "Artificial fangs filled with a potent venom. Apply a quick bite to immobilize your target.", ProductPicture = null, AmountOnStock = 30, Rating = 4.6, ProductPrice = 7500m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("e7b6d0ef-340b-47bb-b5c1-71bcdfe18e98"), ProductName = "Dragon Fire Scroll", ProductDescription = "A spell scroll that unleashes a burst of dragon fire. Use with caution.", ProductPicture = null, AmountOnStock = 20, Rating = 5.0, ProductPrice = 15000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("03d6b8bf-cbdf-4d1e-84f6-87b9f4f2de1a"), ProductName = "Mechanical Minions (Pack of 5)", ProductDescription = "Small but sturdy minions for spying, sabotage, or simply fetching coffee.", ProductPicture = null, AmountOnStock = 100, Rating = 4.3, ProductPrice = 2000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("c1235c97-b12e-4e25-b67e-93f5097ecb6b"), ProductName = "Tome of Ancient Curses", ProductDescription = "A comprehensive guide to curses both ancient and forbidden. Best used sparingly.", ProductPicture = null, AmountOnStock = 12, Rating = 4.8, ProductPrice = 10000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("8ae8dc53-8aaf-4679-98d6-38349b0c5de8"), ProductName = "Portable Lair", ProductDescription = "A foldable hideout that expands into a fully-equipped lair in remote locations.", ProductPicture = null, AmountOnStock = 3, Rating = 5.0, ProductPrice = 250000m },//, ProductCategoryMappings = [], Discounts = [] },
                new Product { ProductId = Guid.Parse("4f8391f1-0dfe-4b1b-b5d2-d9a4f7c1e921"), ProductName = "Trap Door Mechanism", ProductDescription = "Install this to create surprise trapdoors in your lair. Remote-controlled for convenience.", ProductPicture = null, AmountOnStock = 40, Rating = 4.4, ProductPrice = 10000m }//, ProductCategoryMappings = [], Discounts = [] }
        );

        // Category Seeding
        modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = Guid.Parse("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa"), CategoryName = "Weapons" },
                new Category { CategoryId = Guid.Parse("5b07c57d-4027-4558-99ed-1dd86b3b9994"), CategoryName = "Facility" },
                new Category { CategoryId = Guid.Parse("3be3d730-b553-4179-aca5-c3e107bdc2c0"), CategoryName = "Furniture" },
                new Category { CategoryId = Guid.Parse("500a2093-5f38-4ef9-bf0d-63dc2497c144"), CategoryName = "Wearable" },
                new Category { CategoryId = Guid.Parse("76c3a88e-3635-426a-b626-d51942ec6678"), CategoryName = "Consumable" },
                new Category { CategoryId = Guid.Parse("d495519d-3230-40d9-9a79-e767b7c78bc5"), CategoryName = "Scroll" },
                new Category { CategoryId = Guid.Parse("e506c42a-0d97-48eb-a274-743682031d1f"), CategoryName = "Training" },
                new Category { CategoryId = Guid.Parse("42af02c6-970d-4768-aa3a-b321d4406837"), CategoryName = "Magical Item" },
                new Category { CategoryId = Guid.Parse("a2b69bf2-9b88-49a1-b7f3-02078acbbc27"), CategoryName = "Trap" },
                new Category { CategoryId = Guid.Parse("a8b4dcb1-b9be-45c9-aa74-6f88043f1ceb"), CategoryName = "Decoration" },
                new Category { CategoryId = Guid.Parse("023e69b2-204c-40a4-aecf-36ad94276dd5"), CategoryName = "Transport" },
                new Category { CategoryId = Guid.Parse("aba67adf-210f-4073-97fb-992c88b61018"), CategoryName = "Personnel" },
                new Category { CategoryId = Guid.Parse("cd4ebef9-4f75-4198-8771-9467bad98394"), CategoryName = "Book" },
                new Category { CategoryId = Guid.Parse("65416942-64f3-45c7-98c7-8fb6b0737ad5"), CategoryName = "Ammunition" }
            );

        // Discount Seeding
        modelBuilder.Entity<Discount>().HasData(
            new Discount { DiscountId = Guid.Parse("df3921aa-a5bd-43c1-9e03-4dba98bb6bc5"), ProductId = Guid.Parse("0076132c-5a07-4446-bee2-96bf3f097273"), StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(20), DiscountPercentage = 10.0 },
            new Discount { DiscountId = Guid.Parse("dcfafad6-82b6-4063-b5ce-ea53d9149f77"), ProductId = Guid.Parse("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"), StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(15), DiscountPercentage = 5.0 }
        );

        // ProductCategoryMapping Seeding (Many-to-Many Relationship)
        modelBuilder.Entity<ProductCategoryMapping>().HasData(
            new ProductCategoryMapping { ProductId = Guid.Parse("0076132c-5a07-4446-bee2-96bf3f097273"), CategoryId = Guid.Parse("5b07c57d-4027-4558-99ed-1dd86b3b9994") },
            new ProductCategoryMapping { ProductId = Guid.Parse("0076132c-5a07-4446-bee2-96bf3f097273"), CategoryId = Guid.Parse("a8b4dcb1-b9be-45c9-aa74-6f88043f1ceb") },

            new ProductCategoryMapping { ProductId = Guid.Parse("2d821223-b7d5-494b-893b-d310b9266dec"), CategoryId = Guid.Parse("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa") },
            new ProductCategoryMapping { ProductId = Guid.Parse("2d821223-b7d5-494b-893b-d310b9266dec"), CategoryId = Guid.Parse("500a2093-5f38-4ef9-bf0d-63dc2497c144") },
            new ProductCategoryMapping { ProductId = Guid.Parse("2adcff47-f0ee-46da-b876-ae2e6803b8a8"), CategoryId = Guid.Parse("a2b69bf2-9b88-49a1-b7f3-02078acbbc27") },
            new ProductCategoryMapping { ProductId = Guid.Parse("e656b89d-522b-49bc-9105-9fb261b9fd48"), CategoryId = Guid.Parse("76c3a88e-3635-426a-b626-d51942ec6678") },
            new ProductCategoryMapping { ProductId = Guid.Parse("b4403ab3-c9a0-4bf2-b2b8-84420d5bec37"), CategoryId = Guid.Parse("d495519d-3230-40d9-9a79-e767b7c78bc5") },
            new ProductCategoryMapping { ProductId = Guid.Parse("e6692d04-8556-4659-accd-341a187d1632"), CategoryId = Guid.Parse("d495519d-3230-40d9-9a79-e767b7c78bc5") },
            new ProductCategoryMapping { ProductId = Guid.Parse("42868247-7fa2-4fea-a6eb-02edb3d7cb2f"), CategoryId = Guid.Parse("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa") },
            new ProductCategoryMapping { ProductId = Guid.Parse("dbf2f207-7edf-4642-87d7-bcc4ee8bf8e1"), CategoryId = Guid.Parse("5b07c57d-4027-4558-99ed-1dd86b3b9994") },
            new ProductCategoryMapping { ProductId = Guid.Parse("73565726-218f-4024-8de1-e950917ed8af"), CategoryId = Guid.Parse("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa") },
            new ProductCategoryMapping { ProductId = Guid.Parse("29d74c54-e267-4ddb-b156-29a40a2c9581"), CategoryId = Guid.Parse("5b07c57d-4027-4558-99ed-1dd86b3b9994") },
            new ProductCategoryMapping { ProductId = Guid.Parse("45c32195-f5bc-4b61-9680-7fa8bb8ab321"), CategoryId = Guid.Parse("e506c42a-0d97-48eb-a274-743682031d1f") },
            new ProductCategoryMapping { ProductId = Guid.Parse("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"), CategoryId = Guid.Parse("76c3a88e-3635-426a-b626-d51942ec6678") },
            new ProductCategoryMapping { ProductId = Guid.Parse("b9d6a7b2-8e7f-4e13-9dcf-5731f080d612"), CategoryId = Guid.Parse("500a2093-5f38-4ef9-bf0d-63dc2497c144") },
            new ProductCategoryMapping { ProductId = Guid.Parse("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"), CategoryId = Guid.Parse("42af02c6-970d-4768-aa3a-b321d4406837") },
            new ProductCategoryMapping { ProductId = Guid.Parse("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"), CategoryId = Guid.Parse("023e69b2-204c-40a4-aecf-36ad94276dd5") },
            new ProductCategoryMapping { ProductId = Guid.Parse("75ef12de-aeb6-4b0f-8bf4-21e1e6a67b7b"), CategoryId = Guid.Parse("42af02c6-970d-4768-aa3a-b321d4406837") },
            new ProductCategoryMapping { ProductId = Guid.Parse("f60793df-4d93-4823-84ab-23699848b154"), CategoryId = Guid.Parse("42af02c6-970d-4768-aa3a-b321d4406837") },
            new ProductCategoryMapping { ProductId = Guid.Parse("d0d5fd6d-9e4b-4b8b-8732-9f78f9a4e8c3"), CategoryId = Guid.Parse("500a2093-5f38-4ef9-bf0d-63dc2497c144") },
            new ProductCategoryMapping { ProductId = Guid.Parse("e7b6d0ef-340b-47bb-b5c1-71bcdfe18e98"), CategoryId = Guid.Parse("d495519d-3230-40d9-9a79-e767b7c78bc5") },
            new ProductCategoryMapping { ProductId = Guid.Parse("03d6b8bf-cbdf-4d1e-84f6-87b9f4f2de1a"), CategoryId = Guid.Parse("aba67adf-210f-4073-97fb-992c88b61018") },
            new ProductCategoryMapping { ProductId = Guid.Parse("c1235c97-b12e-4e25-b67e-93f5097ecb6b"), CategoryId = Guid.Parse("cd4ebef9-4f75-4198-8771-9467bad98394") },
            new ProductCategoryMapping { ProductId = Guid.Parse("8ae8dc53-8aaf-4679-98d6-38349b0c5de8"), CategoryId = Guid.Parse("42af02c6-970d-4768-aa3a-b321d4406837") },
            new ProductCategoryMapping { ProductId = Guid.Parse("4f8391f1-0dfe-4b1b-b5d2-d9a4f7c1e921"), CategoryId = Guid.Parse("a2b69bf2-9b88-49a1-b7f3-02078acbbc27") },
            new ProductCategoryMapping { ProductId = Guid.Parse("9c074b87-3109-401f-963b-3fd5b9449c26"), CategoryId = Guid.Parse("3be3d730-b553-4179-aca5-c3e107bdc2c0") }
        );


        base.OnModelCreating(modelBuilder);
    }
}
