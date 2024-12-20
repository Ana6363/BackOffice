namespace BackOffice.Application.SurgeryRoom
{
    public class SurgeryRoomDto
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }

        public SurgeryRoomDto(string roomNumber, string type, int capacity)
        {
            RoomNumber = roomNumber;
            Type = type;
            Capacity = capacity;
        }

        
    }
}
