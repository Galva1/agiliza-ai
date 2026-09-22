using HelpDesk.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
<<<<<<< HEAD
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
=======
    public DbSet<Chamado> Chamados => Set<Chamado>();
    public DbSet<ChamadoComentario> ChamadoComentarios => Set<ChamadoComentario>();
    public DbSet<StatusChamado> StatusChamados => Set<StatusChamado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---- usuario ----
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("idusuario");
            entity.Property(u => u.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).HasColumnName("senhaHash").IsRequired();
            entity.Property(u => u.Perfil).HasColumnName("role").HasConversion<int>();
            entity.Property(u => u.Ativo).HasColumnName("ativo");
            entity.Property(u => u.DataCriacao).HasColumnName("dt_criacao");
            entity.Property(u => u.CriadoPorId).HasColumnName("usr_criacao");
            entity.Property(u => u.DataAlteracao).HasColumnName("dt_alteracao");
            entity.Property(u => u.AlteradoPorId).HasColumnName("usr_alteracao");

            // Auditoria: quem criou/alterou o usuário (autorreferência, sem navegação exposta).
            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(u => u.CriadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(u => u.AlteradoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---- ticket_status ----
        modelBuilder.Entity<StatusChamado>(entity =>
        {
            entity.ToTable("ticket_status");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("idstatus");
            entity.Property(s => s.Nome).HasColumnName("nm_status").HasMaxLength(50).IsRequired();
            entity.Property(s => s.DataCriacao).HasColumnName("dt_criacao");
            entity.Property(s => s.Ativo).HasColumnName("ativo");
            entity.HasIndex(s => s.Nome).IsUnique();
        });

        // ---- ticket ----
        modelBuilder.Entity<Chamado>(entity =>
        {
            entity.ToTable("ticket");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasColumnName("idticket");
            entity.Property(t => t.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
            entity.Property(t => t.Descricao).HasColumnName("descricao").IsRequired();
            entity.Property(t => t.StatusChamadoId).HasColumnName("idstatus");
            entity.Property(t => t.Prioridade).HasColumnName("priority").HasConversion<int>();
            entity.Property(t => t.SolicitanteId).HasColumnName("idusuario_solicitante");
            entity.Property(t => t.ResponsavelId).HasColumnName("idusuario_responsavel");
            entity.Property(t => t.Ativo).HasColumnName("ativo");
            entity.Property(t => t.DataCriacao).HasColumnName("dt_criacao");
            entity.Property(t => t.CriadoPorId).HasColumnName("usr_criacao");
            entity.Property(t => t.DataAlteracao).HasColumnName("dt_alteracao");
            entity.Property(t => t.AlteradoPorId).HasColumnName("usr_alteracao");
            entity.Property(t => t.DataFechamento).HasColumnName("dt_fechamento");

            entity.HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.StatusChamadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Solicitante)
                .WithMany(u => u.ChamadosSolicitados)
                .HasForeignKey(t => t.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Responsavel)
                .WithMany(u => u.ChamadosAtribuidos)
                .HasForeignKey(t => t.ResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(t => t.CriadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(t => t.AlteradoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ---- ticket_comentario ----
        modelBuilder.Entity<ChamadoComentario>(entity =>
        {
            entity.ToTable("ticket_comentario");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("idticket_comentario");
            entity.Property(c => c.ChamadoId).HasColumnName("idticket");
            entity.Property(c => c.Mensagem).HasColumnName("mensagem").IsRequired();
            entity.Property(c => c.Ativo).HasColumnName("ativo");
            entity.Property(c => c.DataCriacao).HasColumnName("dt_criacao");
            // Autor do comentário == quem criou o registro (usr_criacao).
            entity.Property(c => c.AutorId).HasColumnName("usr_criacao");
            entity.Property(c => c.DataAlteracao).HasColumnName("dt_alteracao");
            entity.Property(c => c.AlteradoPorId).HasColumnName("usr_alteracao");

            entity.HasOne(c => c.Chamado)
                .WithMany(t => t.Comentarios)
                .HasForeignKey(c => c.ChamadoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Autor)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.AlteradoPorId)
>>>>>>> 974e11b1c3aaa77e243ec890700622933fdbd10d
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
