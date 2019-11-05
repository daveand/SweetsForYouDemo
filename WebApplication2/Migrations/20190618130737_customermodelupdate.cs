using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class customermodelupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CustomerModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "CustomerModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "CustomerModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "CustomerModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "CustomerModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "CustomerModel");
        }
    }
}
