using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpeedTestResults",
                columns: table => new
                {
                    SpeedTestResultId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Download = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    Ping = table.Column<decimal>(type: "decimal(8,3)", nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Upload = table.Column<decimal>(type: "decimal(8,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeedTestResults", x => x.SpeedTestResultId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeedTestResults");
        }
    }
}
