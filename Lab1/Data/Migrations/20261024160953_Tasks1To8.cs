using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Tasks1To8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Contacts",
                newName: "LastName_Value");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Contacts",
                newName: "FirstName_Value");

            migrationBuilder.CreateTable(
                name: "ImportantDate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportantDate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportantDate_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phone_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phone_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportantDate_ContactId",
                table: "ImportantDate",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_CategoryId",
                table: "Phone",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_ContactId",
                table: "Phone",
                column: "ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email");

            migrationBuilder.DropTable(
                name: "ImportantDate");

            migrationBuilder.DropTable(
                name: "Phone");

            migrationBuilder.RenameColumn(
                name: "LastName_Value",
                table: "Contacts",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FirstName_Value",
                table: "Contacts",
                newName: "FirstName");

            migrationBuilder.AddForeignKey(
                name: "FK_Email_Categories_CategoryId",
                table: "Email",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
