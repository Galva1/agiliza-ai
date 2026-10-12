using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ResolucoesEModelosTexto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "modelos_texto",
                columns: table => new
                {
                    id_modelo_texto = table.Column<Guid>(type: "uuid", nullable: false),
                    nm_titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    ds_conteudo = table.Column<string>(type: "text", nullable: false),
                    fl_ativo = table.Column<bool>(type: "boolean", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modelos_texto", x => x.id_modelo_texto);
                });

            migrationBuilder.CreateTable(
                name: "resolucoes_chamado",
                columns: table => new
                {
                    id_resolucao = table.Column<Guid>(type: "uuid", nullable: false),
                    id_chamado = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    ds_resolucao = table.Column<string>(type: "text", nullable: false),
                    id_situacao = table.Column<int>(type: "integer", nullable: false),
                    ds_motivo_recusa = table.Column<string>(type: "text", nullable: true),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id_usuario_avaliador = table.Column<Guid>(type: "uuid", nullable: true),
                    dt_avaliacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resolucoes_chamado", x => x.id_resolucao);
                    table.ForeignKey(
                        name: "FK_resolucoes_chamado_chamados_id_chamado",
                        column: x => x.id_chamado,
                        principalTable: "chamados",
                        principalColumn: "id_chamado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resolucoes_chamado_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_resolucoes_chamado_usuarios_id_usuario_avaliador",
                        column: x => x.id_usuario_avaliador,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_modelos_texto_nm_titulo",
                table: "modelos_texto",
                column: "nm_titulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resolucoes_chamado_id_chamado",
                table: "resolucoes_chamado",
                column: "id_chamado");

            migrationBuilder.CreateIndex(
                name: "IX_resolucoes_chamado_id_usuario",
                table: "resolucoes_chamado",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_resolucoes_chamado_id_usuario_avaliador",
                table: "resolucoes_chamado",
                column: "id_usuario_avaliador");

            // Migra a resolução atual de cada chamado para o novo registro de resoluções.
            migrationBuilder.Sql(@"
                INSERT INTO resolucoes_chamado (id_resolucao, id_chamado, id_usuario, ds_resolucao, id_situacao, dt_cadastro, dt_avaliacao)
                SELECT gen_random_uuid(),
                       c.id_chamado,
                       COALESCE(c.id_usuario_tecnico, c.id_usuario_solicitante),
                       c.ds_resolucao,
                       CASE WHEN c.fl_resolucao_aprovada THEN 2 ELSE 1 END,
                       COALESCE(c.dt_resolucao, c.dt_atualizacao, c.dt_cadastro),
                       CASE WHEN c.fl_resolucao_aprovada THEN c.dt_conclusao ELSE NULL END
                FROM chamados c
                WHERE c.ds_resolucao IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "modelos_texto");

            migrationBuilder.DropTable(
                name: "resolucoes_chamado");
        }
    }
}
