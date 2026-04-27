using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class PatientEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Protocol = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    BloodType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "BirthDate", "BloodType", "CreatedAt", "Gender", "IsActive", "Name", "Phone", "Protocol", "Surname" },
                values: new object[] { 1, "BURSA, Türkiye", new DateTime(2001, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, new DateTime(2026, 4, 27, 21, 59, 52, 159, DateTimeKind.Local).AddTicks(9898), 2, true, "BATUHAN", "5378102935", 20261, "KÖSE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
