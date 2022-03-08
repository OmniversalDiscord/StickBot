using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StickBot.Migrations.SqliteMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BonkMin = table.Column<long>(type: "INTEGER", nullable: false),
                    BonkMax = table.Column<long>(type: "INTEGER", nullable: false),
                    StealSuccessChance = table.Column<float>(type: "REAL", nullable: false),
                    StealBonkChance = table.Column<float>(type: "REAL", nullable: false),
                    StealCooldown = table.Column<long>(type: "INTEGER", nullable: false),
                    StickRole = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BonkedRole = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingsId);
                });

            migrationBuilder.CreateTable(
                name: "StealAttempts",
                columns: table => new
                {
                    StealAttemptId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StealAttempts", x => x.StealAttemptId);
                });

            migrationBuilder.CreateTable(
                name: "Sticks",
                columns: table => new
                {
                    StickId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BonkDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoldingUserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sticks", x => x.StickId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "StealAttempts");

            migrationBuilder.DropTable(
                name: "Sticks");
        }
    }
}
