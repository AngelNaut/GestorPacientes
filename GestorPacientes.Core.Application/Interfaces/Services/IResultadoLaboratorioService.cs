using GestorPacientes.Core.Application.ViewModels.ResultadoLaboratorios;

namespace GestorPacientes.Core.Application.Interfaces.Services
{
    public interface IResultadoLaboratorioService : IGenericService<ResultadoLaboratorioViewModel, SaveResultadoLaboratorioViewModel, EditResultadoLaboratorioViewModel>
    {
        Task<List<ResultadoLaboratorioViewModel>> GetPendingResultsAsync(string cedula);

    }
}