using Microsoft.EntityFrameworkCore.Migrations;

namespace IDAGroupMVC.Migrations
{
    public partial class CompanyWebsiteTableDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Companies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Companies",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
