using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class SurgeryRoomDataModel
{
     [Key]
    public string RoomNumber { get; set; }
    public string Type { get; set; }
    public int Capacity { get; set; }
    public string CurrentStatus { get; set; }

    // Navigation properties for associated entities
    public List<SurgeryPhaseDataModel> Phases { get; set; } = new List<SurgeryPhaseDataModel>();
    public List<MaintenanceSlot> MaintenanceSlots { get; set; } = new List<MaintenanceSlot>();
    public List<AssignedEquipment> Equipments { get; set; } = new List<AssignedEquipment>();
}

public class SurgeryPhaseDataModel
{
     [Key]
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public string PhaseType { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string AppointementId { get; set; }


    // Navigation property
    public SurgeryRoomDataModel SurgeryRoom { get; set; }
}

public class MaintenanceSlot
{
     [Key]
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    // Navigation property
    public SurgeryRoomDataModel SurgeryRoom { get; set; }
}

public class AssignedEquipment
{
    [Key]
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public string EquipmentName { get; set; }

    // Navigation property
    public SurgeryRoomDataModel SurgeryRoom { get; set; }
}
