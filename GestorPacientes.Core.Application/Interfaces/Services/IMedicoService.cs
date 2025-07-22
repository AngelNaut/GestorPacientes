using GestorPacientes.Core.Application.ViewModels.Medicos;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IMedicoService : IGenericService<MedicoViewModel, SaveMedicoViewModel, EditMedicoViewModel>
    {
        Task<EditMedicoViewModel> GetByIdEditViewModelAsync(int id);
    }
}