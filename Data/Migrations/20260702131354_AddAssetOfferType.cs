using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameVault.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetOfferType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OfferType",
                table: "Assets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TradeWants",
                table: "Assets",
                type: "TEXT",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferType",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TradeWants",
                table: "Assets");
        }
    }
}
