using System.Xml.Serialization;
using BackOffice.Domain.Specialization;
using BackOffice.Infraestructure.Specialization;

namespace BackOffice.Application.Specialization
{
    public class SpecializationMapper
    {

        public static SpecializationDto ToDto(Domain.Specialization.Specialization specialization)
        {
            if (specialization == null)
                throw new ArgumentNullException(nameof(specialization), "Specialization cannot be null.");

            return new SpecializationDto(
                specialization.Id.AsString(),
                specialization.Description.Value
            );
        }

        public static Domain.Specialization.Specialization ToDomain(SpecializationDto specializationDto)
        {
            if (specializationDto == null){
                throw new ArgumentNullException(nameof(specializationDto), "SpecializationDto cannot be null.");}
            return new Domain.Specialization.Specialization(
                new Specializations(specializationDto.Name),
                new Description(specializationDto.Description) 
            );
        }

        public static SpecializationsDataModel ToDataModel(Domain.Specialization.Specialization specialization)
        {
            if (specialization == null)
                throw new ArgumentNullException(nameof(specialization), "Specialization cannot be null.");
            return new SpecializationsDataModel
            {
                Id = specialization.Id.AsString(),
                Description = specialization.Description.Value
            };
        }

        public static Domain.Specialization.Specialization ToDomain(SpecializationsDataModel dataModel)
        {
            if (dataModel == null)
                throw new ArgumentNullException(nameof(dataModel), "SpecializationsDataModel cannot be null.");
            return new Domain.Specialization.Specialization(
                new Specializations(dataModel.Id),
                new Description(dataModel.Description)
            );
        }
    }
}
