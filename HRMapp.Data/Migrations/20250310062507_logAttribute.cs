using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class logAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "data_changed",
                table: "LogEmployees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_changed",
                table: "LogEmployees");
        }
    }
}
