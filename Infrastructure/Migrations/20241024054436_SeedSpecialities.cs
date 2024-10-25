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
                table: "specialities",
                columns: new[] {"id", "create_time", "name" }, 
                values: new object[,]
                {
                    { Guid.NewGuid(), DateTime.UtcNow, "Кардиология" },
                    { Guid.NewGuid(),DateTime.UtcNow, "Хирургия" },
                    { Guid.NewGuid(), DateTime.UtcNow, "Педиатрия" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "specialities",
                keyColumn: "name",
                keyValues: new object[]
                {
                    "Кардиология",
                    "Хирургия",
                    "Педиатрия"
                });
        }
    }
}
