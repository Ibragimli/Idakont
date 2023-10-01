using Microsoft.EntityFrameworkCore.Migrations;

namespace IDAGroupMVC.Migrations
{
    public partial class contactPhonenumberdelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Contacts");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Contacts",
                maxLength: 2500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1500)",
                oldMaxLength: 1500);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Contacts",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2500);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 70);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Contacts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
