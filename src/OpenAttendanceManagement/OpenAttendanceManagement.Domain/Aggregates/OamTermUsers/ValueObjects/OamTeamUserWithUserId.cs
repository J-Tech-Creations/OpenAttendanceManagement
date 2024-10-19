using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;

public record OamTeamUserWithUserId(
    OamTermUserId TermUserId,
    OamTenantUserId TenantUserId,
    OamUserName UserName);
