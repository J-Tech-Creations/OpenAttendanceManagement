using OpenAttendanceManagement.Common.Attribute;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;

public record OamTermUserId(
    [property: NotEmptyGuid]
    [property: Required]
    Guid Value)
{
    public static OamTermTenantId Default => new(Guid.Empty);
}
public record OamTeamUserWithUserId(
    OamTermUserId TermUserId,
    OamTenantUserId TenantUserId,
    OamUserName UserName);
