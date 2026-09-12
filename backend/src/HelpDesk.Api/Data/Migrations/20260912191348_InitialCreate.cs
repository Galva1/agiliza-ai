using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpDesk.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_status",
                columns: table => new
                {
                    idstatus = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nm_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_status", x => x.idstatus);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    idusuario = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    senhaHash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usr_criacao = table.Column<Guid>(type: "uuid", nullable: true),
                    dt_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usr_alteracao = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.idusuario);
                    table.ForeignKey(
                        name: "FK_usuario_usuario_usr_alteracao",
                        column: x => x.usr_alteracao,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuario_usuario_usr_criacao",
                        column: x => x.usr_criacao,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    idticket = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    idstatus = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    idusuario_solicitante = table.Column<Guid>(type: "uuid", nullable: false),
                    idusuario_responsavel = table.Column<Guid>(type: "uuid", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usr_criacao = table.Column<Guid>(type: "uuid", nullable: true),
                    dt_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usr_alteracao = table.Column<Guid>(type: "uuid", nullable: true),
                    dt_fechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket", x => x.idticket);
                    table.ForeignKey(
                        name: "FK_ticket_ticket_status_idstatus",
                        column: x => x.idstatus,
                        principalTable: "ticket_status",
                        principalColumn: "idstatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_usuario_idusuario_responsavel",
                        column: x => x.idusuario_responsavel,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_usuario_idusuario_solicitante",
                        column: x => x.idusuario_solicitante,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_usuario_usr_alteracao",
                        column: x => x.usr_alteracao,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_usuario_usr_criacao",
                        column: x => x.usr_criacao,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ticket_comentario",
                columns: table => new
                {
                    idticket_comentario = table.Column<Guid>(type: "uuid", nullable: false),
                    idticket = table.Column<Guid>(type: "uuid", nullable: false),
                    usr_criacao = table.Column<Guid>(type: "uuid", nullable: false),
                    mensagem = table.Column<string>(type: "text", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usr_alteracao = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_comentario", x => x.idticket_comentario);
                    table.ForeignKey(
                        name: "FK_ticket_comentario_ticket_idticket",
                        column: x => x.idticket,
                        principalTable: "ticket",
                        principalColumn: "idticket",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ticket_comentario_usuario_usr_alteracao",
                        column: x => x.usr_alteracao,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_comentario_usuario_usr_criacao",
                        column: x => x.usr_criacao,
                        principalTable: "usuario",
                        principalColumn: "idusuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ticket_idstatus",
                table: "ticket",
                column: "idstatus");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_idusuario_responsavel",
                table: "ticket",
                column: "idusuario_responsavel");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_idusuario_solicitante",
                table: "ticket",
                column: "idusuario_solicitante");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_usr_alteracao",
                table: "ticket",
                column: "usr_alteracao");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_usr_criacao",
                table: "ticket",
                column: "usr_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_comentario_idticket",
                table: "ticket_comentario",
                column: "idticket");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_comentario_usr_alteracao",
                table: "ticket_comentario",
                column: "usr_alteracao");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_comentario_usr_criacao",
                table: "ticket_comentario",
                column: "usr_criacao");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_status_nm_status",
                table: "ticket_status",
                column: "nm_status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_usr_alteracao",
                table: "usuario",
                column: "usr_alteracao");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_usr_criacao",
                table: "usuario",
                column: "usr_criacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_comentario");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "ticket_status");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
