using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Contacts",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "TEXT", nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Profile_Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Profile_DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Profile_FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Profile_LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId_Name",
                table: "Categories",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RefreshTokenHash",
                table: "Users",
                column: "RefreshTokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Users_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Users_UserId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
