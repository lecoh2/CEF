using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeslandesApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACAO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEACAO = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACAO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ETIQUETA",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOME = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    COR = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ETIQUETA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FORO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEFORO = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NIVEL",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMENIVEL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NIVEL", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PECA_CABIVEL",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEPECA = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    PRAZODIAS = table.Column<int>(type: "int", nullable: false),
                    SUGESTAOCOMPLEXIDADEPADRAO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PECA_CABIVEL", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "QUALIFICACAO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEQUALIFICACAO = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QUALIFICACAO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SETORES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMESETOR = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SETORES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEUSUARIO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    LOGIN = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    SENHA = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    STATUS = table.Column<int>(type: "int", nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VARA",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEVARA = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    NUMERO = table.Column<int>(type: "int", nullable: false),
                    TIPO = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    FOROID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VARA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VARA_FORO_FOROID",
                        column: x => x.FOROID,
                        principalTable: "FORO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CASO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PASTA = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    TITULO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    DESCRICAO = table.Column<string>(type: "varchar(2000)", unicode: false, maxLength: 2000, nullable: false),
                    OBSERVACAO = table.Column<string>(type: "varchar(2000)", unicode: false, maxLength: 2000, nullable: true),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RESPONSAVELID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    USUARIOCADASTROID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ACESSO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CASO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CASO_USUARIO",
                        column: x => x.RESPONSAVELID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CASO_USUARIOS_USUARIOCADASTROID",
                        column: x => x.USUARIOCADASTROID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FAILEDLOGINATTEMPTS",
                columns: table => new
                {
                    IDFAILEDLOGINATTEMPT = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDUSUARIO = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LOGIN = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    IPACESSO = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    USERAGENT = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    DATAHORA = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MENSAGEM = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAILEDLOGINATTEMPTS", x => x.IDFAILEDLOGINATTEMPT);
                    table.ForeignKey(
                        name: "FK_FAILEDLOGINATTEMPTS_USUARIOS_IDUSUARIO",
                        column: x => x.IDUSUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FOTOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FOTONOME = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    FILEURL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOTOS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FOTOS_USUARIOS_USUARIO_ID",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "GRUPONIVEL",
                columns: table => new
                {
                    IDUSUARIO = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDNIVEL = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPONIVEL", x => new { x.IDUSUARIO, x.IDNIVEL });
                    table.ForeignKey(
                        name: "FK_GRUPONIVEL_NIVEL_IDNIVEL",
                        column: x => x.IDNIVEL,
                        principalTable: "NIVEL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRUPONIVEL_USUARIOS_IDUSUARIO",
                        column: x => x.IDUSUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOSETORES",
                columns: table => new
                {
                    IDUSUARIO = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDSETOR = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOSETORES", x => new { x.IDUSUARIO, x.IDSETOR });
                    table.ForeignKey(
                        name: "FK_GRUPOSETORES_SETORES_IDSETOR",
                        column: x => x.IDSETOR,
                        principalTable: "SETORES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRUPOSETORES_USUARIOS_IDUSUARIO",
                        column: x => x.IDUSUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HISTORICOGERAL",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ENTIDADE = table.Column<int>(type: "int", nullable: false),
                    ENTIDADEID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DATAALTERACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OBSERVACAO = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    DADOSANTES = table.Column<string>(type: "text", unicode: false, maxLength: 250, nullable: false),
                    DADOSDEPOIS = table.Column<string>(type: "text", unicode: false, maxLength: 250, nullable: false),
                    IP = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    USERAGENT = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORICOGERAL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HISTORICOGERAL_USUARIOS_USUARIOID",
                        column: x => x.USUARIOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LOGINHISTORY",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDUSUARIO = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IPACESSO = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    USERAGENT = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    DATAHORAACESSO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SUCESSO = table.Column<bool>(type: "bit", nullable: false),
                    MENSAGEM = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGINHISTORY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LOGINHISTORY_USUARIOS_IDUSUARIO",
                        column: x => x.IDUSUARIO,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOTE_TRABALHO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NUMEROLOTE = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RESPONSAVELID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    COORDENADORID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DATACRIACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAPRAZOLOTE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    STATUS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOTE_TRABALHO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LOTE_TRABALHO_USUARIOS_COORDENADORID",
                        column: x => x.COORDENADORID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_LOTE_TRABALHO_USUARIOS_RESPONSAVELID",
                        column: x => x.RESPONSAVELID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NOTIFICACAO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TITULO = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    MENSAGEM = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    LIDA = table.Column<bool>(type: "bit", nullable: false),
                    DATACRIACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TIPO = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    LINK = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    ENTIDADEID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICACAO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NOTIFICACAO_USUARIOS_USUARIOID",
                        column: x => x.USUARIOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PESSOA",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOME = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    APELIDO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    TELEFONE = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    SITE = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PERFIL = table.Column<int>(type: "int", nullable: true),
                    TIPO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    RG = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CPF = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    TITULOELEITOR = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CARTEIRATRABALHO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    PISPASEP = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CNH = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    PASSAPORTE = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CERTIDAORESERVISTA = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CNPJ = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    INSCRICAOESTADUAL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    INSCRICAOMUNICIPAL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    SIMPLESNACIONAL = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PESSOA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PESSOA_USUARIOS_USUARIO_ID",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOETIQUETACASOS",
                columns: table => new
                {
                    ETIQUETAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CASOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOETIQUETACASOS", x => new { x.ETIQUETAID, x.CASOID });
                    table.ForeignKey(
                        name: "FK_CASO_ETIQUETA",
                        column: x => x.CASOID,
                        principalTable: "CASO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOETIQUETACASOS_ETIQUETA_ETIQUETAID",
                        column: x => x.ETIQUETAID,
                        principalTable: "ETIQUETA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CONTABANCARIA",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NOMEBANCO = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    AGENCIA = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    NUMEROCONTA = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PIX = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TIPOCONTA = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTABANCARIA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CONTABANCARIA_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ENDERECO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LOGRADOURO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    NUMERO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    COMPLEMENTO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    BAIRRO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CEP = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    LOCALIDADE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    UF = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: true),
                    PESSOA_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENDERECO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ENDERECO_PESSOA_PESSOA_ID",
                        column: x => x.PESSOA_ID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOCASOCLIENTE",
                columns: table => new
                {
                    CASOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOCASOCLIENTE", x => new { x.CASOID, x.PESSOAID });
                    table.ForeignKey(
                        name: "FK_CASO_CASOCLIENTE",
                        column: x => x.CASOID,
                        principalTable: "CASO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOCASOCLIENTE_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOCASOENVOLVIDO",
                columns: table => new
                {
                    CASOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUALIFICACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOCASOENVOLVIDO", x => new { x.CASOID, x.PESSOAID });
                    table.ForeignKey(
                        name: "FK_CASO_CASOENVOLVIDO",
                        column: x => x.CASOID,
                        principalTable: "CASO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOCASOENVOLVIDO_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRUPOCASOENVOLVIDO_QUALIFICACAO_QUALIFICACAOID",
                        column: x => x.QUALIFICACAOID,
                        principalTable: "QUALIFICACAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOPESSOASETIQUETAS",
                columns: table => new
                {
                    ETIQUETAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOPESSOASETIQUETAS", x => new { x.ETIQUETAID, x.PESSOAID });
                    table.ForeignKey(
                        name: "FK_GRUPOPESSOASETIQUETAS_ETIQUETA_ETIQUETAID",
                        column: x => x.ETIQUETAID,
                        principalTable: "ETIQUETA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOPESSOASETIQUETAS_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INFORMACOESCOMPLEMENTARES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CODIGO = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    COMENTARIO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    PESSOA_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TIPOPESSOA = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    DATANASCIMENTO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    NOMEEMPRESA = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    PROFISSAO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ATIVIDADEECONOMICA = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ESTADOCIVIL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NOMEPAI = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    NOMEMAE = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    NATURALIDADE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    NACIONALIDADE = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    TRATAMENTO = table.Column<int>(type: "int", nullable: true),
                    CONTATO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CARGO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INFORMACOESCOMPLEMENTARES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_INFORMACOESCOMPLEMENTARES_PESSOA_PESSOA_ID",
                        column: x => x.PESSOA_ID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PROCESSOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VARAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    USUARIORESPONSAVELID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    USUARIOCADASTROID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PASTA = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    TITULO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    NUMEROPROCESSO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    LINKTRIBUNAL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    OBJETO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    VALORCAUSA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DISTRIBUIDO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VALORCONDENACAO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OBSERVACAO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    INSTANCIA = table.Column<int>(type: "int", nullable: true),
                    ACESSO = table.Column<int>(type: "int", nullable: true),
                    PARTEADVERSAID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROCESSOS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PROCESSOS_ACAO_ACAOID",
                        column: x => x.ACAOID,
                        principalTable: "ACAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PROCESSOS_PESSOA_PARTEADVERSAID",
                        column: x => x.PARTEADVERSAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PROCESSOS_USUARIOS_USUARIOCADASTROID",
                        column: x => x.USUARIOCADASTROID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PROCESSOS_USUARIOS_USUARIORESPONSAVELID",
                        column: x => x.USUARIORESPONSAVELID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PROCESSOS_VARA_VARAID",
                        column: x => x.VARAID,
                        principalTable: "VARA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ATENDIMENTO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ASSUNTO = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    REGISTRO = table.Column<string>(type: "varchar(2000)", unicode: false, maxLength: 2000, nullable: false),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CASOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RESPONSAVELID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    USUARIOCADASTROID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ATENDIMENTOPAIID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TIPOVINCULO = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATENDIMENTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_ATENDIMENTO_ATENDIMENTOPAIID",
                        column: x => x.ATENDIMENTOPAIID,
                        principalTable: "ATENDIMENTO",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_CASO_CASOID",
                        column: x => x.CASOID,
                        principalTable: "CASO",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_PROCESSO",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_USUARIOS_RESPONSAVELID",
                        column: x => x.RESPONSAVELID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_USUARIOS_USUARIOCADASTROID",
                        column: x => x.USUARIOCADASTROID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "GRUPOCLIENTEPROCESSO",
                columns: table => new
                {
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUALIFICACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOCLIENTEPROCESSO", x => new { x.PESSOAID, x.PROCESSOID, x.QUALIFICACAOID });
                    table.ForeignKey(
                        name: "FK_GRUPOCLIENTEPROCESSO_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOCLIENTEPROCESSO_PROCESSOS_PROCESSOID",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOCLIENTEPROCESSO_QUALIFICACAO_QUALIFICACAOID",
                        column: x => x.QUALIFICACAOID,
                        principalTable: "QUALIFICACAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOENVOLVIDOS",
                columns: table => new
                {
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUALIFICACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOENVOLVIDOS", x => new { x.PESSOAID, x.PROCESSOID, x.QUALIFICACAOID });
                    table.ForeignKey(
                        name: "FK_GRUPOENVOLVIDOS_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOENVOLVIDOS_PROCESSOS_PROCESSOID",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOENVOLVIDOS_QUALIFICACAO_QUALIFICACAOID",
                        column: x => x.QUALIFICACAOID,
                        principalTable: "QUALIFICACAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOENVOLVIDOSPROCESSO",
                columns: table => new
                {
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUALIFICACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOENVOLVIDOSPROCESSO", x => new { x.PESSOAID, x.PROCESSOID, x.QUALIFICACAOID });
                    table.ForeignKey(
                        name: "FK_GRUPOENVOLVIDOSPROCESSO_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRUPOENVOLVIDOSPROCESSO_PROCESSOS_PROCESSOID",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOENVOLVIDOSPROCESSO_QUALIFICACAO_QUALIFICACAOID",
                        column: x => x.QUALIFICACAOID,
                        principalTable: "QUALIFICACAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOETIQUETASPROCESSOS",
                columns: table => new
                {
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ETIQUETAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOETIQUETASPROCESSOS", x => new { x.ETIQUETAID, x.PROCESSOID });
                    table.ForeignKey(
                        name: "FK_GRUPOETIQUETASPROCESSOS_ETIQUETA_ETIQUETAID",
                        column: x => x.ETIQUETAID,
                        principalTable: "ETIQUETA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOETIQUETASPROCESSOS_PROCESSOS_PROCESSOID",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOPESSOACLIENTES",
                columns: table => new
                {
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUALIFICACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOPESSOACLIENTES", x => new { x.PESSOAID, x.PROCESSOID, x.QUALIFICACAOID });
                    table.ForeignKey(
                        name: "FK_GRUPOPESSOACLIENTES_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRUPOPESSOACLIENTES_PROCESSOS_PROCESSOID",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOPESSOACLIENTES_QUALIFICACAO_QUALIFICACAOID",
                        column: x => x.QUALIFICACAOID,
                        principalTable: "QUALIFICACAO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INTIMACAO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DATAIMPORTACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAINTIMACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TEXTOINTIMACAO = table.Column<string>(type: "VARCHAR(MAX)", unicode: false, maxLength: 250, nullable: false),
                    STATUSTRIAGEM = table.Column<int>(type: "int", nullable: false),
                    PECACABIVELID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PRAZOINDIVIDUALCPC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    COMPLEXIDADEREAL = table.Column<int>(type: "int", nullable: true),
                    OBSERVACAOTRIAGEM = table.Column<string>(type: "VARCHAR(MAX)", unicode: false, maxLength: 250, nullable: true),
                    ADVOGADOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LOTEID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    STATUSCUMPRIMENTO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INTIMACAO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_INTIMACAO_LOTE_TRABALHO_LOTEID",
                        column: x => x.LOTEID,
                        principalTable: "LOTE_TRABALHO",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_INTIMACAO_PECA_CABIVEL_PECACABIVELID",
                        column: x => x.PECACABIVELID,
                        principalTable: "PECA_CABIVEL",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_INTIMACAO_PROCESSOS_PROCESSOID",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_INTIMACAO_USUARIOS_ADVOGADOID",
                        column: x => x.ADVOGADOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ATENDIMENTOHISTORICO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ATENDIMENTO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIO_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DATAALTERACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OBSERVACAO = table.Column<string>(type: "VARCHAR(255)", unicode: false, maxLength: 250, nullable: true),
                    DADOSANTES = table.Column<string>(type: "VARCHAR(MAX)", unicode: false, maxLength: 250, nullable: true),
                    DADOSDEPOIS = table.Column<string>(type: "VARCHAR(MAX)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATENDIMENTOHISTORICO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ATENDIMENTOHISTORICO_ATENDIMENTO",
                        column: x => x.ATENDIMENTO_ID,
                        principalTable: "ATENDIMENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ATENDIMENTOHISTORICO_USUARIO",
                        column: x => x.USUARIO_ID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TITULO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    DATAINICIAL = table.Column<DateOnly>(type: "date", nullable: false),
                    HORAINICIAL = table.Column<TimeOnly>(type: "time", nullable: false),
                    DATAFINAL = table.Column<DateOnly>(type: "date", nullable: true),
                    HORAFINAL = table.Column<TimeOnly>(type: "time", nullable: true),
                    DIAINTEIRO = table.Column<bool>(type: "bit", nullable: false),
                    ENDERECO = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MODALIDADE = table.Column<int>(type: "int", nullable: false),
                    OBSERVACAO = table.Column<string>(type: "varchar(2000)", unicode: false, maxLength: 2000, nullable: true),
                    TIPORECORRENCIA = table.Column<int>(type: "int", nullable: false),
                    INTERVALORECORRENCIA = table.Column<int>(type: "int", nullable: false),
                    DIASSEMANA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DATAFIMRECORRENCIA = table.Column<DateOnly>(type: "date", nullable: true),
                    QUANTIDADEOCORRENCIAS = table.Column<int>(type: "int", nullable: true),
                    STATUSKANBAN = table.Column<int>(type: "int", nullable: false),
                    USUARIOCRIACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TIPOVINCULOID = table.Column<int>(type: "int", nullable: true),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CASOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ATENDIMENTOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EVENTO_ATENDIMENTO",
                        column: x => x.ATENDIMENTOID,
                        principalTable: "ATENDIMENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVENTO_CASO",
                        column: x => x.CASOID,
                        principalTable: "CASO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVENTO_PROCESSO",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVENTO_USUARIOS_USUARIOCRIACAOID",
                        column: x => x.USUARIOCRIACAOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOATENDIMENTOCLIENTE",
                columns: table => new
                {
                    ATENDIMENTOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PESSOAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOATENDIMENTOCLIENTE", x => new { x.ATENDIMENTOID, x.PESSOAID });
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_ATENDIMENTCLIENTE",
                        column: x => x.ATENDIMENTOID,
                        principalTable: "ATENDIMENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOATENDIMENTOCLIENTE_PESSOA_PESSOAID",
                        column: x => x.PESSOAID,
                        principalTable: "PESSOA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOETIQUETASATENDIMENTOS",
                columns: table => new
                {
                    ETIQUETAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ATENDIMENTOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOETIQUETASATENDIMENTOS", x => new { x.ETIQUETAID, x.ATENDIMENTOID });
                    table.ForeignKey(
                        name: "FK_ATENDIMENTO_ETIQUETA",
                        column: x => x.ATENDIMENTOID,
                        principalTable: "ATENDIMENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOETIQUETASATENDIMENTOS_ETIQUETA_ETIQUETAID",
                        column: x => x.ETIQUETAID,
                        principalTable: "ETIQUETA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TAREFA",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DESCRICAO = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    DATATAREFA = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DATACADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DATAATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TIPOVINCULOID = table.Column<int>(type: "int", nullable: true),
                    PROCESSOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CASOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ATENDIMENTOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    USUARIOCRIACAOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RESPONSAVELID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PRIORIDADE = table.Column<int>(type: "int", nullable: false),
                    STATUSKANBAN = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAREFA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TAREFA_ATENDIMENTO",
                        column: x => x.ATENDIMENTOID,
                        principalTable: "ATENDIMENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TAREFA_CASO",
                        column: x => x.CASOID,
                        principalTable: "CASO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TAREFA_PROCESSO",
                        column: x => x.PROCESSOID,
                        principalTable: "PROCESSOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TAREFA_USUARIOS_RESPONSAVELID",
                        column: x => x.RESPONSAVELID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TAREFA_USUARIOS_USUARIOCRIACAOID",
                        column: x => x.USUARIOCRIACAOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOEVENTOETIQUETAS",
                columns: table => new
                {
                    EVENDOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ETIQUETAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOEVENTOETIQUETAS", x => new { x.EVENDOID, x.ETIQUETAID });
                    table.ForeignKey(
                        name: "FK_EVENTOETIQUETA_ETIQUETA",
                        column: x => x.ETIQUETAID,
                        principalTable: "ETIQUETA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EVENTOETIQUETA_EVENTO",
                        column: x => x.EVENDOID,
                        principalTable: "EVENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOEVENTORESPONSAVEL",
                columns: table => new
                {
                    EVENTOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOEVENTORESPONSAVEL", x => new { x.EVENTOID, x.USUARIOID });
                    table.ForeignKey(
                        name: "FK_EVENTO_EVENTORESPONSAVEL",
                        column: x => x.EVENTOID,
                        principalTable: "EVENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRUPOEVENTORESPONSAVEL_USUARIOS_USUARIOID",
                        column: x => x.USUARIOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "COMENTARIOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TEXTO = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    DATACRIACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    USUARIOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TAREFAID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EVENTOID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMENTARIOS", x => x.ID);
                    table.CheckConstraint("CK_Comentario_Vinculo", "[TarefaId] IS NOT NULL OR [EventoId] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_COMENTARIOS_EVENTO_EVENTOID",
                        column: x => x.EVENTOID,
                        principalTable: "EVENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_COMENTARIOS_TAREFA_TAREFAID",
                        column: x => x.TAREFAID,
                        principalTable: "TAREFA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_COMENTARIOS_USUARIOS_USUARIOID",
                        column: x => x.USUARIOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOTAREFARESPONSAVEIS",
                columns: table => new
                {
                    TAREFAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USUARIOID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOTAREFARESPONSAVEIS", x => new { x.USUARIOID, x.TAREFAID });
                    table.ForeignKey(
                        name: "FK_TAREFARESPONSAVEIS_TAREFA",
                        column: x => x.TAREFAID,
                        principalTable: "TAREFA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TAREFARESPONSAVEIS_USUARIO",
                        column: x => x.USUARIOID,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRUPOTAREFASETIQUETAS",
                columns: table => new
                {
                    TAREFAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ETIQUETAID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRUPOTAREFASETIQUETAS", x => new { x.TAREFAID, x.ETIQUETAID });
                    table.ForeignKey(
                        name: "FK_TAREFAETIQUETA_ETIQUETA",
                        column: x => x.ETIQUETAID,
                        principalTable: "ETIQUETA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TAREFAETIQUETA_TAREFA",
                        column: x => x.TAREFAID,
                        principalTable: "TAREFA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LISTATAREFA",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DESCRICAO = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false),
                    CONCLUIDA = table.Column<bool>(type: "bit", nullable: false),
                    DATACONCLUSAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TAREFAID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ORDEM = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LISTATAREFA", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TAREFA_LISTATAREFA",
                        column: x => x.TAREFAID,
                        principalTable: "TAREFA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTO_ATENDIMENTOPAIID",
                table: "ATENDIMENTO",
                column: "ATENDIMENTOPAIID");

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTO_CASOID",
                table: "ATENDIMENTO",
                column: "CASOID");

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTO_PROCESSOID",
                table: "ATENDIMENTO",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTO_RESPONSAVELID",
                table: "ATENDIMENTO",
                column: "RESPONSAVELID");

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTO_USUARIOCADASTROID",
                table: "ATENDIMENTO",
                column: "USUARIOCADASTROID");

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTOHISTORICO_ATENDIMENTO_ID",
                table: "ATENDIMENTOHISTORICO",
                column: "ATENDIMENTO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ATENDIMENTOHISTORICO_USUARIO_ID",
                table: "ATENDIMENTOHISTORICO",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CASO_RESPONSAVELID",
                table: "CASO",
                column: "RESPONSAVELID");

            migrationBuilder.CreateIndex(
                name: "IX_CASO_USUARIOCADASTROID",
                table: "CASO",
                column: "USUARIOCADASTROID");

            migrationBuilder.CreateIndex(
                name: "IX_COMENTARIOS_EVENTOID",
                table: "COMENTARIOS",
                column: "EVENTOID");

            migrationBuilder.CreateIndex(
                name: "IX_COMENTARIOS_TAREFAID",
                table: "COMENTARIOS",
                column: "TAREFAID");

            migrationBuilder.CreateIndex(
                name: "IX_COMENTARIOS_USUARIOID",
                table: "COMENTARIOS",
                column: "USUARIOID");

            migrationBuilder.CreateIndex(
                name: "IX_CONTABANCARIA_PESSOAID",
                table: "CONTABANCARIA",
                column: "PESSOAID");

            migrationBuilder.CreateIndex(
                name: "IX_ENDERECO_PESSOA_ID",
                table: "ENDERECO",
                column: "PESSOA_ID",
                unique: true,
                filter: "[PESSOA_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_ATENDIMENTOID",
                table: "EVENTO",
                column: "ATENDIMENTOID");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_CASOID",
                table: "EVENTO",
                column: "CASOID");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_PROCESSOID",
                table: "EVENTO",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_USUARIOCRIACAOID",
                table: "EVENTO",
                column: "USUARIOCRIACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_FAILEDLOGINATTEMPTS_IDUSUARIO",
                table: "FAILEDLOGINATTEMPTS",
                column: "IDUSUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_FORO_NOMEFORO",
                table: "FORO",
                column: "NOMEFORO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FOTOS_USUARIO_ID",
                table: "FOTOS",
                column: "USUARIO_ID",
                unique: true,
                filter: "[USUARIO_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOATENDIMENTOCLIENTE_PESSOAID",
                table: "GRUPOATENDIMENTOCLIENTE",
                column: "PESSOAID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOCASOCLIENTE_PESSOAID",
                table: "GRUPOCASOCLIENTE",
                column: "PESSOAID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOCASOENVOLVIDO_PESSOAID",
                table: "GRUPOCASOENVOLVIDO",
                column: "PESSOAID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOCASOENVOLVIDO_QUALIFICACAOID",
                table: "GRUPOCASOENVOLVIDO",
                column: "QUALIFICACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOCLIENTEPROCESSO_PROCESSOID",
                table: "GRUPOCLIENTEPROCESSO",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOCLIENTEPROCESSO_QUALIFICACAOID",
                table: "GRUPOCLIENTEPROCESSO",
                column: "QUALIFICACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOENVOLVIDOS_PROCESSOID",
                table: "GRUPOENVOLVIDOS",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOENVOLVIDOS_QUALIFICACAOID",
                table: "GRUPOENVOLVIDOS",
                column: "QUALIFICACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOENVOLVIDOSPROCESSO_PROCESSOID",
                table: "GRUPOENVOLVIDOSPROCESSO",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOENVOLVIDOSPROCESSO_QUALIFICACAOID",
                table: "GRUPOENVOLVIDOSPROCESSO",
                column: "QUALIFICACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOETIQUETACASOS_CASOID",
                table: "GRUPOETIQUETACASOS",
                column: "CASOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOETIQUETASATENDIMENTOS_ATENDIMENTOID",
                table: "GRUPOETIQUETASATENDIMENTOS",
                column: "ATENDIMENTOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOETIQUETASPROCESSOS_PROCESSOID",
                table: "GRUPOETIQUETASPROCESSOS",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOEVENTOETIQUETAS_ETIQUETAID",
                table: "GRUPOEVENTOETIQUETAS",
                column: "ETIQUETAID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOEVENTORESPONSAVEL_USUARIOID",
                table: "GRUPOEVENTORESPONSAVEL",
                column: "USUARIOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPONIVEL_IDNIVEL",
                table: "GRUPONIVEL",
                column: "IDNIVEL");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOPESSOACLIENTES_PROCESSOID",
                table: "GRUPOPESSOACLIENTES",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOPESSOACLIENTES_QUALIFICACAOID",
                table: "GRUPOPESSOACLIENTES",
                column: "QUALIFICACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOPESSOASETIQUETAS_PESSOAID",
                table: "GRUPOPESSOASETIQUETAS",
                column: "PESSOAID");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOSETORES_IDSETOR",
                table: "GRUPOSETORES",
                column: "IDSETOR");

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOTAREFARESPONSAVEIS_TAREFAID_USUARIOID",
                table: "GRUPOTAREFARESPONSAVEIS",
                columns: new[] { "TAREFAID", "USUARIOID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRUPOTAREFASETIQUETAS_ETIQUETAID",
                table: "GRUPOTAREFASETIQUETAS",
                column: "ETIQUETAID");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICOGERAL_DATAALTERACAO",
                table: "HISTORICOGERAL",
                column: "DATAALTERACAO");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICOGERAL_ENTIDADE",
                table: "HISTORICOGERAL",
                column: "ENTIDADE");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICOGERAL_ENTIDADE_ENTIDADEID",
                table: "HISTORICOGERAL",
                columns: new[] { "ENTIDADE", "ENTIDADEID" });

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICOGERAL_ENTIDADEID",
                table: "HISTORICOGERAL",
                column: "ENTIDADEID");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICOGERAL_USUARIOID",
                table: "HISTORICOGERAL",
                column: "USUARIOID");

            migrationBuilder.CreateIndex(
                name: "IX_INFORMACOESCOMPLEMENTARES_PESSOA_ID",
                table: "INFORMACOESCOMPLEMENTARES",
                column: "PESSOA_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_ADVOGADOID",
                table: "INTIMACAO",
                column: "ADVOGADOID");

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_DATAINTIMACAO",
                table: "INTIMACAO",
                column: "DATAINTIMACAO");

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_LOTEID",
                table: "INTIMACAO",
                column: "LOTEID");

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_PECACABIVELID",
                table: "INTIMACAO",
                column: "PECACABIVELID");

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_PROCESSOID",
                table: "INTIMACAO",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_INTIMACAO_STATUSTRIAGEM",
                table: "INTIMACAO",
                column: "STATUSTRIAGEM");

            migrationBuilder.CreateIndex(
                name: "IX_LISTATAREFA_TAREFAID",
                table: "LISTATAREFA",
                column: "TAREFAID");

            migrationBuilder.CreateIndex(
                name: "IX_LOGINHISTORY_IDUSUARIO",
                table: "LOGINHISTORY",
                column: "IDUSUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_LOTE_TRABALHO_COORDENADORID",
                table: "LOTE_TRABALHO",
                column: "COORDENADORID");

            migrationBuilder.CreateIndex(
                name: "IX_LOTE_TRABALHO_RESPONSAVELID",
                table: "LOTE_TRABALHO",
                column: "RESPONSAVELID");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICACAO_USUARIOID",
                table: "NOTIFICACAO",
                column: "USUARIOID");

            migrationBuilder.CreateIndex(
                name: "IX_PESSOA_USUARIO_ID",
                table: "PESSOA",
                column: "USUARIO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROCESSOS_ACAOID",
                table: "PROCESSOS",
                column: "ACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_PROCESSOS_PARTEADVERSAID",
                table: "PROCESSOS",
                column: "PARTEADVERSAID");

            migrationBuilder.CreateIndex(
                name: "IX_PROCESSOS_USUARIOCADASTROID",
                table: "PROCESSOS",
                column: "USUARIOCADASTROID");

            migrationBuilder.CreateIndex(
                name: "IX_PROCESSOS_USUARIORESPONSAVELID",
                table: "PROCESSOS",
                column: "USUARIORESPONSAVELID");

            migrationBuilder.CreateIndex(
                name: "IX_PROCESSOS_VARAID",
                table: "PROCESSOS",
                column: "VARAID");

            migrationBuilder.CreateIndex(
                name: "IX_TAREFA_ATENDIMENTOID",
                table: "TAREFA",
                column: "ATENDIMENTOID");

            migrationBuilder.CreateIndex(
                name: "IX_TAREFA_CASOID",
                table: "TAREFA",
                column: "CASOID");

            migrationBuilder.CreateIndex(
                name: "IX_TAREFA_PROCESSOID",
                table: "TAREFA",
                column: "PROCESSOID");

            migrationBuilder.CreateIndex(
                name: "IX_TAREFA_RESPONSAVELID",
                table: "TAREFA",
                column: "RESPONSAVELID");

            migrationBuilder.CreateIndex(
                name: "IX_TAREFA_USUARIOCRIACAOID",
                table: "TAREFA",
                column: "USUARIOCRIACAOID");

            migrationBuilder.CreateIndex(
                name: "IX_VARA_FOROID",
                table: "VARA",
                column: "FOROID");

            migrationBuilder.CreateIndex(
                name: "IX_VARA_NUMERO_TIPO_FOROID",
                table: "VARA",
                columns: new[] { "NUMERO", "TIPO", "FOROID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATENDIMENTOHISTORICO");

            migrationBuilder.DropTable(
                name: "COMENTARIOS");

            migrationBuilder.DropTable(
                name: "CONTABANCARIA");

            migrationBuilder.DropTable(
                name: "ENDERECO");

            migrationBuilder.DropTable(
                name: "FAILEDLOGINATTEMPTS");

            migrationBuilder.DropTable(
                name: "FOTOS");

            migrationBuilder.DropTable(
                name: "GRUPOATENDIMENTOCLIENTE");

            migrationBuilder.DropTable(
                name: "GRUPOCASOCLIENTE");

            migrationBuilder.DropTable(
                name: "GRUPOCASOENVOLVIDO");

            migrationBuilder.DropTable(
                name: "GRUPOCLIENTEPROCESSO");

            migrationBuilder.DropTable(
                name: "GRUPOENVOLVIDOS");

            migrationBuilder.DropTable(
                name: "GRUPOENVOLVIDOSPROCESSO");

            migrationBuilder.DropTable(
                name: "GRUPOETIQUETACASOS");

            migrationBuilder.DropTable(
                name: "GRUPOETIQUETASATENDIMENTOS");

            migrationBuilder.DropTable(
                name: "GRUPOETIQUETASPROCESSOS");

            migrationBuilder.DropTable(
                name: "GRUPOEVENTOETIQUETAS");

            migrationBuilder.DropTable(
                name: "GRUPOEVENTORESPONSAVEL");

            migrationBuilder.DropTable(
                name: "GRUPONIVEL");

            migrationBuilder.DropTable(
                name: "GRUPOPESSOACLIENTES");

            migrationBuilder.DropTable(
                name: "GRUPOPESSOASETIQUETAS");

            migrationBuilder.DropTable(
                name: "GRUPOSETORES");

            migrationBuilder.DropTable(
                name: "GRUPOTAREFARESPONSAVEIS");

            migrationBuilder.DropTable(
                name: "GRUPOTAREFASETIQUETAS");

            migrationBuilder.DropTable(
                name: "HISTORICOGERAL");

            migrationBuilder.DropTable(
                name: "INFORMACOESCOMPLEMENTARES");

            migrationBuilder.DropTable(
                name: "INTIMACAO");

            migrationBuilder.DropTable(
                name: "LISTATAREFA");

            migrationBuilder.DropTable(
                name: "LOGINHISTORY");

            migrationBuilder.DropTable(
                name: "NOTIFICACAO");

            migrationBuilder.DropTable(
                name: "EVENTO");

            migrationBuilder.DropTable(
                name: "NIVEL");

            migrationBuilder.DropTable(
                name: "QUALIFICACAO");

            migrationBuilder.DropTable(
                name: "SETORES");

            migrationBuilder.DropTable(
                name: "ETIQUETA");

            migrationBuilder.DropTable(
                name: "LOTE_TRABALHO");

            migrationBuilder.DropTable(
                name: "PECA_CABIVEL");

            migrationBuilder.DropTable(
                name: "TAREFA");

            migrationBuilder.DropTable(
                name: "ATENDIMENTO");

            migrationBuilder.DropTable(
                name: "CASO");

            migrationBuilder.DropTable(
                name: "PROCESSOS");

            migrationBuilder.DropTable(
                name: "ACAO");

            migrationBuilder.DropTable(
                name: "PESSOA");

            migrationBuilder.DropTable(
                name: "VARA");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "FORO");
        }
    }
}
