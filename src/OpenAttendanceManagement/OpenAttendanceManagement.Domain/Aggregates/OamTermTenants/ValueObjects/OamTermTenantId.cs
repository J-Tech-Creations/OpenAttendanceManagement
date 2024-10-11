using OpenAttendanceManagement.Common.Attribute;
using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;

public record OamTermTenantId(
    [property: NotEmptyGuid]
    [property: Required]
    Guid Value)
{
    public static OamTermTenantId Default => new(Guid.Empty);
}
