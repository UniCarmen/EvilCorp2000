using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RazorPagesSpielwiese.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountOnStock = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscountPercentage = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.DiscountId);
                    table.ForeignKey(
                        name: "FK_Discounts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategoryMappings",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategoryMappings", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProductCategoryMappings_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductCategoryMappings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { new Guid("023e69b2-204c-40a4-aecf-36ad94276dd5"), "Transport" },
                    { new Guid("3be3d730-b553-4179-aca5-c3e107bdc2c0"), "Furniture" },
                    { new Guid("42af02c6-970d-4768-aa3a-b321d4406837"), "Magical Item" },
                    { new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144"), "Wearable" },
                    { new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994"), "Facility" },
                    { new Guid("65416942-64f3-45c7-98c7-8fb6b0737ad5"), "Ammunition" },
                    { new Guid("76c3a88e-3635-426a-b626-d51942ec6678"), "Consumable" },
                    { new Guid("a2b69bf2-9b88-49a1-b7f3-02078acbbc27"), "Trap" },
                    { new Guid("a8b4dcb1-b9be-45c9-aa74-6f88043f1ceb"), "Decoration" },
                    { new Guid("aba67adf-210f-4073-97fb-992c88b61018"), "Personnel" },
                    { new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa"), "Weapons" },
                    { new Guid("cd4ebef9-4f75-4198-8771-9467bad98394"), "Book" },
                    { new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5"), "Scroll" },
                    { new Guid("e506c42a-0d97-48eb-a274-743682031d1f"), "Training" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AmountOnStock", "ProductDescription", "ProductName", "ProductPicture", "ProductPrice", "Rating" },
                values: new object[,]
                {
                    { new Guid("0076132c-5a07-4446-bee2-96bf3f097273"), 0, "Enjoy watching enemies experience primal fear. A terrifying experience for enemies.", "Shark Tank", null, 50000m, null },
                    { new Guid("03d6b8bf-cbdf-4d1e-84f6-87b9f4f2de1a"), 100, "Small but sturdy minions for spying, sabotage, or simply fetching coffee.", "Mechanical Minions (Pack of 5)", null, 2000m, 4.2999999999999998 },
                    { new Guid("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4"), 0, "A cloak that grants temporary invisibility, ideal for stealth missions and unexpected escapes.", "Invisible Cloak", null, 25000m, 4.9000000000000004 },
                    { new Guid("29d74c54-e267-4ddb-b156-29a40a2c9581"), 0, "3 darts prepared with the poison of the renowned Phyllobates terribilis. Highly effective and silent", "Blowgun Darts - Terribilis", null, 100m, null },
                    { new Guid("2adcff47-f0ee-46da-b876-ae2e6803b8a8"), 0, "Ideal for traps where the cage drops onto the target", "Cage Without Bottom", null, 200m, null },
                    { new Guid("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"), 0, "Become proficient in the art of stealth and combat. Get a full bootcamp education to become a ninja. Use thorwing stars, wear a black mask and be stealthy as hell", "Ninja Master Education", null, 15000m, null },
                    { new Guid("2d821223-b7d5-494b-893b-d310b9266dec"), 0, "Polyplastic knife - safe for planes. Comfortable and chic.", "Shoes with Hidden Knives", null, 50000m, null },
                    { new Guid("42868247-7fa2-4fea-a6eb-02edb3d7cb2f"), 0, "Elaborately decorated with golden tentacles and a black metal blade, it’s ideal for both ritual and display", "Sacrificial Dagger", null, 10000m, null },
                    { new Guid("45c32195-f5bc-4b61-9680-7fa8bb8ab321"), 0, "A spine-chilling addition to any haunted property. Make everyone shiver with fright when nearing your not so secret hideout", "Creepy Gateway", null, 999m, null },
                    { new Guid("4f8391f1-0dfe-4b1b-b5d2-d9a4f7c1e921"), 40, "Install this to create surprise trapdoors in your lair. Remote-controlled for convenience.", "Trap Door Mechanism", null, 10000m, 4.4000000000000004 },
                    { new Guid("73565726-218f-4024-8de1-e950917ed8af"), 0, "Simple but possible deadly. 10 darts included.", "Blowgun", null, 45m, null },
                    { new Guid("75ef12de-aeb6-4b0f-8bf4-21e1e6a67b7b"), 5, "Single-use crystal that teleports the user to a pre-set location. Perfect for rapid escape.", "Teleportation Crystal", null, 50000m, 4.7999999999999998 },
                    { new Guid("8ae8dc53-8aaf-4679-98d6-38349b0c5de8"), 3, "A foldable hideout that expands into a fully-equipped lair in remote locations.", "Portable Lair", null, 250000m, 5.0 },
                    { new Guid("9c074b87-3109-401f-963b-3fd5b9449c26"), 0, "The classic", "Guillotine", null, 5000m, null },
                    { new Guid("b4403ab3-c9a0-4bf2-b2b8-84420d5bec37"), 0, "Stock up - you’ll need it", "Chloroform Bottle, 250ml", null, 100m, null },
                    { new Guid("b9d6a7b2-8e7f-4e13-9dcf-5731f080d612"), 50, "A powerful serum that temporarily enables mind control over unsuspecting targets. Effect duration varies.", "Mind-Control Serum", null, 1500m, 4.7000000000000002 },
                    { new Guid("c1235c97-b12e-4e25-b67e-93f5097ecb6b"), 12, "A comprehensive guide to curses both ancient and forbidden. Best used sparingly.", "Tome of Ancient Curses", null, 10000m, 4.7999999999999998 },
                    { new Guid("d0d5fd6d-9e4b-4b8b-8732-9f78f9a4e8c3"), 30, "Artificial fangs filled with a potent venom. Apply a quick bite to immobilize your target.", "Venomous Fangs", null, 7500m, 4.5999999999999996 },
                    { new Guid("dbf2f207-7edf-4642-87d7-bcc4ee8bf8e1"), 0, "Large estate with leafless trees and an ivy-covered mansion with extensive cellars and catacombs", "Victorian Style Mansion", null, 1500000m, null },
                    { new Guid("e656b89d-522b-49bc-9105-9fb261b9fd48"), 0, "Convenient, silent, effective", "Narcotics - Set of 10 Syringes", null, 4000m, null },
                    { new Guid("e6692d04-8556-4659-accd-341a187d1632"), 0, "Causes intense nausea when cast upon a target", "Curse Scroll, Basic: Nausea", null, 30m, null },
                    { new Guid("e7b6d0ef-340b-47bb-b5c1-71bcdfe18e98"), 20, "A spell scroll that unleashes a burst of dragon fire. Use with caution.", "Dragon Fire Scroll", null, 15000m, 5.0 },
                    { new Guid("f60793df-4d93-4823-84ab-23699848b154"), 7, "A mirror that shows the darkest fears of whoever gazes into it. Popular in haunted mansions.", "Cursed Mirror", null, 4000m, 3.0 }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "DiscountId", "DiscountPercentage", "EndDate", "ProductId", "StartDate" },
                values: new object[,]
                {
                    { new Guid("dcfafad6-82b6-4063-b5ce-ea53d9149f77"), 5.0, new DateTime(2024, 11, 13, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1145), new Guid("2b860dae-c065-4f8f-a133-8bbf5c9a2da4"), new DateTime(2024, 10, 29, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1144) },
                    { new Guid("df3921aa-a5bd-43c1-9e03-4dba98bb6bc5"), 10.0, new DateTime(2024, 11, 18, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1140), new Guid("0076132c-5a07-4446-bee2-96bf3f097273"), new DateTime(2024, 10, 19, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1084) }
                });

            migrationBuilder.InsertData(
                table: "ProductCategoryMappings",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[,]
                {
                    { new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994"), new Guid("0076132c-5a07-4446-bee2-96bf3f097273") },
                    { new Guid("a8b4dcb1-b9be-45c9-aa74-6f88043f1ceb"), new Guid("0076132c-5a07-4446-bee2-96bf3f097273") },
                    { new Guid("aba67adf-210f-4073-97fb-992c88b61018"), new Guid("03d6b8bf-cbdf-4d1e-84f6-87b9f4f2de1a") },
                    { new Guid("023e69b2-204c-40a4-aecf-36ad94276dd5"), new Guid("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4") },
                    { new Guid("42af02c6-970d-4768-aa3a-b321d4406837"), new Guid("0ac29d9a-3d3c-46e8-98b8-2174e50bd9e4") },
                    { new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994"), new Guid("29d74c54-e267-4ddb-b156-29a40a2c9581") },
                    { new Guid("a2b69bf2-9b88-49a1-b7f3-02078acbbc27"), new Guid("2adcff47-f0ee-46da-b876-ae2e6803b8a8") },
                    { new Guid("76c3a88e-3635-426a-b626-d51942ec6678"), new Guid("2b860dae-c065-4f8f-a133-8bbf5c9a2da4") },
                    { new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144"), new Guid("2d821223-b7d5-494b-893b-d310b9266dec") },
                    { new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa"), new Guid("2d821223-b7d5-494b-893b-d310b9266dec") },
                    { new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa"), new Guid("42868247-7fa2-4fea-a6eb-02edb3d7cb2f") },
                    { new Guid("e506c42a-0d97-48eb-a274-743682031d1f"), new Guid("45c32195-f5bc-4b61-9680-7fa8bb8ab321") },
                    { new Guid("a2b69bf2-9b88-49a1-b7f3-02078acbbc27"), new Guid("4f8391f1-0dfe-4b1b-b5d2-d9a4f7c1e921") },
                    { new Guid("c3ad7a7d-b333-4f59-84c1-e1244ec0e3aa"), new Guid("73565726-218f-4024-8de1-e950917ed8af") },
                    { new Guid("42af02c6-970d-4768-aa3a-b321d4406837"), new Guid("75ef12de-aeb6-4b0f-8bf4-21e1e6a67b7b") },
                    { new Guid("42af02c6-970d-4768-aa3a-b321d4406837"), new Guid("8ae8dc53-8aaf-4679-98d6-38349b0c5de8") },
                    { new Guid("3be3d730-b553-4179-aca5-c3e107bdc2c0"), new Guid("9c074b87-3109-401f-963b-3fd5b9449c26") },
                    { new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5"), new Guid("b4403ab3-c9a0-4bf2-b2b8-84420d5bec37") },
                    { new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144"), new Guid("b9d6a7b2-8e7f-4e13-9dcf-5731f080d612") },
                    { new Guid("cd4ebef9-4f75-4198-8771-9467bad98394"), new Guid("c1235c97-b12e-4e25-b67e-93f5097ecb6b") },
                    { new Guid("500a2093-5f38-4ef9-bf0d-63dc2497c144"), new Guid("d0d5fd6d-9e4b-4b8b-8732-9f78f9a4e8c3") },
                    { new Guid("5b07c57d-4027-4558-99ed-1dd86b3b9994"), new Guid("dbf2f207-7edf-4642-87d7-bcc4ee8bf8e1") },
                    { new Guid("76c3a88e-3635-426a-b626-d51942ec6678"), new Guid("e656b89d-522b-49bc-9105-9fb261b9fd48") },
                    { new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5"), new Guid("e6692d04-8556-4659-accd-341a187d1632") },
                    { new Guid("d495519d-3230-40d9-9a79-e767b7c78bc5"), new Guid("e7b6d0ef-340b-47bb-b5c1-71bcdfe18e98") },
                    { new Guid("42af02c6-970d-4768-aa3a-b321d4406837"), new Guid("f60793df-4d93-4823-84ab-23699848b154") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_ProductId",
                table: "Discounts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategoryMappings_CategoryId",
                table: "ProductCategoryMappings",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "ProductCategoryMappings");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
