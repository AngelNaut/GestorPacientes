using GestorPacientes.Core.Domain.Common;
using GestorPacientes.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;




namespace GestorPacientes.Infraestructure.Persistence.Contexts
{
    public class GestorPacienteContext : DbContext
    {
        public GestorPacienteContext(DbContextOptions<GestorPacienteContext> options) : base(options) { }

        // DbSets para cada entidad
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Consultorio> Consultorios { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<PruebaLaboratorio> PruebasLaboratorio { get; set; }
        public DbSet<ResultadoLaboratorio> ResultadosLaboratorio { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.Now;
                        entry.Entity.CreatedBy = "Sistema";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.Now;
                        entry.Entity.LastModifiedBy = "Sistema";
                        break;
                }
            }
                return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Configuraciones Globales
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #endregion

            #region Nombres de Tablas
            modelBuilder.Entity<Cita>().ToTable("Citas");
            modelBuilder.Entity<Consultorio>().ToTable("Consultorios");
            modelBuilder.Entity<Medico>().ToTable("Medicos");
            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<PruebaLaboratorio>().ToTable("PruebasLaboratorio");
            modelBuilder.Entity<ResultadoLaboratorio>().ToTable("ResultadosLaboratorio");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            #endregion

            #region Claves Primarias
            modelBuilder.Entity<Cita>().HasKey(c => c.Id);
            modelBuilder.Entity<Consultorio>().HasKey(c => c.Id);
            modelBuilder.Entity<Medico>().HasKey(m => m.Id);
            modelBuilder.Entity<Paciente>().HasKey(p => p.Id);
            modelBuilder.Entity<PruebaLaboratorio>().HasKey(p => p.Id);
            modelBuilder.Entity<ResultadoLaboratorio>().HasKey(r => r.Id);
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            #endregion

            #region Relaciones

            // Relación Consultorio 1:* Usuarios
            modelBuilder.Entity<Consultorio>()
                .HasMany(c => c.Usuarios)
                .WithOne(u => u.Consultorio)
                .HasForeignKey(u => u.ConsultorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Consultorio 1:* Medicos
            modelBuilder.Entity<Consultorio>()
                .HasMany(c => c.Medicos)
                .WithOne(m => m.Consultorio)
                .HasForeignKey(m => m.ConsultorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Consultorio 1:* Pacientes
            modelBuilder.Entity<Consultorio>()
                .HasMany(c => c.Pacientes)
                .WithOne(p => p.Consultorio)
                .HasForeignKey(p => p.ConsultorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Consultorio 1:* PruebasLaboratorio
            modelBuilder.Entity<Consultorio>()
                .HasMany(c => c.PruebasLaboratorio)
                .WithOne(p => p.Consultorio)
                .HasForeignKey(p => p.ConsultorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Consultorio 1:* Citas
            modelBuilder.Entity<Consultorio>()
                .HasMany(c => c.Citas)
                .WithOne(c => c.Consultorio)
                .HasForeignKey(c => c.ConsultorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Consultorio 1:* ResultadosLaboratorio
            modelBuilder.Entity<Consultorio>()
                .HasMany(c => c.ResultadosLaboratorio)
                .WithOne(r => r.Consultorio)
                .HasForeignKey(r => r.ConsultorioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Cita 1:* ResultadosLaboratorio
            modelBuilder.Entity<Cita>()
                .HasMany(c => c.ResultadosLaboratorio)
                .WithOne(r => r.Cita)
                .HasForeignKey(r => r.CitaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Cita 1:1 Paciente
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Cita 1:1 Medico
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Medico)
                .WithMany(m => m.Citas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación ResultadoLaboratorio 1:1 PruebaLaboratorio
            modelBuilder.Entity<ResultadoLaboratorio>()
                .HasOne(r => r.PruebaLaboratorio)
                .WithMany(p => p.ResultadosLaboratorio)
                .HasForeignKey(r => r.PruebaLaboratorioId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Configuración de Propiedades

            // Configuración para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(u => u.TipoUsuario)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.NombreUsuario)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Contrasena)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(u => u.NombreUsuario).IsUnique(); // Nombre de usuario único
                entity.HasIndex(u => u.Correo).IsUnique(); // Correo único
            });

            // Configuración para Medico
            modelBuilder.Entity<Medico>(entity =>
            {
                entity.Property(m => m.Cedula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(m => m.Cedula).IsUnique(); // Cédula única
            });

            // Configuración para Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.Property(p => p.Cedula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(p => p.Cedula).IsUnique(); // Cédula única
            });

            // Configuración para Cita
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.Property(c => c.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(20); // "PendienteConsulta", "PendienteResultados", "Completada"
            });

            // Configuración para ResultadoLaboratorio
            modelBuilder.Entity<ResultadoLaboratorio>(entity =>
            {
                entity.Property(r => r.Estado)
                    .HasConversion<string>()
                    .HasMaxLength(20); // "Pendiente", "Completada"
            });

            #endregion
        }
    }
}