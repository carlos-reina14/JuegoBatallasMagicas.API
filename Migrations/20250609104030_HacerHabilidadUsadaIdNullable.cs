﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JuegoBatallasMagicas.API.Migrations
{
    /// <inheritdoc />
    public partial class HacerHabilidadUsadaIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HabilidadUsadaId",
                table: "AccionesCombate",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HabilidadUsadaId",
                table: "AccionesCombate",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
