﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RazorPagesSpielwiese.DBContexts;

#nullable disable

namespace RazorPagesSpielwiese.Migrations
{
    [DbContext(typeof(EvilCorp2000Context))]
    [Migration("20241029104747_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.Category", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            CategoryId = new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa"),
                            CategoryName = "Weapons"
                        },
                        new
                        {
                            CategoryId = new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994"),
                            CategoryName = "Facility"
                        },
                        new
                        {
                            CategoryId = new Guid("3be3d730-b553-4179-aca5-c3e107bdc2c0"),
                            CategoryName = "Furniture"
                        },
                        new
                        {
                            CategoryId = new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144"),
                            CategoryName = "Wearable"
                        },
                        new
                        {
                            CategoryId = new Guid("76c3a88e-3635-426a-b626-d51942ec6678"),
                            CategoryName = "Consumable"
                        },
                        new
                        {
                            CategoryId = new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5"),
                            CategoryName = "Scroll"
                        },
                        new
                        {
                            CategoryId = new Guid("e506c42a-0d97-48eb-a274-743682031d1f"),
                            CategoryName = "Training"
                        },
                        new
                        {
                            CategoryId = new Guid("42af02c6-970d-4768-aa3a-b321d4406837"),
                            CategoryName = "Magical Item"
                        },
                        new
                        {
                            CategoryId = new Guid("a2b69bf2-9b88-49a1-b7f3-02078acbbc27"),
                            CategoryName = "Trap"
                        },
                        new
                        {
                            CategoryId = new Guid("a8b4dcb1-b9be-45c9-aa74-6f88043f1ceb"),
                            CategoryName = "Decoration"
                        },
                        new
                        {
                            CategoryId = new Guid("023e69b2-204c-40a4-aecf-36ad94276dd5"),
                            CategoryName = "Transport"
                        },
                        new
                        {
                            CategoryId = new Guid("aba67adf-210f-4073-97fb-992c88b61018"),
                            CategoryName = "Personnel"
                        },
                        new
                        {
                            CategoryId = new Guid("cd4ebef9-4f75-4198-8771-9467bad98394"),
                            CategoryName = "Book"
                        },
                        new
                        {
                            CategoryId = new Guid("65416942-64f3-45c7-98c7-8fb6b0737ad5"),
                            CategoryName = "Ammunition"
                        });
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.Discount", b =>
                {
                    b.Property<Guid>("DiscountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("DiscountPercentage")
                        .HasColumnType("float");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("DiscountId");

                    b.HasIndex("ProductId");

                    b.ToTable("Discounts");

                    b.HasData(
                        new
                        {
                            DiscountId = new Guid("df3921aa-a5bd-43c1-9e03-4dba98bb6bc5"),
                            DiscountPercentage = 10.0,
                            EndDate = new DateTime(2024, 11, 18, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1140),
                            ProductId = new Guid("0076132c-5a07-4446-bee2-96bf3f097273"),
                            StartDate = new DateTime(2024, 10, 19, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1084)
                        },
                        new
                        {
                            DiscountId = new Guid("dcfafad6-82b6-4063-b5ce-ea53d9149f77"),
                            DiscountPercentage = 5.0,
                            EndDate = new DateTime(2024, 11, 13, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1145),
                            ProductId = new Guid("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"),
                            StartDate = new DateTime(2024, 10, 29, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1144)
                        });
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AmountOnStock")
                        .HasColumnType("int");

                    b.Property<string>("ProductDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ProductPicture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ProductPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.Property<double?>("Rating")
                        .HasColumnType("float");

                    b.HasKey("ProductId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = new Guid("0076132c-5a07-4446-bee2-96bf3f097273"),
                            AmountOnStock = 0,
                            ProductDescription = "Enjoy watching enemies experience primal fear. A terrifying experience for enemies.",
                            ProductName = "Shark Tank",
                            ProductPrice = 50000m
                        },
                        new
                        {
                            ProductId = new Guid("9c074b87-3109-401f-963b-3fd5b9449c26"),
                            AmountOnStock = 0,
                            ProductDescription = "The classic",
                            ProductName = "Guillotine",
                            ProductPrice = 5000m
                        },
                        new
                        {
                            ProductId = new Guid("2d821223-b7d5-494b-893b-d310b9266dec"),
                            AmountOnStock = 0,
                            ProductDescription = "Polyplastic knife - safe for planes. Comfortable and chic.",
                            ProductName = "Shoes with Hidden Knives",
                            ProductPrice = 50000m
                        },
                        new
                        {
                            ProductId = new Guid("2adcff47-f0ee-46da-b876-ae2e6803b8a8"),
                            AmountOnStock = 0,
                            ProductDescription = "Ideal for traps where the cage drops onto the target",
                            ProductName = "Cage Without Bottom",
                            ProductPrice = 200m
                        },
                        new
                        {
                            ProductId = new Guid("e656b89d-522b-49bc-9105-9fb261b9fd48"),
                            AmountOnStock = 0,
                            ProductDescription = "Convenient, silent, effective",
                            ProductName = "Narcotics - Set of 10 Syringes",
                            ProductPrice = 4000m
                        },
                        new
                        {
                            ProductId = new Guid("b4403ab3-c9a0-4bf2-b2b8-84420d5bec37"),
                            AmountOnStock = 0,
                            ProductDescription = "Stock up - you’ll need it",
                            ProductName = "Chloroform Bottle, 250ml",
                            ProductPrice = 100m
                        },
                        new
                        {
                            ProductId = new Guid("e6692d04-8556-4659-accd-341a187d1632"),
                            AmountOnStock = 0,
                            ProductDescription = "Causes intense nausea when cast upon a target",
                            ProductName = "Curse Scroll, Basic: Nausea",
                            ProductPrice = 30m
                        },
                        new
                        {
                            ProductId = new Guid("42868247-7fa2-4fea-a6eb-02edb3d7cb2f"),
                            AmountOnStock = 0,
                            ProductDescription = "Elaborately decorated with golden tentacles and a black metal blade, it’s ideal for both ritual and display",
                            ProductName = "Sacrificial Dagger",
                            ProductPrice = 10000m
                        },
                        new
                        {
                            ProductId = new Guid("dbf2f207-7edf-4642-87d7-bcc4ee8bf8e1"),
                            AmountOnStock = 0,
                            ProductDescription = "Large estate with leafless trees and an ivy-covered mansion with extensive cellars and catacombs",
                            ProductName = "Victorian Style Mansion",
                            ProductPrice = 1500000m
                        },
                        new
                        {
                            ProductId = new Guid("73565726-218f-4024-8de1-e950917ed8af"),
                            AmountOnStock = 0,
                            ProductDescription = "Simple but possible deadly. 10 darts included.",
                            ProductName = "Blowgun",
                            ProductPrice = 45m
                        },
                        new
                        {
                            ProductId = new Guid("29d74c54-e267-4ddb-b156-29a40a2c9581"),
                            AmountOnStock = 0,
                            ProductDescription = "3 darts prepared with the poison of the renowned Phyllobates terribilis. Highly effective and silent",
                            ProductName = "Blowgun Darts - Terribilis",
                            ProductPrice = 100m
                        },
                        new
                        {
                            ProductId = new Guid("45c32195-f5bc-4b61-9680-7fa8bb8ab321"),
                            AmountOnStock = 0,
                            ProductDescription = "A spine-chilling addition to any haunted property. Make everyone shiver with fright when nearing your not so secret hideout",
                            ProductName = "Creepy Gateway",
                            ProductPrice = 999m
                        },
                        new
                        {
                            ProductId = new Guid("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"),
                            AmountOnStock = 0,
                            ProductDescription = "Become proficient in the art of stealth and combat. Get a full bootcamp education to become a ninja. Use thorwing stars, wear a black mask and be stealthy as hell",
                            ProductName = "Ninja Master Education",
                            ProductPrice = 15000m
                        },
                        new
                        {
                            ProductId = new Guid("b9d6a7b2-8e7f-4e13-9dcf-5731f080d612"),
                            AmountOnStock = 50,
                            ProductDescription = "A powerful serum that temporarily enables mind control over unsuspecting targets. Effect duration varies.",
                            ProductName = "Mind-Control Serum",
                            ProductPrice = 1500m,
                            Rating = 4.7000000000000002
                        },
                        new
                        {
                            ProductId = new Guid("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"),
                            AmountOnStock = 0,
                            ProductDescription = "A cloak that grants temporary invisibility, ideal for stealth missions and unexpected escapes.",
                            ProductName = "Invisible Cloak",
                            ProductPrice = 25000m,
                            Rating = 4.9000000000000004
                        },
                        new
                        {
                            ProductId = new Guid("75ef12de-aeb6-4b0f-8bf4-21e1e6a67b7b"),
                            AmountOnStock = 5,
                            ProductDescription = "Single-use crystal that teleports the user to a pre-set location. Perfect for rapid escape.",
                            ProductName = "Teleportation Crystal",
                            ProductPrice = 50000m,
                            Rating = 4.7999999999999998
                        },
                        new
                        {
                            ProductId = new Guid("f60793df-4d93-4823-84ab-23699848b154"),
                            AmountOnStock = 7,
                            ProductDescription = "A mirror that shows the darkest fears of whoever gazes into it. Popular in haunted mansions.",
                            ProductName = "Cursed Mirror",
                            ProductPrice = 4000m,
                            Rating = 3.0
                        },
                        new
                        {
                            ProductId = new Guid("d0d5fd6d-9e4b-4b8b-8732-9f78f9a4e8c3"),
                            AmountOnStock = 30,
                            ProductDescription = "Artificial fangs filled with a potent venom. Apply a quick bite to immobilize your target.",
                            ProductName = "Venomous Fangs",
                            ProductPrice = 7500m,
                            Rating = 4.5999999999999996
                        },
                        new
                        {
                            ProductId = new Guid("e7b6d0ef-340b-47bb-b5c1-71bcdfe18e98"),
                            AmountOnStock = 20,
                            ProductDescription = "A spell scroll that unleashes a burst of dragon fire. Use with caution.",
                            ProductName = "Dragon Fire Scroll",
                            ProductPrice = 15000m,
                            Rating = 5.0
                        },
                        new
                        {
                            ProductId = new Guid("03d6b8bf-cbdf-4d1e-84f6-87b9f4f2de1a"),
                            AmountOnStock = 100,
                            ProductDescription = "Small but sturdy minions for spying, sabotage, or simply fetching coffee.",
                            ProductName = "Mechanical Minions (Pack of 5)",
                            ProductPrice = 2000m,
                            Rating = 4.2999999999999998
                        },
                        new
                        {
                            ProductId = new Guid("c1235c97-b12e-4e25-b67e-93f5097ecb6b"),
                            AmountOnStock = 12,
                            ProductDescription = "A comprehensive guide to curses both ancient and forbidden. Best used sparingly.",
                            ProductName = "Tome of Ancient Curses",
                            ProductPrice = 10000m,
                            Rating = 4.7999999999999998
                        },
                        new
                        {
                            ProductId = new Guid("8ae8dc53-8aaf-4679-98d6-38349b0c5de8"),
                            AmountOnStock = 3,
                            ProductDescription = "A foldable hideout that expands into a fully-equipped lair in remote locations.",
                            ProductName = "Portable Lair",
                            ProductPrice = 250000m,
                            Rating = 5.0
                        },
                        new
                        {
                            ProductId = new Guid("4f8391f1-0dfe-4b1b-b5d2-d9a4f7c1e921"),
                            AmountOnStock = 40,
                            ProductDescription = "Install this to create surprise trapdoors in your lair. Remote-controlled for convenience.",
                            ProductName = "Trap Door Mechanism",
                            ProductPrice = 10000m,
                            Rating = 4.4000000000000004
                        });
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.ProductCategoryMapping", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProductId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("ProductCategoryMappings");

                    b.HasData(
                        new
                        {
                            ProductId = new Guid("0076132c-5a07-4446-bee2-96bf3f097273"),
                            CategoryId = new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994")
                        },
                        new
                        {
                            ProductId = new Guid("0076132c-5a07-4446-bee2-96bf3f097273"),
                            CategoryId = new Guid("a8b4dcb1-b9be-45c9-aa74-6f88043f1ceb")
                        },
                        new
                        {
                            ProductId = new Guid("2d821223-b7d5-494b-893b-d310b9266dec"),
                            CategoryId = new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa")
                        },
                        new
                        {
                            ProductId = new Guid("2d821223-b7d5-494b-893b-d310b9266dec"),
                            CategoryId = new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144")
                        },
                        new
                        {
                            ProductId = new Guid("2adcff47-f0ee-46da-b876-ae2e6803b8a8"),
                            CategoryId = new Guid("a2b69bf2-9b88-49a1-b7f3-02078acbbc27")
                        },
                        new
                        {
                            ProductId = new Guid("e656b89d-522b-49bc-9105-9fb261b9fd48"),
                            CategoryId = new Guid("76c3a88e-3635-426a-b626-d51942ec6678")
                        },
                        new
                        {
                            ProductId = new Guid("b4403ab3-c9a0-4bf2-b2b8-84420d5bec37"),
                            CategoryId = new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5")
                        },
                        new
                        {
                            ProductId = new Guid("e6692d04-8556-4659-accd-341a187d1632"),
                            CategoryId = new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5")
                        },
                        new
                        {
                            ProductId = new Guid("42868247-7fa2-4fea-a6eb-02edb3d7cb2f"),
                            CategoryId = new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa")
                        },
                        new
                        {
                            ProductId = new Guid("dbf2f207-7edf-4642-87d7-bcc4ee8bf8e1"),
                            CategoryId = new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994")
                        },
                        new
                        {
                            ProductId = new Guid("73565726-218f-4024-8de1-e950917ed8af"),
                            CategoryId = new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa")
                        },
                        new
                        {
                            ProductId = new Guid("29d74c54-e267-4ddb-b156-29a40a2c9581"),
                            CategoryId = new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994")
                        },
                        new
                        {
                            ProductId = new Guid("45c32195-f5bc-4b61-9680-7fa8bb8ab321"),
                            CategoryId = new Guid("e506c42a-0d97-48eb-a274-743682031d1f")
                        },
                        new
                        {
                            ProductId = new Guid("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"),
                            CategoryId = new Guid("76c3a88e-3635-426a-b626-d51942ec6678")
                        },
                        new
                        {
                            ProductId = new Guid("b9d6a7b2-8e7f-4e13-9dcf-5731f080d612"),
                            CategoryId = new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144")
                        },
                        new
                        {
                            ProductId = new Guid("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"),
                            CategoryId = new Guid("42af02c6-970d-4768-aa3a-b321d4406837")
                        },
                        new
                        {
                            ProductId = new Guid("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"),
                            CategoryId = new Guid("023e69b2-204c-40a4-aecf-36ad94276dd5")
                        },
                        new
                        {
                            ProductId = new Guid("75ef12de-aeb6-4b0f-8bf4-21e1e6a67b7b"),
                            CategoryId = new Guid("42af02c6-970d-4768-aa3a-b321d4406837")
                        },
                        new
                        {
                            ProductId = new Guid("f60793df-4d93-4823-84ab-23699848b154"),
                            CategoryId = new Guid("42af02c6-970d-4768-aa3a-b321d4406837")
                        },
                        new
                        {
                            ProductId = new Guid("d0d5fd6d-9e4b-4b8b-8732-9f78f9a4e8c3"),
                            CategoryId = new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144")
                        },
                        new
                        {
                            ProductId = new Guid("e7b6d0ef-340b-47bb-b5c1-71bcdfe18e98"),
                            CategoryId = new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5")
                        },
                        new
                        {
                            ProductId = new Guid("03d6b8bf-cbdf-4d1e-84f6-87b9f4f2de1a"),
                            CategoryId = new Guid("aba67adf-210f-4073-97fb-992c88b61018")
                        },
                        new
                        {
                            ProductId = new Guid("c1235c97-b12e-4e25-b67e-93f5097ecb6b"),
                            CategoryId = new Guid("cd4ebef9-4f75-4198-8771-9467bad98394")
                        },
                        new
                        {
                            ProductId = new Guid("8ae8dc53-8aaf-4679-98d6-38349b0c5de8"),
                            CategoryId = new Guid("42af02c6-970d-4768-aa3a-b321d4406837")
                        },
                        new
                        {
                            ProductId = new Guid("4f8391f1-0dfe-4b1b-b5d2-d9a4f7c1e921"),
                            CategoryId = new Guid("a2b69bf2-9b88-49a1-b7f3-02078acbbc27")
                        },
                        new
                        {
                            ProductId = new Guid("9c074b87-3109-401f-963b-3fd5b9449c26"),
                            CategoryId = new Guid("3be3d730-b553-4179-aca5-c3e107bdc2c0")
                        });
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.Discount", b =>
                {
                    b.HasOne("RazorPagesSpielwiese.Entities.Product", "Product")
                        .WithMany("Discounts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.ProductCategoryMapping", b =>
                {
                    b.HasOne("RazorPagesSpielwiese.Entities.Category", "Category")
                        .WithMany("ProductCategoryMappings")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RazorPagesSpielwiese.Entities.Product", "Product")
                        .WithMany("ProductCategoryMappings")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.Category", b =>
                {
                    b.Navigation("ProductCategoryMappings");
                });

            modelBuilder.Entity("RazorPagesSpielwiese.Entities.Product", b =>
                {
                    b.Navigation("Discounts");

                    b.Navigation("ProductCategoryMappings");
                });
#pragma warning restore 612, 618
        }
    }
}
