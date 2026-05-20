using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeslandesApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExclusaoLogicaPecaCabivelLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "LOTE_TRABALHO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "LOTE_TRABALHO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "LOTE_TRABALHO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LOTETRABALHOID",
                table: "INTIMACAO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_LOTETRABALHOID",
                table: "INTIMACAO",
                column: "LOTETRABALHOID");

            migrationBuilder.AddForeignKey(
                name: "FK_INTIMACAO_LOTE_TRABALHO_LOTETRABALHOID",
                table: "INTIMACAO",
                column: "LOTETRABALHOID",
                principalTable: "LOTE_TRABALHO",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_INTIMACAO_LOTE_TRABALHO_LOTETRABALHOID",
                table: "INTIMACAO");

            migrationBuilder.DropIndex(
                name: "IX_INTIMACAO_LOTETRABALHOID",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "LOTE_TRABALHO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "LOTE_TRABALHO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "LOTE_TRABALHO");

            migrationBuilder.DropColumn(
                name: "LOTETRABALHOID",
                table: "INTIMACAO");
        }
    }
}
