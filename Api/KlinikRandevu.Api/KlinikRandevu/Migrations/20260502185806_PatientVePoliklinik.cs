using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class PatientVePoliklinik : Migration
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
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TcKimlik = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Poliklinikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolNo = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PolUzKod = table.Column<int>(type: "int", nullable: false),
                    DoktorNo = table.Column<int>(type: "int", nullable: false),
                    KatNo = table.Column<int>(type: "int", nullable: true),
                    OdaNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaxRandevuSuresi = table.Column<int>(type: "int", nullable: true),
                    GunlukMaksRandevuSayisi = table.Column<int>(type: "int", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OnlineRandevuAktif = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poliklinikler", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Address", "BirthDate", "BloodType", "CreatedAt", "Gender", "IsActive", "Name", "Phone", "Protocol", "Surname", "TcKimlik" },
                values: new object[] { 1, "BURSA, Türkiye", new DateTime(2001, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, new DateTime(2026, 5, 2, 21, 58, 6, 639, DateTimeKind.Local).AddTicks(4840), 2, true, "BATUHAN", "5378102935", 20261, "KÖSE", 0L });

            migrationBuilder.InsertData(
                table: "Poliklinikler",
                columns: new[] { "Id", "Aciklama", "DoktorNo", "GunlukMaksRandevuSayisi", "KatNo", "MaxRandevuSuresi", "Name", "OdaNo", "OnlineRandevuAktif", "PolNo", "PolUzKod", "Telefon", "isActive" },
                values: new object[] { 1, "İÇ Hastalıkları pol öğleden önce", 1, 30, 1, 10, "İÇ HASTALIKLARI HÜSEYİN AĞAC", "Birinci kat", true, 1, 1000, "1650", true });

            migrationBuilder.CreateIndex(
                name: "IX_Poliklinikler_PolNo",
                table: "Poliklinikler",
                column: "PolNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Poliklinikler");
        }
    }
}
