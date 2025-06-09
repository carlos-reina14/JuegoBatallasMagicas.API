using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JuegoBatallasMagicas.API.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habilidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    CosteMana = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoEfecto = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorEfecto = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    VidaMaxima = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    Fuerza = table.Column<int>(type: "INTEGER", nullable: false),
                    Inteligencia = table.Column<int>(type: "INTEGER", nullable: false),
                    VidaActual = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaActual = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personajes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Combates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Personaje1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Personaje2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    GanadorId = table.Column<int>(type: "INTEGER", nullable: true),
                    FechaCombate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Combates_Personajes_GanadorId",
                        column: x => x.GanadorId,
                        principalTable: "Personajes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Combates_Personajes_Personaje1Id",
                        column: x => x.Personaje1Id,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Combates_Personajes_Personaje2Id",
                        column: x => x.Personaje2Id,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonajeHabilidades",
                columns: table => new
                {
                    PersonajeId = table.Column<int>(type: "INTEGER", nullable: false),
                    HabilidadId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonajeHabilidades", x => new { x.PersonajeId, x.HabilidadId });
                    table.ForeignKey(
                        name: "FK_PersonajeHabilidades_Habilidades_HabilidadId",
                        column: x => x.HabilidadId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonajeHabilidades_Personajes_PersonajeId",
                        column: x => x.PersonajeId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionesCombate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CombateId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroTurno = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonajeAtacanteId = table.Column<int>(type: "INTEGER", nullable: false),
                    HabilidadUsadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonajeObjetivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DanoInfligido = table.Column<int>(type: "INTEGER", nullable: false),
                    CuracionAplicada = table.Column<int>(type: "INTEGER", nullable: false),
                    EscudoAplicado = table.Column<int>(type: "INTEGER", nullable: false),
                    VidaRestanteAtacante = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaRestanteAtacante = table.Column<int>(type: "INTEGER", nullable: false),
                    VidaRestanteObjetivo = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaRestanteObjetivo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesCombate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccionesCombate_Combates_CombateId",
                        column: x => x.CombateId,
                        principalTable: "Combates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccionesCombate_Habilidades_HabilidadUsadaId",
                        column: x => x.HabilidadUsadaId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccionesCombate_Personajes_PersonajeAtacanteId",
                        column: x => x.PersonajeAtacanteId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccionesCombate_Personajes_PersonajeObjetivoId",
                        column: x => x.PersonajeObjetivoId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccionesCombate_CombateId",
                table: "AccionesCombate",
                column: "CombateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccionesCombate_HabilidadUsadaId",
                table: "AccionesCombate",
                column: "HabilidadUsadaId");

            migrationBuilder.CreateIndex(
                name: "IX_AccionesCombate_PersonajeAtacanteId",
                table: "AccionesCombate",
                column: "PersonajeAtacanteId");

            migrationBuilder.CreateIndex(
                name: "IX_AccionesCombate_PersonajeObjetivoId",
                table: "AccionesCombate",
                column: "PersonajeObjetivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Combates_GanadorId",
                table: "Combates",
                column: "GanadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Combates_Personaje1Id",
                table: "Combates",
                column: "Personaje1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Combates_Personaje2Id",
                table: "Combates",
                column: "Personaje2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PersonajeHabilidades_HabilidadId",
                table: "PersonajeHabilidades",
                column: "HabilidadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionesCombate");

            migrationBuilder.DropTable(
                name: "PersonajeHabilidades");

            migrationBuilder.DropTable(
                name: "Combates");

            migrationBuilder.DropTable(
                name: "Habilidades");

            migrationBuilder.DropTable(
                name: "Personajes");
        }
    }
}
