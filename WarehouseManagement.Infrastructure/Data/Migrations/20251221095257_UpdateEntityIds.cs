using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProduct_Categories_CategoriesCategoryId",
                table: "CategoryProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProduct_Products_ProductsProductId",
                table: "CategoryProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Users_UserId",
                table: "Warehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Warehouses_WarehouseId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactionDetails_Products_ProductId",
                table: "WarehouseTransactionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Products_ProductId",
                table: "ProductSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Suppliers_SupplierId",
                table: "ProductSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactionDetails_WarehouseTransactions_WarehouseTransactionId",
                table: "WarehouseTransactionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactions_Users_CreatedBy",
                table: "WarehouseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactions_Users_ApprovedBy",
                table: "WarehouseTransactions");

            migrationBuilder.DropPrimaryKey("PK_Users", "Users");
            migrationBuilder.DropPrimaryKey("PK_Suppliers", "Suppliers");
            migrationBuilder.DropPrimaryKey("PK_Products", "Products");
            migrationBuilder.DropPrimaryKey("PK_Categories", "Categories");
            migrationBuilder.DropPrimaryKey("PK_Warehouses", "Warehouses");
            migrationBuilder.DropPrimaryKey("PK_Stocks", "Stocks");
            migrationBuilder.DropPrimaryKey("PK_WarehouseTransactions", "WarehouseTransactions");
            migrationBuilder.DropPrimaryKey("PK_WarehouseTransactionDetails", "WarehouseTransactionDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSuppliers",
                table: "ProductSuppliers");


            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "WarehouseTransactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WarehouseTransactionDetailId",
                table: "WarehouseTransactionDetails",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "Warehouses",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Suppliers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "Stocks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductsProductId",
                table: "CategoryProduct",
                newName: "ProductsId");

            migrationBuilder.RenameColumn(
                name: "CategoriesCategoryId",
                table: "CategoryProduct",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryProduct_ProductsProductId",
                table: "CategoryProduct",
                newName: "IX_CategoryProduct_ProductsId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "Id");

            migrationBuilder.AddPrimaryKey("PK_Users", "Users", "Id");
            migrationBuilder.AddPrimaryKey("PK_Suppliers", "Suppliers", "Id");
            migrationBuilder.AddPrimaryKey("PK_Products", "Products", "Id");
            migrationBuilder.AddPrimaryKey("PK_Categories", "Categories", "Id");
            migrationBuilder.AddPrimaryKey("PK_Warehouses", "Warehouses", "Id");
            migrationBuilder.AddPrimaryKey("PK_Stocks", "Stocks", "Id");
            migrationBuilder.AddPrimaryKey("PK_WarehouseTransactions", "WarehouseTransactions", "Id");
            migrationBuilder.AddPrimaryKey("PK_WarehouseTransactionDetails", "WarehouseTransactionDetails", "Id");


            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ProductSuppliers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSuppliers",
                table: "ProductSuppliers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuppliers_ProductId_SupplierId",
                table: "ProductSuppliers",
                columns: ["ProductId", "SupplierId"],
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProduct_Categories_CategoriesId",
                table: "CategoryProduct",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProduct_Products_ProductsId",
                table: "CategoryProduct",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Products_ProductId",
                table: "ProductSuppliers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Suppliers_SupplierId",
                table: "ProductSuppliers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Users_UserId",
                table: "Warehouses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Warehouses_WarehouseId",
                table: "Stocks",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactionDetails_Products_ProductId",
                table: "WarehouseTransactionDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactionDetails_WarehouseTransactions_WarehouseTransactionId",
                table: "WarehouseTransactionDetails",
                column: "WarehouseTransactionId",
                principalTable: "WarehouseTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactions_Users_CreatedBy",
                table: "WarehouseTransactions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactions_Users_ApprovedBy",
                table: "WarehouseTransactions",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_CategoryProduct_Categories_CategoriesId", "CategoryProduct");
            migrationBuilder.DropForeignKey("FK_CategoryProduct_Products_ProductsId", "CategoryProduct");

            migrationBuilder.DropForeignKey("FK_ProductSuppliers_Products_ProductId", "ProductSuppliers");
            migrationBuilder.DropForeignKey("FK_ProductSuppliers_Suppliers_SupplierId", "ProductSuppliers");

            migrationBuilder.DropForeignKey("FK_Warehouses_Users_UserId", "Warehouses");
            migrationBuilder.DropForeignKey("FK_Stocks_Warehouses_WarehouseId", "Stocks");
            migrationBuilder.DropForeignKey("FK_Stocks_Products_ProductId", "Stocks");

            migrationBuilder.DropForeignKey("FK_WarehouseTransactionDetails_Products_ProductId", "WarehouseTransactionDetails");
            migrationBuilder.DropForeignKey("FK_WarehouseTransactionDetails_WarehouseTransactions_WarehouseTransactionId", "WarehouseTransactionDetails");

            migrationBuilder.DropForeignKey("FK_AuditLogs_Users_UserId", "AuditLogs");

            migrationBuilder.DropForeignKey("FK_WarehouseTransactions_Users_CreatedBy", "WarehouseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransactions_Users_ApprovedBy",
                table: "WarehouseTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ProductSuppliers_ProductId_SupplierId",
                table: "ProductSuppliers");

            migrationBuilder.DropPrimaryKey("PK_ProductSuppliers", "ProductSuppliers");

            migrationBuilder.DropPrimaryKey("PK_Users", "Users");
            migrationBuilder.DropPrimaryKey("PK_Suppliers", "Suppliers");
            migrationBuilder.DropPrimaryKey("PK_Products", "Products");
            migrationBuilder.DropPrimaryKey("PK_Categories", "Categories");
            migrationBuilder.DropPrimaryKey("PK_Warehouses", "Warehouses");
            migrationBuilder.DropPrimaryKey("PK_Stocks", "Stocks");
            migrationBuilder.DropPrimaryKey("PK_WarehouseTransactions", "WarehouseTransactions");
            migrationBuilder.DropPrimaryKey("PK_WarehouseTransactionDetails", "WarehouseTransactionDetails");
                        
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductSuppliers");
                        
            migrationBuilder.RenameColumn("Id", "WarehouseTransactions", "TransactionId");
            migrationBuilder.RenameColumn("Id", "WarehouseTransactionDetails", "WarehouseTransactionDetailId");
            migrationBuilder.RenameColumn("Id", "Warehouses", "WarehouseId");
            migrationBuilder.RenameColumn("Id", "Users", "UserId");
            migrationBuilder.RenameColumn("Id", "Suppliers", "SupplierId");
            migrationBuilder.RenameColumn("Id", "Stocks", "StockId");
            migrationBuilder.RenameColumn("Id", "Products", "ProductId");
            migrationBuilder.RenameColumn("Id", "Categories", "CategoryId");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "CategoryProduct",
                newName: "ProductsProductId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryProduct",
                newName: "CategoriesCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryProduct_ProductsId",
                table: "CategoryProduct",
                newName: "IX_CategoryProduct_ProductsProductId");

            
            migrationBuilder.AddPrimaryKey("PK_Users", "Users", "UserId");
            migrationBuilder.AddPrimaryKey("PK_Suppliers", "Suppliers", "SupplierId");
            migrationBuilder.AddPrimaryKey("PK_Products", "Products", "ProductId");
            migrationBuilder.AddPrimaryKey("PK_Categories", "Categories", "CategoryId");
            migrationBuilder.AddPrimaryKey("PK_Warehouses", "Warehouses", "WarehouseId");
            migrationBuilder.AddPrimaryKey("PK_Stocks", "Stocks", "StockId");
            migrationBuilder.AddPrimaryKey("PK_WarehouseTransactions", "WarehouseTransactions", "TransactionId");
            migrationBuilder.AddPrimaryKey("PK_WarehouseTransactionDetails", "WarehouseTransactionDetails", "WarehouseTransactionDetailId");

            
            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProduct_Categories_CategoriesCategoryId",
                table: "CategoryProduct",
                column: "CategoriesCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProduct_Products_ProductsProductId",
                table: "CategoryProduct",
                column: "ProductsProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Users_UserId",
                table: "Warehouses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Warehouses_WarehouseId",
                table: "Stocks",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Products_ProductId",
                table: "Stocks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactionDetails_Products_ProductId",
                table: "WarehouseTransactionDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactionDetails_WarehouseTransactions_WarehouseTransactionId",
                table: "WarehouseTransactionDetails",
                column: "WarehouseTransactionId",
                principalTable: "WarehouseTransactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactions_Users_CreatedBy",
                table: "WarehouseTransactions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransactions_Users_ApprovedBy",
                table: "WarehouseTransactions",
                column: "ApprovedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
