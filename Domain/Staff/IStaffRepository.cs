using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Staff
{
    public interface IStaffRepository
    {
        Task<StaffDataModel> GetByLicenseNumberAsync(string licenseNumber);
        Task<List<StaffDataModel>> GetAllAsync();
        Task AddAsync(StaffDataModel staff);
        Task UpdateAsync(StaffDataModel staff);
        Task DeleteAsync(string licenseNumber);
        Task<List<AvailableSlotDataModel>> GetAvailableSlotsByStaffAsync(string licenseNumber);
        Task AddAvailableSlotAsync(AvailableSlotDataModel slot);
        Task RemoveAvailableSlotAsync(int slotId);
    }
}
