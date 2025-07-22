using GestorPacientes.Core.Application.ViewModels.Citas;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface ICitaService : IGenericService<CitaViewModel, SaveCitaViewModel, EditCitaViewModel>
    {
       
    }
}