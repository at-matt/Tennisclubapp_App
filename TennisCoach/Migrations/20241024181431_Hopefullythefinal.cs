using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisCoach.Migrations
{
    /// <inheritdoc />
    public partial class Hopefullythefinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Members_MemberId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "EnrollId",
                table: "Enrollments",
                newName: "EnrollmentId");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoachesCoachId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CoachesCoachId",
                table: "Schedules",
                column: "CoachesCoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Coaches_CoachesCoachId",
                table: "Schedules",
                column: "CoachesCoachId",
                principalTable: "Coaches",
                principalColumn: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Members_MemberId",
                table: "Schedules",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Coaches_CoachesCoachId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Members_MemberId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_CoachesCoachId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CoachesCoachId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "Enrollments",
                newName: "EnrollId");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Schedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Members_MemberId",
                table: "Schedules",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId");
        }
    }
}
