using System;

namespace BackOffice.Domain.SurgeryPhase
{
    public class SurgeryPhaseDto
{
    public int? Id { get; set; }
    public string? RoomNumber { get; set; }
    public string? PhaseType { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? AppointementId { get; set; }
    

}
}