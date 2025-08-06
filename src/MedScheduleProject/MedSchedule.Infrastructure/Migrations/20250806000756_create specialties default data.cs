using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedSchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createspecialtiesdefaultdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "Staffs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "AvgConsultationTime", "CreatedAt", "MinEmergencySlots", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44742950-2177-4d89-b48c-7e9d5790b401"), 30, new DateTime(2025, 8, 6, 0, 7, 56, 97, DateTimeKind.Utc).AddTicks(4363), 4, "Ophthalmology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("4501be09-12b7-4b6b-901c-43cb364b7db6"), 30, new DateTime(2025, 8, 6, 0, 7, 56, 97, DateTimeKind.Utc).AddTicks(4348), 3, "Endocrinology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("c1e52720-3ef9-47bb-bca4-1220b180d30d"), 15, new DateTime(2025, 8, 6, 0, 7, 56, 97, DateTimeKind.Utc).AddTicks(4295), 4, "Pediatrics", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("db4cba6b-92c2-438f-9a94-867e0f81794b"), 20, new DateTime(2025, 8, 6, 0, 7, 56, 97, DateTimeKind.Utc).AddTicks(3673), 2, "Cardiology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f58412ac-3d49-49cd-9961-3ff854b662e5"), 90, new DateTime(2025, 8, 6, 0, 7, 56, 97, DateTimeKind.Utc).AddTicks(4346), 3, "Oncology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("f6074c5a-b6dc-4301-95c1-10559c57863a"), 20, new DateTime(2025, 8, 6, 0, 7, 56, 97, DateTimeKind.Utc).AddTicks(4349), 4, "Orthopedics", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_SpecialtyId",
                table: "Staffs",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SpecialtyId",
                table: "Appointments",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Specialties_SpecialtyId",
                table: "Appointments",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Specialties_SpecialtyId",
                table: "Staffs",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Specialties_SpecialtyId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Specialties_SpecialtyId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_SpecialtyId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SpecialtyId",
                table: "Appointments");

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("44742950-2177-4d89-b48c-7e9d5790b401"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("4501be09-12b7-4b6b-901c-43cb364b7db6"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("c1e52720-3ef9-47bb-bca4-1220b180d30d"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("db4cba6b-92c2-438f-9a94-867e0f81794b"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("f58412ac-3d49-49cd-9961-3ff854b662e5"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("f6074c5a-b6dc-4301-95c1-10559c57863a"));

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Appointments");
        }
    }
}
