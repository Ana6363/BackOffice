using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infraestructure.RoomTypes
{
    public class RoomTypeDataModel
    {
        [Key]
        public required string Id { get; set; } 
        public required string Designation{ get; set; }
        public string Description { get; set; }
        public required string SurgerySuitability { get; set; }

    }
}
