using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seller_Reference_Search.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesIndexUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Search_Sales_ParcelNumber_LotAcreage_OfferPrice_County_State_ClosingDate",
                table: "Sales");

            migrationBuilder.CreateIndex(
                name: "Search_Sales_ParcelNumber_LotAcreage_OfferPrice_County_State_ClosingDate_FileUploadId",
                table: "Sales",
                columns: new[] { "ParcelNumber", "LotAcreage", "OfferPrice", "County", "State", "ClosingDate", "FileUploadId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Search_Sales_ParcelNumber_LotAcreage_OfferPrice_County_State_ClosingDate_FileUploadId",
                table: "Sales");

            migrationBuilder.CreateIndex(
                name: "Search_Sales_ParcelNumber_LotAcreage_OfferPrice_County_State_ClosingDate",
                table: "Sales",
                columns: new[] { "ParcelNumber", "LotAcreage", "OfferPrice", "County", "State", "ClosingDate" });
        }
    }
}
