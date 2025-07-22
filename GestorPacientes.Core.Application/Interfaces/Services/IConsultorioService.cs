using GestorPacientes.Core.Application.ViewModels.Citas;
using GestorPacientes.Core.Application.ViewModels.Consultorios;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IConsultorioService : IGenericService<ConsultorioViewModel, SaveConsultorioViewModel, EditCitaViewModel>
    {
       
    }
}