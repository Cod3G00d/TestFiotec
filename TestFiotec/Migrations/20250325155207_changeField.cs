using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestFiotec.Migrations
{
    /// <inheritdoc />
    public partial class changeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DadosEpidemiologicos",
                newName: "DadosEpidemiologicosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DadosEpidemiologicosId",
                table: "DadosEpidemiologicos",
                newName: "Id");
        }
    }
}
