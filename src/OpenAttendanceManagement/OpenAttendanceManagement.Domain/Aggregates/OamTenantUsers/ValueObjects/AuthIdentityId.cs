using System.ComponentModel.DataAnnotations;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record AuthIdentityId(
    [property: MaxLength(100)] string Value)
{
    public static AuthIdentityId Default => new(string.Empty);
    public static AuthIdentityId FromString(string value) => new(value);
}