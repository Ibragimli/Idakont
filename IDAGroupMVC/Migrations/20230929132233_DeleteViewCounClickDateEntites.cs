using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDAGroupMVC.Migrations
{
    public partial class DeleteViewCounClickDateEntites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_ViewCounts_ViewCountId",
                table: "Companies");

            migrationBuilder.DropTable(
                name: "ClickDates");

            migrationBuilder.DropTable(
                name: "ViewCounts");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ViewCountId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ViewCountId",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Companies",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Website",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCountId",
                table: "Companies",
                type: "int",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ViewCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompany = table.Column<bool>(type: "bit", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewCounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClickDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ViewCountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClickDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClickDates_ViewCounts_ViewCountId",
                        column: x => x.ViewCountId,
                        principalTable: "ViewCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ViewCountId",
                table: "Companies",
                column: "ViewCountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClickDates_ViewCountId",
                table: "ClickDates",
                column: "ViewCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_ViewCounts_ViewCountId",
                table: "Companies",
                column: "ViewCountId",
                principalTable: "ViewCounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
