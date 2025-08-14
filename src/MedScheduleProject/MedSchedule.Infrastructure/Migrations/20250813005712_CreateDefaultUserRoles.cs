using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedSchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateDefaultUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("05c33ca5-2edd-478c-8472-aa51608c610f"), null, "admin", "ADMIN" },
                    { new Guid("4a377ec7-d318-4e31-ae12-0342ab0f04d8"), null, "professional", "PROFESSIONAL" },
                    { new Guid("78ea5b7f-45ec-4e9b-9522-9526034fa3b9"), null, "patient", "PATIENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("05c33ca5-2edd-478c-8472-aa51608c610f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4a377ec7-d318-4e31-ae12-0342ab0f04d8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("78ea5b7f-45ec-4e9b-9522-9526034fa3b9"));
        }
    }
}
