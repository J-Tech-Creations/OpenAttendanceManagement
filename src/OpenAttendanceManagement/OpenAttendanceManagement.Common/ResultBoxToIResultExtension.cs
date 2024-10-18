using ResultBoxes;
namespace OpenAttendanceManagement.Common;

public static class ResultBoxToIResultExtension
{
    public static IResult ToResults<TResult>(this ResultBox<TResult> resultBox) where TResult : notnull
        => Results.Ok(resultBox.UnwrapBox());

    public static async Task<IResult> ToResults<TResult>(this Task<ResultBox<TResult>> resultBox)
        where TResult : notnull
        => Results.Ok(await resultBox.UnwrapBox());
}
