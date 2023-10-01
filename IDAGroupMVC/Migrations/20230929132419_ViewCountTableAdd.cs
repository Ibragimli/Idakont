using Microsoft.EntityFrameworkCore.Migrations;

namespace IDAGroupMVC.Migrations
{
    public partial class ViewCountTableAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Companies",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Companies");
        }
    }
}
