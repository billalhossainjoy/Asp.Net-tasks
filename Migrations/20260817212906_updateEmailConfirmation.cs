using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.Net_tasks.Migrations
{
    /// <inheritdoc />
    public partial class updateEmailConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EmailConfirmationTokenExpiresAtUtc",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationTokenExpiresAtUtc",
                table: "Users");
        }
    }
}
