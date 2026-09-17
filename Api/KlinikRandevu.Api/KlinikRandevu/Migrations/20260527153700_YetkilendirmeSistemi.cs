using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KlinikRandevu.Migrations
{
    /// <inheritdoc />
    public partial class YetkilendirmeSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "yetkiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_yetkiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_yetkiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    YetkiId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_yetkiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_yetkiler_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_yetkiler_yetkiler_YetkiId",
                        column: x => x.YetkiId,
                        principalTable: "yetkiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 27, 18, 36, 59, 637, DateTimeKind.Local).AddTicks(8636));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 27, 18, 36, 59, 639, DateTimeKind.Local).AddTicks(4686));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 27, 18, 36, 59, 639, DateTimeKind.Local).AddTicks(4702));

            migrationBuilder.InsertData(
                table: "yetkiler",
                columns: new[] { "Id", "Ad", "Kod" },
                values: new object[,]
                {
                    { 1, "Randevu Açma", "randevu_acma" },
                    { 2, "Muayene Açma", "muayene_acma" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_yetkiler_UserId_YetkiId",
                table: "user_yetkiler",
                columns: new[] { "UserId", "YetkiId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_yetkiler_YetkiId",
                table: "user_yetkiler",
                column: "YetkiId");

            migrationBuilder.CreateIndex(
                name: "IX_yetkiler_Kod",
                table: "yetkiler",
                column: "Kod",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_yetkiler");

            migrationBuilder.DropTable(
                name: "yetkiler");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 23, 18, 18, 27, 954, DateTimeKind.Local).AddTicks(247));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 1,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 23, 18, 18, 27, 954, DateTimeKind.Local).AddTicks(9850));

            migrationBuilder.UpdateData(
                table: "parametreler",
                keyColumn: "Id",
                keyValue: 2,
                column: "OlusturmaTarihi",
                value: new DateTime(2026, 5, 23, 18, 18, 27, 954, DateTimeKind.Local).AddTicks(9864));
        }
    }
}
