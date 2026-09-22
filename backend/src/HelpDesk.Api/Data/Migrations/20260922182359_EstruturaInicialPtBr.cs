using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstruturaInicialPtBr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id_categoria = table.Column<Guid>(type: "uuid", nullable: false),
                    nm_categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ds_categoria = table.Column<string>(type: "text", nullable: true),
                    fl_ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    nm_usuario = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ds_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ds_senha_hash = table.Column<string>(type: "text", nullable: false),
                    id_perfil = table.Column<int>(type: "integer", nullable: false),
                    fl_ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "chamados",
                columns: table => new
                {
                    id_chamado = table.Column<Guid>(type: "uuid", nullable: false),
                    nm_titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ds_descricao = table.Column<string>(type: "text", nullable: false),
                    id_status = table.Column<int>(type: "integer", nullable: false),
                    id_prioridade = table.Column<int>(type: "integer", nullable: false),
                    id_categoria = table.Column<Guid>(type: "uuid", nullable: true),
                    id_usuario_solicitante = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario_tecnico = table.Column<Guid>(type: "uuid", nullable: true),
                    ds_resolucao = table.Column<string>(type: "text", nullable: true),
                    fl_resolucao_aprovada = table.Column<bool>(type: "boolean", nullable: false),
                    dt_resolucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dt_conclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    dt_fechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chamados", x => x.id_chamado);
                    table.ForeignKey(
                        name: "FK_chamados_categorias_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categorias",
                        principalColumn: "id_categoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_usuarios_id_usuario_solicitante",
                        column: x => x.id_usuario_solicitante,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chamados_usuarios_id_usuario_tecnico",
                        column: x => x.id_usuario_tecnico,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comentarios_chamado",
                columns: table => new
                {
                    id_comentario = table.Column<Guid>(type: "uuid", nullable: false),
                    id_chamado = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    ds_mensagem = table.Column<string>(type: "text", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios_chamado", x => x.id_comentario);
                    table.ForeignKey(
                        name: "FK_comentarios_chamado_chamados_id_chamado",
                        column: x => x.id_chamado,
                        principalTable: "chamados",
                        principalColumn: "id_chamado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comentarios_chamado_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historico_chamado",
                columns: table => new
                {
                    id_historico = table.Column<Guid>(type: "uuid", nullable: false),
                    id_chamado = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    nm_acao = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ds_valor_anterior = table.Column<string>(type: "text", nullable: true),
                    ds_valor_novo = table.Column<string>(type: "text", nullable: true),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_chamado", x => x.id_historico);
                    table.ForeignKey(
                        name: "FK_historico_chamado_chamados_id_chamado",
                        column: x => x.id_chamado,
                        principalTable: "chamados",
                        principalColumn: "id_chamado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historico_chamado_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chamados_id_categoria",
                table: "chamados",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_id_usuario_solicitante",
                table: "chamados",
                column: "id_usuario_solicitante");

            migrationBuilder.CreateIndex(
                name: "IX_chamados_id_usuario_tecnico",
                table: "chamados",
                column: "id_usuario_tecnico");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_chamado_id_chamado",
                table: "comentarios_chamado",
                column: "id_chamado");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_chamado_id_usuario",
                table: "comentarios_chamado",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_historico_chamado_id_chamado",
                table: "historico_chamado",
                column: "id_chamado");

            migrationBuilder.CreateIndex(
                name: "IX_historico_chamado_id_usuario",
                table: "historico_chamado",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_ds_email",
                table: "usuarios",
                column: "ds_email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentarios_chamado");

            migrationBuilder.DropTable(
                name: "historico_chamado");

            migrationBuilder.DropTable(
                name: "chamados");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
