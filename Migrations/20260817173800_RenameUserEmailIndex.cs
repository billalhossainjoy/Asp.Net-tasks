using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net_task3.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "Users_Email",
                table: "Users",
                newName: "Email_Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "Email_Users",
                table: "Users",
                newName: "Users_Email");
        }
    }
}
