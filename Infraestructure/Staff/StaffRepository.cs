using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Staff
{
    public class StaffRepository : IStaffRepository
    {
        private readonly BackOfficeDbContext _context;

        public StaffRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        public async Task<StaffDataModel> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.Staff
                .Include(s => s.AvailableSlots)
                .FirstOrDefaultAsync(s => s.LicenseNumber == licenseNumber);
        }

        public async Task<List<StaffDataModel>> GetAllAsync()
        {
            return await _context.Staff
                .Include(s => s.AvailableSlots)
                .ToListAsync();
        }

        public async Task AddAsync(StaffDataModel staff)
        {
            if (staff == null) throw new ArgumentNullException(nameof(staff));
            await _context.Staff.AddAsync(staff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StaffDataModel staff)
        {
            if (staff == null) throw new ArgumentNullException(nameof(staff));
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string licenseNumber)
        {
            var staff = await GetByLicenseNumberAsync(licenseNumber);
            if (staff == null) throw new Exception("Staff not found.");

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AvailableSlotDataModel>> GetAvailableSlotsByStaffAsync(string licenseNumber)
        {
            var staff = await GetByLicenseNumberAsync(licenseNumber);
            return staff?.AvailableSlots ?? new List<AvailableSlotDataModel>();
        }

        public async Task AddAvailableSlotAsync(AvailableSlotDataModel slot)
        {
            if (slot == null) throw new ArgumentNullException(nameof(slot));

            await _context.AvailableSlots.AddAsync(slot);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAvailableSlotAsync(int slotId)
        {
            var slot = await _context.AvailableSlots.FindAsync(slotId);
            if (slot == null) throw new Exception("Available slot not found.");

            _context.AvailableSlots.Remove(slot);
            await _context.SaveChangesAsync();
        }
    }
}
