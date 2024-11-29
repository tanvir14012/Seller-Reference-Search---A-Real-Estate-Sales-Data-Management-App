using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Seller_Reference_Search.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class salesAndFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileSize = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UploadedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW()"),
                    UploadPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UploadDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileUploads_AspNetUsers_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParcelNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LotAcreage = table.Column<double>(type: "double precision", nullable: false),
                    OfferPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OfferPPA = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    RealPPA = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PPACalc = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Profit = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    RetailValue = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    County = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    State = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    FileUploadId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_FileUploads_FileUploadId",
                        column: x => x.FileUploadId,
                        principalTable: "FileUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_UploadedByUserId",
                table: "FileUploads",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_FileUploadId",
                table: "Sales",
                column: "FileUploadId");

            migrationBuilder.CreateIndex(
                name: "Search_Sales_ParcelNumber_LotAcreage_OfferPrice_County_State_ClosingDate",
                table: "Sales",
                columns: new[] { "ParcelNumber", "LotAcreage", "OfferPrice", "County", "State", "ClosingDate" });

            migrationBuilder.CreateIndex(
                name: "Uniq_Sales_Reference",
                table: "Sales",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "FileUploads");
        }
    }
}
