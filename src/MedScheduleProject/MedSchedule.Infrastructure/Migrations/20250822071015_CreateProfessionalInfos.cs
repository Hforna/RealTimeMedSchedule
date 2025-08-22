using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedSchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateProfessionalInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Staffs_StaffId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Specialties_SpecialtyId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_SpecialtyId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_StaffId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AvgConsultationTime",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "MaxPriorityLevel",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "TotalServices",
                table: "Staffs");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionalInfosId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProfessionalsInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TotalServices = table.Column<int>(type: "int", nullable: false),
                    AvgConsultationTime = table.Column<int>(type: "int", nullable: true),
                    MaxPriorityLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalsInfos_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfessionalsInfos_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ProfessionalInfosId",
                table: "Appointments",
                column: "ProfessionalInfosId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalsInfos_SpecialtyId",
                table: "ProfessionalsInfos",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalsInfos_StaffId",
                table: "ProfessionalsInfos",
                column: "StaffId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_ProfessionalsInfos_ProfessionalInfosId",
                table: "Appointments",
                column: "ProfessionalInfosId",
                principalTable: "ProfessionalsInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_ProfessionalsInfos_ProfessionalInfosId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "ProfessionalsInfos");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ProfessionalInfosId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ProfessionalInfosId",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "AvgConsultationTime",
                table: "Staffs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPriorityLevel",
                table: "Staffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "Staffs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalServices",
                table: "Staffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_SpecialtyId",
                table: "Staffs",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StaffId",
                table: "Appointments",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Staffs_StaffId",
                table: "Appointments",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Specialties_SpecialtyId",
                table: "Staffs",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id");
        }
    }
}
