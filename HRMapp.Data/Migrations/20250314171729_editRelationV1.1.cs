using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class editRelationV11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Factories_factory_id",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Departments_department_id",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_department_id",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Departments_factory_id",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "factory_id",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "marital_status",
                table: "Employees",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "graduation_date",
                table: "Employees",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "factory_id",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_factory_id",
                table: "Employees",
                column: "factory_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Factories_factory_id",
                table: "Employees",
                column: "factory_id",
                principalTable: "Factories",
                principalColumn: "factory_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Factories_factory_id",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_factory_id",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "factory_id",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "marital_status",
                keyValue: null,
                column: "marital_status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "marital_status",
                table: "Employees",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "graduation_date",
                table: "Employees",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "factory_id",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_department_id",
                table: "Jobs",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_factory_id",
                table: "Departments",
                column: "factory_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Factories_factory_id",
                table: "Departments",
                column: "factory_id",
                principalTable: "Factories",
                principalColumn: "factory_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Departments_department_id",
                table: "Jobs",
                column: "department_id",
                principalTable: "Departments",
                principalColumn: "department_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
