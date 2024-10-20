using System;

namespace BackOffice.Domain.Staff
{
    public class SlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public SlotDto() {}

    public SlotDto(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }


     public override string ToString()
        {
            return $"{StartTime:yyyy-MM-dd HH:mm} - {EndTime:yyyy-MM-dd HH:mm}";
        }
}
}