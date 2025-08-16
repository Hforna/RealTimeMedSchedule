using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedSchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStaffTableForAllTheirServicesCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvgConsultationTime",
                table: "Staffs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalServices",
                table: "Staffs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgConsultationTime",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "TotalServices",
                table: "Staffs");
        }
    }
}
