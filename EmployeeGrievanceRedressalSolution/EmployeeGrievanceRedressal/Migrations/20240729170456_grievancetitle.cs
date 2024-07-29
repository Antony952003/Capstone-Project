using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeGrievanceRedressal.Migrations
{
    public partial class grievancetitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Grievances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Grievances");
        }
    }
}
