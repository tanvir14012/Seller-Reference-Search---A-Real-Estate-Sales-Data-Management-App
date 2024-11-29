using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seller_Reference_Search.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FileUploadFieldChange2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowsFound",
                table: "FileUploads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RowsInserted",
                table: "FileUploads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RowsUpdated",
                table: "FileUploads",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowsFound",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "RowsInserted",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "RowsUpdated",
                table: "FileUploads");
        }
    }
}
