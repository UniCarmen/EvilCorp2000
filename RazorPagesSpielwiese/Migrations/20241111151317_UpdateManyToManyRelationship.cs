using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RazorPagesSpielwiese.Migrations
{
    /// <inheritdoc />
    public partial class UpdateManyToManyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCategoryMappings");

            migrationBuilder.CreateTable(
                name: "CategoryProduct",
                columns: table => new
                {
                    CategoriesCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProduct", x => new { x.CategoriesCategoryId, x.ProductsProductId });
                    table.ForeignKey(
                        name: "FK_CategoryProduct_Category_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProduct_Products_ProductsProductId",
                        column: x => x.ProductsProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('42868247-7FA2-4FEA-A6EB-02EDB3D7CB2F', 'C3AD7A7D-B333-4F59-84C1-E1244EC0E3AA');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('0AC29D9A-3D3C-46E8-98B8-2174E50BD9E4', '023E69B2-204C-40A4-AECF-36AD94276DD5');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('0AC29D9A-3D3C-46E8-98B8-2174E50BD9E4', '42AF02C6-970D-4768-AA3A-B321D4406837');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('75EF12DE-AEB6-4B0F-8BF4-21E1E6A67B7B', '42AF02C6-970D-4768-AA3A-B321D4406837');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('F60793DF-4D93-4823-84AB-23699848B154', '42AF02C6-970D-4768-AA3A-B321D4406837');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('29D74C54-E267-4DDB-B156-29A40A2C9581', '5B07C57D-4027-4558-99ED-1DD86B3B9994');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('E6692D04-8556-4659-ACCD-341A187D1632', 'D495519D-3230-40D9-9A79-E767B7C78BC5');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('8AE8DC53-8AAF-4679-98D6-38349B0C5DE8', '42AF02C6-970D-4768-AA3A-B321D4406837');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('9C074B87-3109-401F-963B-3FD5B9449C26', '3BE3D730-B553-4179-ACA5-C3E107BDC2C0');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('B9D6A7B2-8E7F-4E13-9DCF-5731F080D612', '500A2093-5F38-4EF9-BF0D-63DC2497C144');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('E7B6D0EF-340B-47BB-B5C1-71BCDFE18E98', 'D495519D-3230-40D9-9A79-E767B7C78BC5');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('45C32195-F5BC-4B61-9680-7FA8BB8AB321', 'E506C42A-0D97-48EB-A274-743682031D1F');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('B4403AB3-C9A0-4BF2-B2B8-84420D5BEC37', 'D495519D-3230-40D9-9A79-E767B7C78BC5');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('03D6B8BF-CBDF-4D1E-84F6-87B9F4F2DE1A', 'ABA67ADF-210F-4073-97FB-992C88B61018');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('2B860DAE-C065-4F8F-A133-8BBF5C9A2DA4', '76C3A88E-3635-426A-B626-D51942EC6678');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('C1235C97-B12E-4E25-B67E-93F5097ECB6B', 'CD4EBEF9-4F75-4198-8771-9467BAD98394');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('0076132C-5A07-4446-BEE2-96BF3F097273', '5B07C57D-4027-4558-99ED-1DD86B3B9994');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('0076132C-5A07-4446-BEE2-96BF3F097273', 'A8B4DCB1-B9BE-45C9-AA74-6F88043F1CEB');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('D0D5FD6D-9E4B-4B8B-8732-9F78F9A4E8C3', '500A2093-5F38-4EF9-BF0D-63DC2497C144');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('E656B89D-522B-49BC-9105-9FB261B9FD48', '76C3A88E-3635-426A-B626-D51942EC6678');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('C56D7909-FCBC-4098-A404-A58016777765', '023E69B2-204C-40A4-AECF-36AD94276DD5');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('2ADCFF47-F0EE-46DA-B876-AE2E6803B8A8', 'A2B69BF2-9B88-49A1-B7F3-02078ACBBC27');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('DBF2F207-7EDF-4642-87D7-BCC4EE8BF8E1', '5B07C57D-4027-4558-99ED-1DD86B3B9994');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('2D821223-B7D5-494B-893B-D310B9266DEC', '500A2093-5F38-4EF9-BF0D-63DC2497C144');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('2D821223-B7D5-494B-893B-D310B9266DEC', 'C3AD7A7D-B333-4F59-84C1-E1244EC0E3AA');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('4F8391F1-0DFE-4B1B-B5D2-D9A4F7C1E921', 'A2B69BF2-9B88-49A1-B7F3-02078ACBBC27');
INSERT INTO CategoryProduct (ProductsProductId, CategoriesCategoryId) VALUES ('73565726-218F-4024-8DE1-E950917ED8AF', 'C3AD7A7D-B333-4F59-84C1-E1244EC0E3AA');
            ");

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: new Guid("dcfafad6-82b6-4063-b5ce-ea53d9149f77"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 11, 26, 16, 13, 16, 857, DateTimeKind.Local).AddTicks(4275), new DateTime(2024, 11, 11, 16, 13, 16, 857, DateTimeKind.Local).AddTicks(4274) });

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: new Guid("df3921aa-a5bd-43c1-9e03-4dba98bb6bc5"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 12, 1, 16, 13, 16, 857, DateTimeKind.Local).AddTicks(4270), new DateTime(2024, 11, 1, 16, 13, 16, 857, DateTimeKind.Local).AddTicks(4211) });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProduct_ProductsProductId",
                table: "CategoryProduct",
                column: "ProductsProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryProduct");

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

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: new Guid("dcfafad6-82b6-4063-b5ce-ea53d9149f77"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 11, 13, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1145), new DateTime(2024, 10, 29, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1144) });

            migrationBuilder.UpdateData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: new Guid("df3921aa-a5bd-43c1-9e03-4dba98bb6bc5"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 11, 18, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1140), new DateTime(2024, 10, 19, 11, 47, 47, 659, DateTimeKind.Local).AddTicks(1084) });

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
                name: "IX_ProductCategoryMappings_CategoryId",
                table: "ProductCategoryMappings",
                column: "CategoryId");
        }
    }
}
