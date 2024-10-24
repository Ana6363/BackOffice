using System.ComponentModel.DataAnnotations;
using BackOffice.Infrastructure.Staff;

public class AvailableSlotDataModel
{
    [Key]
    public int Id { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string? StaffId { get; set; }

    public StaffDataModel Staff { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is AvailableSlotDataModel other)
        {
            return this.StartTime == other.StartTime && this.EndTime == other.EndTime;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartTime, EndTime);
    }
}
