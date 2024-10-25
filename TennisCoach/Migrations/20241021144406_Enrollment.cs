using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisCoach.Migrations
{
    /// <inheritdoc />
    public partial class Enrollment : Migration
    {
        public object Member { get; internal set; }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "eventname",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "EnrollmentId",
                table: "Enrollments",
                newName: "EnrollId");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "EnrollId",
                table: "Enrollments",
                newName: "EnrollmentId");

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "eventname",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentDate",
                table: "Enrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
