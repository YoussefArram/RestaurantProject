using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurrantProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    DailyOrderCount = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    Quanitity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "Pizza", null },
                    { 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "Sandwiches", null },
                    { 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "Meals", null }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "CategoryID", "CreatedAt", "DailyOrderCount", "Description", "IsAvailable", "IsDeleted", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Classic pizza with tomato sauce and mozzarella", true, false, "Margherita Pizza", 95.00m, null },
                    { 2, 1, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Pepperoni slices over cheesy crust", true, false, "Pepperoni Pizza", 110.00m, null },
                    { 3, 1, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Grilled chicken with BBQ sauce", true, false, "BBQ Chicken Pizza", 120.00m, null },
                    { 4, 1, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Blend of mozzarella, cheddar, parmesan, and blue cheese", true, false, "Four Cheese Pizza", 130.00m, null },
                    { 5, 1, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Loaded with fresh vegetables", true, false, "Veggie Pizza", 100.00m, null },
                    { 6, 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Juicy beef patty with cheese and lettuce", true, false, "Beef Burger", 90.00m, null },
                    { 7, 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Crispy chicken fillet with mayo", true, false, "Chicken Burger", 85.00m, null },
                    { 8, 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Triple-decker sandwich with turkey and egg", true, false, "Club Sandwich", 95.00m, null },
                    { 9, 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Tuna salad with lettuce and tomato", true, false, "Tuna Sandwich", 80.00m, null },
                    { 10, 2, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Traditional Egyptian falafel with tahini", true, false, "Falafel Sandwich", 50.00m, null },
                    { 11, 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Half grilled chicken with rice and salad", true, false, "Grilled Chicken Meal", 150.00m, null },
                    { 12, 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Tender beef steak with mashed potatoes", true, false, "Beef Steak Meal", 180.00m, null },
                    { 13, 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Grilled kofta with rice and salad", true, false, "Kofta Meal", 140.00m, null },
                    { 14, 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Crispy fried chicken pieces with fries", true, false, "Fried Chicken Meal", 130.00m, null },
                    { 15, 3, new DateTime(2025, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Breaded fish fillet with tartar sauce", true, false, "Fish Fillet Meal", 160.00m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryID",
                table: "Items",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ItemID",
                table: "OrderItems",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderID",
                table: "OrderItems",
                column: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
