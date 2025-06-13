using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCutiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cuti",
                columns: table => new
                {
                    cuti_id = table.Column<int>(type: "int", nullable: false),
                    employee_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuti", x => x.cuti_id);
                    table.ForeignKey(
                        name: "FK_Cuti_Employees_cuti_id",
                        column: x => x.cuti_id,
                        principalTable: "Employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cuti");
        }
    }
}
