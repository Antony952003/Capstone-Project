using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeGrievanceRedressal.Migrations
{
    public partial class ratingmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Grievances_GrievanceId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Users_SolverId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_SolverId",
                table: "Ratings",
                newName: "IX_Ratings_SolverId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_GrievanceId",
                table: "Ratings",
                newName: "IX_Ratings_GrievanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Grievances_GrievanceId",
                table: "Ratings",
                column: "GrievanceId",
                principalTable: "Grievances",
                principalColumn: "GrievanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Users_SolverId",
                table: "Ratings",
                column: "SolverId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Grievances_GrievanceId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Users_SolverId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_SolverId",
                table: "Rating",
                newName: "IX_Rating_SolverId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_GrievanceId",
                table: "Rating",
                newName: "IX_Rating_GrievanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Grievances_GrievanceId",
                table: "Rating",
                column: "GrievanceId",
                principalTable: "Grievances",
                principalColumn: "GrievanceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Users_SolverId",
                table: "Rating",
                column: "SolverId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
