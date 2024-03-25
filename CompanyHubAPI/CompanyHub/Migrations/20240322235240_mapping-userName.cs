using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyHub.Migrations
{
    /// <inheritdoc />
    public partial class mappinguserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ChatUserMapping",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ChatUserMapping");
        }
    }
}
