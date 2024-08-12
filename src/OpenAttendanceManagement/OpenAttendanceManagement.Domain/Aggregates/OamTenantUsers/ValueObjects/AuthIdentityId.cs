using System.ComponentModel.DataAnnotations;
using ResultBoxes;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record AuthIdentityId(
    [property: MaxLength(100)] string Value)
{
    public static AuthIdentityId Default => new(string.Empty);
    public static AuthIdentityId FromString(string value) => new(value);

    public static OptionalValue<AuthIdentityId> FromOptionalString(OptionalValue<string> value) => value.HasValue
        ? OptionalValue.FromValue(new AuthIdentityId(value.GetValue()))
        : OptionalValue<AuthIdentityId>.Empty;
}