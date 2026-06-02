using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_assignments_driver_id",
                table: "assignments",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_assignments_vehicle_id",
                table: "assignments",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "FK_assignments_drivers_driver_id",
                table: "assignments",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_assignments_vehicles_vehicle_id",
                table: "assignments",
                column: "vehicle_id",
                principalTable: "vehicles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_assignments_drivers_driver_id",
                table: "assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_assignments_vehicles_vehicle_id",
                table: "assignments");

            migrationBuilder.DropIndex(
                name: "IX_assignments_driver_id",
                table: "assignments");

            migrationBuilder.DropIndex(
                name: "IX_assignments_vehicle_id",
                table: "assignments");
        }
    }
}
