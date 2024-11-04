using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSpecialities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "speciality",
                columns: new[] { "id", "create_time", "name" },
                values: new object[,]
                {
                    { Guid.NewGuid(), DateTime.UtcNow, "Cardiology" },
                    { Guid.NewGuid(), DateTime.UtcNow, "Neurology" },
                    { Guid.NewGuid(), DateTime.UtcNow, "Orthopedics" }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "speciality",
                keyColumn: "name",
                keyValues: new object[]
                {
                    "Cardiology",
                    "Neurology",
                    "Orthopedics"
                }
            );
        }
    }
}
