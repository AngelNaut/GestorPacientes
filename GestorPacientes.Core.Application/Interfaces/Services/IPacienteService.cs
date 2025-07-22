using GestorPacientes.Core.Application.ViewModels.Pacientes;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IPacienteService : IGenericService<PacienteViewModel, SavePacienteViewModel, EditPacienteViewModel>
    {
        Task<EditPacienteViewModel> GetByIdEditViewModelAsync(int id);
    }
}