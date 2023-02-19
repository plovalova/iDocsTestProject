using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iDocsTestProject.Migrations
{
    public partial class AddDocumentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Number = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Number);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedDate_CreatedUserId",
                table: "Documents",
                columns: new[] { "CreatedDate", "CreatedUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedDate_ReceiverUserId",
                table: "Documents",
                columns: new[] { "CreatedDate", "ReceiverUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
