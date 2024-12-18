namespace BackOffice.Application.RoomType
{
    public class RoomTypeDto
    {
        public string InternalCode { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }
        public string SurgerySuitability { get; set; }

        public RoomTypeDto(string internalCode, string designation, string description, string surgerySuitability)
        {
            InternalCode = internalCode;
            Designation = designation;
            Description = description;
            SurgerySuitability = surgerySuitability;
        }
    }
}
