using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class PendenciasESolicitacoesAprovacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ds_resposta",
                table: "comentarios_chamado",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dt_resposta",
                table: "comentarios_chamado",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_responsavel",
                table: "comentarios_chamado",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_situacao",
                table: "comentarios_chamado",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_tipo",
                table: "comentarios_chamado",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "id_usuario_resposta",
                table: "comentarios_chamado",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_chamado_id_usuario_resposta",
                table: "comentarios_chamado",
                column: "id_usuario_resposta");

            migrationBuilder.AddForeignKey(
                name: "FK_comentarios_chamado_usuarios_id_usuario_resposta",
                table: "comentarios_chamado",
                column: "id_usuario_resposta",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comentarios_chamado_usuarios_id_usuario_resposta",
                table: "comentarios_chamado");

            migrationBuilder.DropIndex(
                name: "IX_comentarios_chamado_id_usuario_resposta",
                table: "comentarios_chamado");

            migrationBuilder.DropColumn(
                name: "ds_resposta",
                table: "comentarios_chamado");

            migrationBuilder.DropColumn(
                name: "dt_resposta",
                table: "comentarios_chamado");

            migrationBuilder.DropColumn(
                name: "id_responsavel",
                table: "comentarios_chamado");

            migrationBuilder.DropColumn(
                name: "id_situacao",
                table: "comentarios_chamado");

            migrationBuilder.DropColumn(
                name: "id_tipo",
                table: "comentarios_chamado");

            migrationBuilder.DropColumn(
                name: "id_usuario_resposta",
                table: "comentarios_chamado");
        }
    }
}
