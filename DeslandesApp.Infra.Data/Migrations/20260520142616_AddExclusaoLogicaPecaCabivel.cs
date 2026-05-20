using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeslandesApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExclusaoLogicaPecaCabivel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "PECA_CABIVEL",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "PECA_CABIVEL",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "PECA_CABIVEL",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "PECA_CABIVEL");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "PECA_CABIVEL");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "PECA_CABIVEL");
        }
    }
}
