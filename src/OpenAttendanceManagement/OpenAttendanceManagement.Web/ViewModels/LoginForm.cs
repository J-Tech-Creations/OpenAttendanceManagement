namespace OpenAttendanceManagement.Web.ViewModels;

public class LoginForm
{
    public string? Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
