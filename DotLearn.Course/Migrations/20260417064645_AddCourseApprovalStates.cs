using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotLearn.Course.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseApprovalStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Courses");
        }
    }
}
