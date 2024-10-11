namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;

public record OamTerm(DateOnly Start, DateOnly End)
{
    public static OamTerm Default => new(DateOnly.MinValue, DateOnly.MinValue);
    public static OamTerm MonthTerm(int year, int month) => new(
        new DateOnly(year, month, 1),
        new DateOnly(year, month, DateTime.DaysInMonth(year, month)));
    public OamTerm NextMonthTerm() => new(End.AddDays(1), End.AddMonths(1));
}
