using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestFiotec.Migrations
{
    /// <inheritdoc />
    public partial class firstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solicitantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solicitacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataSolicitacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Arbovirose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolicitanteId = table.Column<int>(type: "int", nullable: false),
                    SemanaInicio = table.Column<int>(type: "int", nullable: false),
                    SemanaFim = table.Column<int>(type: "int", nullable: false),
                    CodigoIBGE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Municipio = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitacoes_Solicitantes_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Solicitantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DadosEpidemiologicos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasProvEst = table.Column<double>(type: "float", nullable: true),
                    CasProvEstMin = table.Column<double>(type: "float", nullable: true),
                    CasProvEstMax = table.Column<double>(type: "float", nullable: true),
                    CasConf = table.Column<double>(type: "float", nullable: true),
                    NotifAccumYear = table.Column<double>(type: "float", nullable: true),
                    DataIniSE = table.Column<long>(type: "bigint", nullable: false),
                    SE = table.Column<double>(type: "float", nullable: true),
                    CasosEst = table.Column<double>(type: "float", nullable: true),
                    CasosEstMin = table.Column<double>(type: "float", nullable: true),
                    CasosEstMax = table.Column<double>(type: "float", nullable: true),
                    Casos = table.Column<double>(type: "float", nullable: true),
                    PRt1 = table.Column<double>(type: "float", nullable: true),
                    PInc100k = table.Column<double>(type: "float", nullable: true),
                    LocalidadeId = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<double>(type: "float", nullable: true),
                    IdConsulta = table.Column<long>(type: "bigint", nullable: false),
                    VersaoModelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tweet = table.Column<double>(type: "float", nullable: true),
                    Rt = table.Column<double>(type: "float", nullable: true),
                    Pop = table.Column<double>(type: "float", nullable: true),
                    TempMin = table.Column<double>(type: "float", nullable: true),
                    UmidMax = table.Column<double>(type: "float", nullable: true),
                    Receptivo = table.Column<double>(type: "float", nullable: true),
                    Transmissao = table.Column<double>(type: "float", nullable: true),
                    NivelInc = table.Column<double>(type: "float", nullable: true),
                    UmidMed = table.Column<double>(type: "float", nullable: true),
                    UmidMin = table.Column<double>(type: "float", nullable: true),
                    TempMed = table.Column<double>(type: "float", nullable: true),
                    TempMax = table.Column<double>(type: "float", nullable: true),
                    CasProv = table.Column<double>(type: "float", nullable: true),
                    SolicitacaoId = table.Column<int>(type: "int", nullable: true),
                    SolicitanteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosEpidemiologicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DadosEpidemiologicos_Solicitacoes_SolicitacaoId",
                        column: x => x.SolicitacaoId,
                        principalTable: "Solicitacoes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DadosEpidemiologicos_Solicitantes_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Solicitantes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DadosEpidemiologicos_SolicitacaoId",
                table: "DadosEpidemiologicos",
                column: "SolicitacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DadosEpidemiologicos_SolicitanteId",
                table: "DadosEpidemiologicos",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_SolicitanteId",
                table: "Solicitacoes",
                column: "SolicitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DadosEpidemiologicos");

            migrationBuilder.DropTable(
                name: "Solicitacoes");

            migrationBuilder.DropTable(
                name: "Solicitantes");
        }
    }
}
