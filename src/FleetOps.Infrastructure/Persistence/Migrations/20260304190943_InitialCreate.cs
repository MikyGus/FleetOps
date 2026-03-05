using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetOps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignments", x => x.id);
                });

                migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS btree_gist;");

                migrationBuilder.Sql(@"
                ALTER TABLE assignments
                ADD CONSTRAINT ck_assignments_time CHECK (end_utc > start_utc);
                ");

                migrationBuilder.Sql(@"
                ALTER TABLE assignments
                ADD CONSTRAINT ex_assignments_driver_no_overlap
                EXCLUDE USING gist (
                    driver_id WITH =,
                    tstzrange(start_utc, end_utc, '[)') WITH &&
                );
                ");

                migrationBuilder.Sql(@"
                ALTER TABLE assignments
                ADD CONSTRAINT ex_assignments_vehicle_no_overlap
                EXCLUDE USING gist (
                    vehicle_id WITH =,
                    tstzrange(start_utc, end_utc, '[)') WITH &&
                );
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE assignments DROP CONSTRAINT IF EXISTS ex_assignments_vehicle_no_overlap;"); 
            migrationBuilder.Sql(@"ALTER TABLE assignments DROP CONSTRAINT IF EXISTS ex_assignments_driver_no_overlap;"); 
            migrationBuilder.Sql(@"ALTER TABLE assignments DROP CONSTRAINT IF EXISTS ck_assignments_time;");

            migrationBuilder.DropTable(
                name: "assignments");
        }
    }
}
