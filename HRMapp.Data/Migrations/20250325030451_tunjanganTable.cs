using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMapp.Data.Migrations
{
    /// <inheritdoc />
    public partial class tunjanganTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tunjangan",
                columns: table => new
                {
                    tunjangan_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tunjangan_name = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<int>(type: "int", nullable: true),
                    contract_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tunjangan", x => x.tunjangan_id);
                    table.ForeignKey(
                        name: "FK_Tunjangan_Contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "Contracts",
                        principalColumn: "contract_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tunjangan_contract_id",
                table: "Tunjangan",
                column: "contract_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tunjangan");
        }
    }
}
