using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record AuthIdentityEmail(
    [property: EmailAddress]
    string Value)
{
    public static AuthIdentityEmail Default => new(string.Empty);
}
