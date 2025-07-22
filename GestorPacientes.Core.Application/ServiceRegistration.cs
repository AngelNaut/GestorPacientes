using GestorPacientes.Core.Application.Interfaces.Services;
using GestorPacientes.Core.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GestorPacientes.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddAplicationDependency(this IServiceCollection services)
        {
            #region "Services DI"
            services.AddTransient<ICitaService, CitaService>();
            services.AddTransient<IConsultorioService, ConsultorioService>();
            services.AddTransient<IMedicoService, MedicoService>();
            services.AddTransient<IPacienteService, PacienteService>();
            services.AddTransient<IPruebaLaboratorioService, PruebaLaboratorioService>();
            services.AddTransient<IResultadoLaboratorioService, ResultadoLaboratorioService>();
            services.AddTransient<IUsuarioService, UsuarioService>();
            #endregion
        }
    }

}
