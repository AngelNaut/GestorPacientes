using GestorPacientes.Core.Application.Interfaces.Repositories;
using GestorPacientes.Infraestructure.Persistence.Contexts;
using GestorPacientes.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestorPacientes.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceDependency(this IServiceCollection services, IConfiguration configuration)
        {
            #region "DataBase Configuration"
            if (configuration.GetValue<bool>("UseDatabaseInMemory"))
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<GestorPacienteContext>(options => options.UseInMemoryDatabase("GestorPaciente"));
            }
            else
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<GestorPacienteContext>(options => options.UseSqlServer(connectionString, m=> m.MigrationsAssembly(typeof(GestorPacienteContext).Assembly.FullName)));
            }
            
            #endregion

            #region "Repositories DI"
            // Registro del repositorio genérico
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Registro de los repositorios específicos utilizados en los servicios
            services.AddTransient<ICitaRepository, CitaRepository>();
            services.AddTransient<IConsultorioRepository, ConsultorioRepository>();
            services.AddTransient<IMedicoRepository, MedicoRepository>();
            services.AddTransient<IPacienteRepository, PacienteRepository>();
            services.AddTransient<IPruebaLaboratorioRepository, PruebaLaboratorioRepository>();
            services.AddTransient<IResultadoLaboratorioRepository, ResultadoLaboratorioRepository>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();

            #endregion
        }
    }
}

