using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infraestructure.Specialization
{
    public class SpecializationsDataModel
    {
        [Key]
        public required string Id { get; set; } // Primary key

        public required string Description { get; set; }
    }
}
