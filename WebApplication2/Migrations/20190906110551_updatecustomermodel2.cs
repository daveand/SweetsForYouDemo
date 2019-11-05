using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class updatecustomermodel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PeronalNumber",
                table: "CustomerModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Receiver",
                table: "CustomerModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeronalNumber",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "Receiver",
                table: "CustomerModel");
        }
    }
}
