using BackOffice.Infraestructure.RoomTypes;

namespace BackOffice.Domain.RoomTypes
{
    public interface IRoomTypeRepository
    {
        Task<RoomTypeDataModel> AddAsync(RoomType roomType);
    }
}
