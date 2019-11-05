using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class updatecustomermodelpayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayMethod",
                table: "CustomerModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayNumber",
                table: "CustomerModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayMethod",
                table: "CustomerModel");

            migrationBuilder.DropColumn(
                name: "PayNumber",
                table: "CustomerModel");
        }
    }
}
