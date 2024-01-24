using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularAythAPI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionMembers",
                columns: table => new
                {
                    CommissionsId = table.Column<int>(type: "int", nullable: false),
                    MembersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionMembers", x => new { x.CommissionsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_CommissionMembers_Commisions_CommissionsId",
                        column: x => x.CommissionsId,
                        principalTable: "Commisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Labors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionId = table.Column<int>(type: "int", nullable: true),
                    ProfesorId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfSubmission = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfDefense = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labors_Commisions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "Commisions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ProfesorId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfesorId = table.Column<int>(type: "int", nullable: true),
                    StudentThemesId = table.Column<int>(type: "int", nullable: true),
                    StudentsLaborId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Labors_StudentsLaborId",
                        column: x => x.StudentsLaborId,
                        principalTable: "Labors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Themes_StudentThemesId",
                        column: x => x.StudentThemesId,
                        principalTable: "Themes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Users_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionMembers_MembersId",
                table: "CommissionMembers",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_Labors_CommissionId",
                table: "Labors",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Labors_ProfesorId",
                table: "Labors",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Labors_StudentId",
                table: "Labors",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_ProfesorId",
                table: "Themes",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_StudentId",
                table: "Themes",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_UserId",
                table: "Themes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfesorId",
                table: "Users",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentsLaborId",
                table: "Users",
                column: "StudentsLaborId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentThemesId",
                table: "Users",
                column: "StudentThemesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionMembers_Users_MembersId",
                table: "CommissionMembers",
                column: "MembersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labors_Users_ProfesorId",
                table: "Labors",
                column: "ProfesorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Labors_Users_StudentId",
                table: "Labors",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Themes_Users_ProfesorId",
                table: "Themes",
                column: "ProfesorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Themes_Users_StudentId",
                table: "Themes",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Themes_Users_UserId",
                table: "Themes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labors_Commisions_CommissionId",
                table: "Labors");

            migrationBuilder.DropForeignKey(
                name: "FK_Labors_Users_ProfesorId",
                table: "Labors");

            migrationBuilder.DropForeignKey(
                name: "FK_Labors_Users_StudentId",
                table: "Labors");

            migrationBuilder.DropForeignKey(
                name: "FK_Themes_Users_ProfesorId",
                table: "Themes");

            migrationBuilder.DropForeignKey(
                name: "FK_Themes_Users_StudentId",
                table: "Themes");

            migrationBuilder.DropForeignKey(
                name: "FK_Themes_Users_UserId",
                table: "Themes");

            migrationBuilder.DropTable(
                name: "CommissionMembers");

            migrationBuilder.DropTable(
                name: "Commisions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Labors");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
