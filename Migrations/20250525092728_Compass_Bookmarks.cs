using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODEliteTracker.Migrations
{
    /// <inheritdoc />
    public partial class Compass_Bookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemBookmarks",
                columns: table => new
                {
                    Address = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    X = table.Column<double>(type: "REAL", nullable: false),
                    Y = table.Column<double>(type: "REAL", nullable: false),
                    Z = table.Column<double>(type: "REAL", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemBookmarks", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "BookMarkDTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BodyId = table.Column<long>(type: "INTEGER", nullable: false),
                    BodyName = table.Column<string>(type: "TEXT", nullable: false),
                    BodyNameLocal = table.Column<string>(type: "TEXT", nullable: false),
                    BookmarkName = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    SystemAddress = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookMarkDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAddress",
                        column: x => x.SystemAddress,
                        principalTable: "SystemBookmarks",
                        principalColumn: "Address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookMarkDTO_SystemAddress",
                table: "BookMarkDTO",
                column: "SystemAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookMarkDTO");

            migrationBuilder.DropTable(
                name: "SystemBookmarks");
        }
    }
}
