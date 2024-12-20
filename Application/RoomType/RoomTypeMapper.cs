using Microsoft.EntityFrameworkCore.Query;
using BackOffice.Domain.RoomTypes;
using BackOffice.Infraestructure.RoomTypes;

namespace BackOffice.Application.RoomType
{
    public class RoomTypeMapper
    {
        public static RoomTypeDto ToDto(BackOffice.Domain.RoomTypes.RoomType roomType)
        {
            if (roomType == null)
                throw new ArgumentNullException(nameof(roomType), "RoomType cannot be null.");

            return new RoomTypeDto(roomType.Id.ToString(), roomType.Designation.ToString(), roomType.Description.ToString(), roomType.SurgerySuitability.ToString());
        }

        public static BackOffice.Domain.RoomTypes.RoomType ToDomain(RoomTypeDto roomTypeDto)
        {
            if (roomTypeDto == null)
                throw new ArgumentNullException(nameof(roomTypeDto), "RoomTypeDto cannot be null.");
            return new BackOffice.Domain.RoomTypes.RoomType(
                InternalCode.Create(roomTypeDto.InternalCode).ToString(),
                new RoomDesignation(roomTypeDto.Designation),
                new RoomDescription(roomTypeDto.Description),
                new SurgerySuitability(Enum.Parse<SurgerySuitability.Suitability>(roomTypeDto.SurgerySuitability))
            );
        }

        public static RoomTypeDataModel ToDataModel(BackOffice.Domain.RoomTypes.RoomType roomType)
        {
            if (roomType == null)
                throw new ArgumentNullException(nameof(roomType), "RoomType cannot be null.");
            return new RoomTypeDataModel
            {
                Id = roomType.Id.ToString(),
                Designation = roomType.Designation.ToString(),
                Description = roomType.Description.ToString(),
                SurgerySuitability = roomType.SurgerySuitability.ToString()
            };
        }

        public static BackOffice.Domain.RoomTypes.RoomType ToDomain(RoomTypeDataModel dataModel)
        {
            if (dataModel == null)
                throw new ArgumentNullException(nameof(dataModel), "RoomTypeDataModel cannot be null.");
            return new BackOffice.Domain.RoomTypes.RoomType(
                InternalCode.Create(dataModel.Id).ToString(), // Use the Create method instead of the constructor
                new RoomDesignation(dataModel.Designation),
                new RoomDescription(dataModel.Description),
                new SurgerySuitability(Enum.Parse<SurgerySuitability.Suitability>(dataModel.SurgerySuitability))
            );
        }

    }
}
