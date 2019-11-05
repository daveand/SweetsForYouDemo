using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class updatecustomermodel3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PeronalNumber",
                table: "CustomerModel",
                newName: "PersonalNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PersonalNumber",
                table: "CustomerModel",
                newName: "PeronalNumber");
        }
    }
}
