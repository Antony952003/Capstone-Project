using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeGrievanceRedressal.Migrations
{
    public partial class approvalrequestupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateRequested",
                table: "ApprovalRequests",
                newName: "RequestDate");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ApprovalRequests");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "ApprovalRequests",
                newName: "DateRequested");
        }
    }
}
