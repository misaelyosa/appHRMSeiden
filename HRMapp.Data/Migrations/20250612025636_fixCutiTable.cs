using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixCutiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "yearly_cuti_left",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cuti_day_count",
                table: "Cuti",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "cuti_end_date",
                table: "Cuti",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "cuti_start_date",
                table: "Cuti",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "Cuti",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "yearly_cuti_left",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "cuti_day_count",
                table: "Cuti");

            migrationBuilder.DropColumn(
                name: "cuti_end_date",
                table: "Cuti");

            migrationBuilder.DropColumn(
                name: "cuti_start_date",
                table: "Cuti");

            migrationBuilder.DropColumn(
                name: "reason",
                table: "Cuti");
        }
    }
}
