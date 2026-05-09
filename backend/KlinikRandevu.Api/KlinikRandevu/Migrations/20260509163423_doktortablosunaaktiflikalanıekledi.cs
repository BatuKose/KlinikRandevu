using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class doktortablosunaaktiflikalanıekledi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Doktorlar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Doktorlar",
                keyColumn: "Id",
                keyValue: 1,
                column: "isActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Doktorlar",
                keyColumn: "Id",
                keyValue: 2,
                column: "isActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Doktorlar",
                keyColumn: "Id",
                keyValue: 3,
                column: "isActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 9, 19, 34, 22, 844, DateTimeKind.Local).AddTicks(5178));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Doktorlar");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 7, 20, 56, 21, 657, DateTimeKind.Local).AddTicks(8934));
        }
    }
}
