using HelpDesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<ComentarioChamado> Comentarios => Set<ComentarioChamado>();
    public DbSet<HistoricoChamado> Historico => Set<HistoricoChamado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id_usuario");
            entity.Property(u => u.Nome).HasColumnName("nm_usuario").HasMaxLength(150).IsRequired();
            entity.Property(u => u.Email).HasColumnName("ds_email").HasMaxLength(200).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).HasColumnName("ds_senha_hash").IsRequired();
            entity.Property(u => u.Perfil).HasColumnName("id_perfil").HasConversion<int>();
            entity.Property(u => u.Ativo).HasColumnName("fl_ativo");
            entity.Property(u => u.CriadoEm).HasColumnName("dt_cadastro");
            entity.Property(u => u.AtualizadoEm).HasColumnName("dt_atualizacao");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id_categoria");
            entity.Property(c => c.Nome).HasColumnName("nm_categoria").HasMaxLength(100).IsRequired();
            entity.Property(c => c.Descricao).HasColumnName("ds_categoria");
            entity.Property(c => c.Ativa).HasColumnName("fl_ativo");
            entity.Property(c => c.CriadoEm).HasColumnName("dt_cadastro");
        });

        modelBuilder.Entity<Chamado>(entity =>
        {
            entity.ToTable("chamados");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id_chamado");
            entity.Property(c => c.Titulo).HasColumnName("nm_titulo").HasMaxLength(200).IsRequired();
            entity.Property(c => c.Descricao).HasColumnName("ds_descricao").IsRequired();
            entity.Property(c => c.Status).HasColumnName("id_status").HasConversion<int>();
            entity.Property(c => c.Prioridade).HasColumnName("id_prioridade").HasConversion<int>();
            entity.Property(c => c.CategoriaId).HasColumnName("id_categoria");
            entity.Property(c => c.SolicitanteId).HasColumnName("id_usuario_solicitante");
            entity.Property(c => c.TecnicoId).HasColumnName("id_usuario_tecnico");
            entity.Property(c => c.Resolucao).HasColumnName("ds_resolucao");
            entity.Property(c => c.ResolucaoAprovada).HasColumnName("fl_resolucao_aprovada");
            entity.Property(c => c.ResolucaoPropostaEm).HasColumnName("dt_resolucao");
            entity.Property(c => c.ConcluidoEm).HasColumnName("dt_conclusao");
            entity.Property(c => c.CriadoEm).HasColumnName("dt_cadastro");
            entity.Property(c => c.AtualizadoEm).HasColumnName("dt_atualizacao");
            entity.Property(c => c.FechadoEm).HasColumnName("dt_fechamento");

            entity.HasOne(c => c.Categoria)
                .WithMany(cat => cat.Chamados)
                .HasForeignKey(c => c.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Solicitante)
                .WithMany(u => u.ChamadosSolicitados)
                .HasForeignKey(c => c.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Tecnico)
                .WithMany(u => u.ChamadosAtribuidos)
                .HasForeignKey(c => c.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ComentarioChamado>(entity =>
        {
            entity.ToTable("comentarios_chamado");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id_comentario");
            entity.Property(c => c.ChamadoId).HasColumnName("id_chamado");
            entity.Property(c => c.UsuarioId).HasColumnName("id_usuario");
            entity.Property(c => c.Mensagem).HasColumnName("ds_mensagem").IsRequired();
            entity.Property(c => c.CriadoEm).HasColumnName("dt_cadastro");

            entity.HasOne(c => c.Chamado)
                .WithMany(c => c.Comentarios)
                .HasForeignKey(c => c.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HistoricoChamado>(entity =>
        {
            entity.ToTable("historico_chamado");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Id).HasColumnName("id_historico");
            entity.Property(h => h.ChamadoId).HasColumnName("id_chamado");
            entity.Property(h => h.UsuarioId).HasColumnName("id_usuario");
            entity.Property(h => h.Acao).HasColumnName("nm_acao").HasMaxLength(40).IsRequired();
            entity.Property(h => h.ValorAnterior).HasColumnName("ds_valor_anterior");
            entity.Property(h => h.ValorNovo).HasColumnName("ds_valor_novo");
            entity.Property(h => h.CriadoEm).HasColumnName("dt_cadastro");

            entity.HasOne(h => h.Chamado)
                .WithMany(c => c.Historico)
                .HasForeignKey(h => h.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
