namespace OpenAttendanceManagement.AuthCommon;

public record RealmAccess(string[] Roles)
{
    public bool IsInRole(string role) => Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
}
