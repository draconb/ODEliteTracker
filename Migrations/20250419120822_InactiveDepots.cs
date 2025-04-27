using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODEliteTracker.Migrations
{
    /// <inheritdoc />
    public partial class InactiveDepots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InactiveDepots",
                columns: table => new
                {
                    MarketID = table.Column<long>(type: "INTEGER", nullable: false),
                    SystemAddress = table.Column<long>(type: "INTEGER", nullable: false),
                    StationName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InactiveDepots", x => new { x.MarketID, x.SystemAddress, x.StationName });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InactiveDepots");
        }
    }
}
