using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeslandesApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "VARA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "VARA",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "VARA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "VARA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "VARA",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "USUARIOS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "USUARIOS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "USUARIOS",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "TAREFA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "TAREFA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "TAREFA",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "SETORES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "SETORES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "SETORES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "SETORES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "SETORES",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "QUALIFICACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "QUALIFICACAO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "QUALIFICACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "QUALIFICACAO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "QUALIFICACAO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "PROCESSOS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "PROCESSOS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "PROCESSOS",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "PESSOA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "PESSOA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "PESSOA",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "PECA_CABIVEL",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "PECA_CABIVEL",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "NIVEL",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "NIVEL",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "NIVEL",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "NIVEL",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "NIVEL",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "LOTE_TRABALHO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "LOTE_TRABALHO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "LOGINHISTORY",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "LOGINHISTORY",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "LOGINHISTORY",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "LOGINHISTORY",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "LOGINHISTORY",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "LISTATAREFA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "LISTATAREFA",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "LISTATAREFA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "LISTATAREFA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "LISTATAREFA",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "INTIMACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "INTIMACAO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "INTIMACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATATRIAGEM",
                table: "INTIMACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "INTIMACAO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "INTIMACAO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "INFORMACOESCOMPLEMENTARES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "INFORMACOESCOMPLEMENTARES",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "INFORMACOESCOMPLEMENTARES",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "INFORMACOESCOMPLEMENTARES",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "INFORMACOESCOMPLEMENTARES",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "HISTORICOGERAL",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "HISTORICOGERAL",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "HISTORICOGERAL",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "HISTORICOGERAL",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "HISTORICOGERAL",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "GRUPOCASOENVOLVIDO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "GRUPOCASOENVOLVIDO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "GRUPOCASOENVOLVIDO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "GRUPOCASOENVOLVIDO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "GRUPOCASOENVOLVIDO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "FOTOS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "FOTOS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "FOTOS",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "FORO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "FORO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "FORO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "FORO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "FORO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "FAILEDLOGINATTEMPTS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "FAILEDLOGINATTEMPTS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "FAILEDLOGINATTEMPTS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "FAILEDLOGINATTEMPTS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "FAILEDLOGINATTEMPTS",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "EVENTO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "EVENTO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "EVENTO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "ETIQUETA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "ETIQUETA",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "ETIQUETA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "ETIQUETA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "ETIQUETA",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "ENDERECO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "ENDERECO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "ENDERECO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "ENDERECO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "ENDERECO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "CONTABANCARIA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "CONTABANCARIA",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "CONTABANCARIA",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "CONTABANCARIA",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "CONTABANCARIA",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "COMENTARIOS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "COMENTARIOS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "COMENTARIOS",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "COMENTARIOS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "COMENTARIOS",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "CASO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "CASO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "CASO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "ATENDIMENTOHISTORICO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "ATENDIMENTOHISTORICO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "ATENDIMENTOHISTORICO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "ATENDIMENTOHISTORICO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "ATENDIMENTOHISTORICO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "ATENDIMENTO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "ATENDIMENTO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "ATENDIMENTO",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAATUALIZACAO",
                table: "ACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DATACADASTRO",
                table: "ACAO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DATAEXCLUSAO",
                table: "ACAO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EXCLUIDO",
                table: "ACAO",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "USUARIOEXCLUSAOID",
                table: "ACAO",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "VARA");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "VARA");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "VARA");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "VARA");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "VARA");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "USUARIOS");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "TAREFA");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "TAREFA");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "TAREFA");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "SETORES");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "SETORES");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "SETORES");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "SETORES");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "SETORES");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "QUALIFICACAO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "QUALIFICACAO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "QUALIFICACAO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "QUALIFICACAO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "QUALIFICACAO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "PROCESSOS");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "PROCESSOS");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "PROCESSOS");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "PESSOA");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "PESSOA");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "PESSOA");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "PECA_CABIVEL");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "PECA_CABIVEL");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "NIVEL");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "NIVEL");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "NIVEL");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "NIVEL");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "NIVEL");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "LOTE_TRABALHO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "LOTE_TRABALHO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "LOGINHISTORY");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "LOGINHISTORY");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "LOGINHISTORY");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "LOGINHISTORY");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "LOGINHISTORY");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "LISTATAREFA");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "LISTATAREFA");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "LISTATAREFA");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "LISTATAREFA");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "LISTATAREFA");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "DATATRIAGEM",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "INTIMACAO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "INFORMACOESCOMPLEMENTARES");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "INFORMACOESCOMPLEMENTARES");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "INFORMACOESCOMPLEMENTARES");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "INFORMACOESCOMPLEMENTARES");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "INFORMACOESCOMPLEMENTARES");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "HISTORICOGERAL");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "HISTORICOGERAL");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "HISTORICOGERAL");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "HISTORICOGERAL");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "HISTORICOGERAL");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "GRUPOCASOENVOLVIDO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "GRUPOCASOENVOLVIDO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "GRUPOCASOENVOLVIDO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "GRUPOCASOENVOLVIDO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "GRUPOCASOENVOLVIDO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "FOTOS");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "FOTOS");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "FOTOS");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "FORO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "FORO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "FORO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "FORO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "FORO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "FAILEDLOGINATTEMPTS");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "FAILEDLOGINATTEMPTS");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "FAILEDLOGINATTEMPTS");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "FAILEDLOGINATTEMPTS");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "FAILEDLOGINATTEMPTS");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "EVENTO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "EVENTO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "EVENTO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "ETIQUETA");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "ETIQUETA");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "ETIQUETA");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "ETIQUETA");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "ETIQUETA");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "ENDERECO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "ENDERECO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "ENDERECO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "ENDERECO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "ENDERECO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "CONTABANCARIA");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "CONTABANCARIA");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "CONTABANCARIA");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "CONTABANCARIA");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "CONTABANCARIA");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "COMENTARIOS");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "COMENTARIOS");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "COMENTARIOS");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "COMENTARIOS");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "COMENTARIOS");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "CASO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "CASO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "CASO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "ATENDIMENTOHISTORICO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "ATENDIMENTOHISTORICO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "ATENDIMENTOHISTORICO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "ATENDIMENTOHISTORICO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "ATENDIMENTOHISTORICO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "ATENDIMENTO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "ATENDIMENTO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "ATENDIMENTO");

            migrationBuilder.DropColumn(
                name: "DATAATUALIZACAO",
                table: "ACAO");

            migrationBuilder.DropColumn(
                name: "DATACADASTRO",
                table: "ACAO");

            migrationBuilder.DropColumn(
                name: "DATAEXCLUSAO",
                table: "ACAO");

            migrationBuilder.DropColumn(
                name: "EXCLUIDO",
                table: "ACAO");

            migrationBuilder.DropColumn(
                name: "USUARIOEXCLUSAOID",
                table: "ACAO");
        }
    }
}
