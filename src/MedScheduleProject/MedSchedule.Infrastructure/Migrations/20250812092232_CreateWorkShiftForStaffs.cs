using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedSchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorkShiftForStaffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.RenameColumn(
                name: "ScheduleWork_StartMinutes",
                table: "Staffs",
                newName: "WorkShift_StartMinutes");

            migrationBuilder.RenameColumn(
                name: "ScheduleWork_StartHours",
                table: "Staffs",
                newName: "WorkShift_StartHours");

            migrationBuilder.RenameColumn(
                name: "ScheduleWork_EndMinutes",
                table: "Staffs",
                newName: "WorkShift_EndMinutes");

            migrationBuilder.RenameColumn(
                name: "ScheduleWork_EndHours",
                table: "Staffs",
                newName: "WorkShift_EndHours");

            migrationBuilder.AddColumn<int>(
                name: "TotalPositions",
                table: "QueueRoots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDate",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Schedule_AppointmentDate",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "AvgConsultationTime", "CreatedAt", "MinEmergencySlots", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1126ccc3-2137-4a85-888d-b3ba4c73bf0f"), 30, new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2606), 4, "Ophthalmology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("3e6b5514-5ab9-4410-8a15-ebfaaca453b5"), 20, new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2602), 4, "Orthopedics", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("6797cbd8-4b25-4274-8bc6-3f1550917bbc"), 30, new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2589), 3, "Endocrinology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("7e8fc7a1-e211-42ea-9d7a-ae7166779c1f"), 90, new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2588), 3, "Oncology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("9353101a-ab97-4144-a312-516b8d2cac0a"), 15, new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(2584), 4, "Pediatrics", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("aa3c4f4a-d218-418e-858c-edc1720ecf59"), 20, new DateTime(2025, 8, 12, 9, 22, 31, 953, DateTimeKind.Utc).AddTicks(1967), 2, "Cardiology", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Staffs_StaffId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_StaffId",
                table: "Appointments");

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("1126ccc3-2137-4a85-888d-b3ba4c73bf0f"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("3e6b5514-5ab9-4410-8a15-ebfaaca453b5"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("6797cbd8-4b25-4274-8bc6-3f1550917bbc"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("7e8fc7a1-e211-42ea-9d7a-ae7166779c1f"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("9353101a-ab97-4144-a312-516b8d2cac0a"));

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: new Guid("aa3c4f4a-d218-418e-858c-edc1720ecf59"));

            migrationBuilder.DropColumn(
                name: "TotalPositions",
                table: "QueueRoots");

            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Schedule_AppointmentDate",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "WorkShift_StartMinutes",
                table: "Staffs",
                newName: "ScheduleWork_StartMinutes");

            migrationBuilder.RenameColumn(
                name: "WorkShift_StartHours",
                table: "Staffs",
                newName: "ScheduleWork_StartHours");

            migrationBuilder.RenameColumn(
                name: "WorkShift_EndMinutes",
                table: "Staffs",
                newName: "ScheduleWork_EndMinutes");

            migrationBuilder.RenameColumn(
                name: "WorkShift_EndHours",
                table: "Staffs",
                newName: "ScheduleWork_EndHours");

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
        }
    }
}
