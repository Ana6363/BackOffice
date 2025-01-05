using BackOffice.Domain.RoomTypes;
using BackOffice.Domain.Shared;
using BackOffice.Infraestructure.RoomTypes;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.RoomType
{
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly BackOfficeDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository, BackOfficeDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _roomTypeRepository = roomTypeRepository;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<RoomTypeDataModel> CreateRoomType(RoomTypeDto roomTypeDto)
        {
            var existingRoomType = await _dbContext.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomTypeDto.InternalCode);
            if (existingRoomType != null)
            {
                throw new ArgumentException("Room type already exists.");
            }

            var roomType = RoomTypeMapper.ToDomain(roomTypeDto);

            if (roomType == null) {
                throw new ArgumentException("Room type failed mapping.");
            }

            try
            {
                return await _roomTypeRepository.AddAsync(roomType);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                throw new ArgumentException("Room type failed to be created.");
            }
        }
        public async Task<List<RoomTypeDto>> GetAllRoomTypesAsync()
            {
                try
                {
                    return await _dbContext.RoomTypes
                        .Select(rt => new RoomTypeDto(
                            rt.Id,
                            rt.Designation,
                            rt.Description,
                            rt.SurgerySuitability.ToString()
                        ))
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to retrieve room types.", ex);
                }
            }
        
        }
}
