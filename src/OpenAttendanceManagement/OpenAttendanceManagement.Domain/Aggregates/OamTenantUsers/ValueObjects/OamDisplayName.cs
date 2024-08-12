using System.ComponentModel.DataAnnotations;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record OamDisplayName(
    [property: RegularExpression("^[a-zA-Z0-9]{1,30}$")]
    string Value)
{
    public static OamDisplayName Default => new(string.Empty);
    public static OamDisplayName FromString(string value) => new(value);
}