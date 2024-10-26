namespace BackOffice.Application.Staffs
{
    public class StaffDeactivateDto
    {
        public string StaffId { get; set; }

        public StaffDeactivateDto(string staffId)
        {
            StaffId = staffId;

        }
      
    }
}
