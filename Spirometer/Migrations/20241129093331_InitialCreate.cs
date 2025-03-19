using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spirometer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    CPRNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.CPRNumber);
                });

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FEV1 = table.Column<double>(type: "REAL", nullable: false),
                    FVC = table.Column<double>(type: "REAL", nullable: false),
                    Ratio = table.Column<double>(type: "REAL", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCritical = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSeen = table.Column<bool>(type: "INTEGER", nullable: false),
                    RawData = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CPRNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Data_Patients_CPRNumber",
                        column: x => x.CPRNumber,
                        principalTable: "Patients",
                        principalColumn: "CPRNumber");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Data_CPRNumber",
                table: "Data",
                column: "CPRNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
