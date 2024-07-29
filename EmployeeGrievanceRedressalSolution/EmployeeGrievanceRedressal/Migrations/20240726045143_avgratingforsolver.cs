using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeGrievanceRedressal.Migrations
{
    public partial class avgratingforsolver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Users",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Users");
        }
    }
}
