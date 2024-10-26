using System;

namespace BackOffice.Domain.Staff
{
    public class SlotDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string? StaffId { get; set; }

    public SlotDto() {}

    public SlotDto(int id,DateTime startTime, DateTime endTime)
    {
        Id= id;
        StartTime = startTime;
        EndTime = endTime;
    }


     public override string ToString()
        {
            return $"{StartTime:yyyy-MM-dd HH:mm} - {EndTime:yyyy-MM-dd HH:mm}";
        }
}
}