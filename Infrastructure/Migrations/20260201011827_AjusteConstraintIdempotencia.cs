using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.ContaCorrente.Migrations
{
    /// <inheritdoc />
    public partial class AjusteConstraintIdempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movimentos_ContaId",
                table: "Movimentos");

            migrationBuilder.DropIndex(
                name: "IX_Movimentos_RequestId",
                table: "Movimentos");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_ContaId_RequestId",
                table: "Movimentos",
                columns: new[] { "ContaId", "RequestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movimentos_ContaId_RequestId",
                table: "Movimentos");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_ContaId",
                table: "Movimentos",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_RequestId",
                table: "Movimentos",
                column: "RequestId",
                unique: true);
        }
    }
}
