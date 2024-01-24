using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngularAythAPI.Migrations
{
    public partial class keys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionMembers_Users_MembersId",
                table: "CommissionMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionMembers",
                table: "CommissionMembers");

            migrationBuilder.DropIndex(
                name: "IX_CommissionMembers_MembersId",
                table: "CommissionMembers");

            migrationBuilder.RenameColumn(
                name: "MembersId",
                table: "CommissionMembers",
                newName: "CommissionMembersId");

            migrationBuilder.AddColumn<int>(
                name: "CommissionId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionMembers",
                table: "CommissionMembers",
                columns: new[] { "CommissionMembersId", "CommissionsId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionMembers_CommissionsId",
                table: "CommissionMembers",
                column: "CommissionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionMembers_Users_CommissionMembersId",
                table: "CommissionMembers",
                column: "CommissionMembersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionMembers_Users_CommissionMembersId",
                table: "CommissionMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionMembers",
                table: "CommissionMembers");

            migrationBuilder.DropIndex(
                name: "IX_CommissionMembers_CommissionsId",
                table: "CommissionMembers");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "CommissionMembersId",
                table: "CommissionMembers",
                newName: "MembersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionMembers",
                table: "CommissionMembers",
                columns: new[] { "CommissionsId", "MembersId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionMembers_MembersId",
                table: "CommissionMembers",
                column: "MembersId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionMembers_Users_MembersId",
                table: "CommissionMembers",
                column: "MembersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
