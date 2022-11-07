using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonkService.Infrastructure.Migrations
{
    public partial class UsernameRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Honks",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Honks",
                newName: "Username");
        }
    }
}
