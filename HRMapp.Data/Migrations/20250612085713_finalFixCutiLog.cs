using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class finalFixCutiLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cuti_Employees_cuti_id",
                table: "Cuti");

            migrationBuilder.AlterColumn<int>(
                name: "cuti_id",
                table: "Cuti",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Cuti_employee_id",
                table: "Cuti",
                column: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cuti_Employees_employee_id",
                table: "Cuti",
                column: "employee_id",
                principalTable: "Employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cuti_Employees_employee_id",
                table: "Cuti");

            migrationBuilder.DropIndex(
                name: "IX_Cuti_employee_id",
                table: "Cuti");

            migrationBuilder.AlterColumn<int>(
                name: "cuti_id",
                table: "Cuti",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Cuti_Employees_cuti_id",
                table: "Cuti",
                column: "cuti_id",
                principalTable: "Employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
