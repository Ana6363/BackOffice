using BackOffice.Domain.OperationRequest;
using BackOffice.Infraestructure.Appointement;
using BackOffice.Infraestructure.OperationRequest;

namespace BackOffice.Domain.Appointement
{
    public interface IAppointementRepository
    {
        Task<AppointementDataModel> AddAsync(Appointement appointement);
        Task<AppointementDataModel> GetByIdAsync(AppointementId id);
        Task<List<AppointementDataModel>> GetAllAsync();
        Task UpdateAsync(Appointement appointement);
        Task DeleteAsync(AppointementId id);
    }
}
