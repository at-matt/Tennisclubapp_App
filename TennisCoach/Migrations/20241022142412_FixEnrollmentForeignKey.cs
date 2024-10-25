using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisCoach.Migrations
{
    /// <inheritdoc />
    public partial class FixEnrollmentForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SchedulesScheduleId",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_SchedulesScheduleId",
                table: "Enrollments",
                column: "SchedulesScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Schedules_SchedulesScheduleId",
                table: "Enrollments",
                column: "SchedulesScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Schedules_SchedulesScheduleId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_SchedulesScheduleId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "SchedulesScheduleId",
                table: "Enrollments");
        }
    }
}
