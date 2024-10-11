using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Common.Attribute;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return false; // null の場合もエラーとする
        }

        if (value is Guid guidValue)
        {
            return guidValue != Guid.Empty; // Guid.Empty を無効とする
        }

        return false; // Guid でない場合もエラーとする
    }

    public override string FormatErrorMessage(string name) => $"{name} must not be an empty GUID.";
}
