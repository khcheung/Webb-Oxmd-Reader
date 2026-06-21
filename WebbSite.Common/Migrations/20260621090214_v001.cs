using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebbSite.Common.Migrations
{
    /// <inheritdoc />
    public partial class v001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockCode = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StockCCASS",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StockCode = table.Column<int>(type: "INTEGER", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TopFive = table.Column<decimal>(type: "TEXT", nullable: false),
                    TopTen = table.Column<decimal>(type: "TEXT", nullable: false),
                    TopTenNCIP = table.Column<decimal>(type: "TEXT", nullable: false),
                    StakeInCCASS = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCCASS", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "StockCCASS");
        }
    }
}
