using System.ComponentModel.DataAnnotations;

namespace FamilyDashboard.Blazor.Models.SmartHome;

public enum SmartHomePresenceState
{
    Present = 1,
    [Display(Name = "Not Present")]
    NotPresent = 2,
    Unknown = 0
}
