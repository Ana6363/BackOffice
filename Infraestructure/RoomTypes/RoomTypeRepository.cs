using BackOffice.Domain.RoomTypes;
using BackOffice.Infrastructure;

namespace BackOffice.Infraestructure.RoomTypes
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly BackOfficeDbContext _dbContext;

        public RoomTypeRepository(BackOfficeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RoomTypeDataModel> AddAsync(RoomType roomType)
        {
            if (roomType == null)
            {
                throw new ArgumentNullException(nameof(roomType), "RoomType cannot be null.");
            }
            var roomTypeDataModel = new RoomTypeDataModel
            {
                Id = roomType.Id.AsString(),
                Designation = roomType.Designation.ToString(),
                Description = roomType.Description.ToString(),
                SurgerySuitability = roomType.SurgerySuitability.ToString()
            };
            await _dbContext.RoomTypes.AddAsync(roomTypeDataModel);
            await _dbContext.SaveChangesAsync();
            return roomTypeDataModel;
        }
    }
}
