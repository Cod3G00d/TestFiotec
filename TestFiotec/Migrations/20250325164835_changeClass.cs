using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestFiotec.Migrations
{
    /// <inheritdoc />
    public partial class changeClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DadosEpidemiologicos_Solicitantes_SolicitanteId",
                table: "DadosEpidemiologicos");

            migrationBuilder.DropIndex(
                name: "IX_DadosEpidemiologicos_SolicitanteId",
                table: "DadosEpidemiologicos");

            migrationBuilder.DropColumn(
                name: "SolicitanteId",
                table: "DadosEpidemiologicos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolicitanteId",
                table: "DadosEpidemiologicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DadosEpidemiologicos_SolicitanteId",
                table: "DadosEpidemiologicos",
                column: "SolicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_DadosEpidemiologicos_Solicitantes_SolicitanteId",
                table: "DadosEpidemiologicos",
                column: "SolicitanteId",
                principalTable: "Solicitantes",
                principalColumn: "Id");
        }
    }
}
