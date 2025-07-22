using GestorPacientes.Core.Application.ViewModels.PruebaLaboratorios;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IPruebaLaboratorioService : IGenericService<PruebaLaboratorioViewModel, SavePruebaLaboratorioViewModel, EditPruebaLaboratorioViewModel>
    {
        Task<EditPruebaLaboratorioViewModel> GetByIdEditViewModelAsync(int id);


    }
}